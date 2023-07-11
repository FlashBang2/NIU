using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using static SDL2.SDL;

namespace WpfApp1
{
    public class Entity : IEntity
    {
        private IEntity _parent;
        private float _posX;
        private float _posY;
        private SDL_Rect _bounds;
        private string _name;
        private bool _active = true;

        private float _angle = 0;

        private readonly IList<IEntity> _children = new List<IEntity>();
        private readonly IList<IRenderable> _renderablesComponents = new List<IRenderable>();
        private readonly List<Component> _components = new List<Component>();
        private readonly IDictionary<Type, Component> _newComponents = new Dictionary<Type, Component>();

        public static Entity RootEntity = new Entity("Root");
        public float lastX = 0;
        public float lastY = 0;

        public static Entity CreateEntity(string name)
        {
            Entity e = new Entity(name);
            RootEntity.AttachChild(e);
            return e;
        }

        public static Entity GetEntity(string name, bool shouldOnlyTop)
        {
            IEntity e = RootEntity.FindChild(name);
            if (e == null)
            {
                if (!shouldOnlyTop)
                {
                    return null;
                    throw new ArgumentException("Couldn't find such entity");
                }

                foreach (var ch in RootEntity._children)
                {
                    e = ch.FindChild(name);
                    if (e != null)
                    {
                        return (Entity)e;
                    }
                }

                foreach (var ch in RootEntity._children)
                {
                    var child = (Entity)ch;

                    foreach (var ch2 in child._children)
                    {
                        e = ch2.FindChild(name);
                        if (e != null)
                        {
                            return (Entity)e;
                        }
                    }
                }
            }

            return (Entity)e;
        }

        public Entity(string name)
        {
            _parent = null;
            _name = name;
            _bounds = new SDL_Rect();
            _posX = 0;
            _posY = 0;
        }

        public float PosX
        {
            get => _posX; set
            {
                float offset = _posX - value;
                _posX = value;

                foreach (IEntity entity in _children)
                {
                    entity.AddWorldOffset(offset, 0);
                }


                lastX = offset + value;
            }
        }

        public float PosY
        {
            get => _posY; set
            {
                float offset = _posY - value;
                _posY = value;

                foreach (IEntity entity in _children)
                {
                    entity.AddWorldOffset(0, offset);
                }

                lastY = offset + value;
            }
        }

        public float Width { get => _bounds.w; set { SdlRectMath.FromXywh(PosX, PosY, value, Height, out _bounds); } }

        public string Name { get => _name; set => _name = value; }
        public float Height { get => _bounds.h; set { SdlRectMath.FromXywh(PosX, PosY, Width, value, out _bounds); } }

        public Vector Right => new Vector(Math.Cos(_angle / 180.0 * Math.PI), Math.Sin(_angle / 180.0f * Math.PI));

        public Vector Left => -Right;

        public Vector Up => new Vector(-Math.Sin(_angle / 180.0 * Math.PI), -Math.Cos(_angle / 180.0 * Math.PI));
        public Vector Down => -Up;

        public bool IsActive { get => _active; set => SetActive(value); }

        public SDL_Rect Bounds { get => _bounds; }

        public void SetActive(bool active)
        {
            if (_active)
            {
                if (_active != active)
                {
                    foreach (var component in _components)
                    {
                        component.Deactivated();
                    }
                }
            }
            else
            {
                if (_active != active)
                {
                    foreach (var component in _components)
                    {
                        component.Activated();
                    }
                }
            }
            _active = active;
        }
        public void AddComponent<T>() where T : Component
        {
            ConstructorInfo info = typeof(T).GetConstructors().Where(constructor => constructor.GetParameters().Length == 0).FirstOrDefault();

            if (info == null)
            {
                throw new ArgumentException(typeof(T).Name + " doesn't contain default constructor");
            }

            Component component = (Component)info.Invoke(null);
            component.OnComponentRegistered(this);
            if (typeof(IRenderable).IsInstanceOfType(component))
            {
                _renderablesComponents.Add((IRenderable)component);
            }

            component.Spawned();
            _newComponents.Add(typeof(T), component);
        }

        public void AddWorldOffset(float x, float y)
        {
            lastX = _posX;
            lastY = _posY;
            PosX += x;
            PosY += y;

            SdlRectMath.FromXywh(_posX, _posY, _bounds.w, _bounds.h, out _bounds);
        }

        public void AttachChild(IEntity entity)
        {
            if (entity is Entity)
            {
                Entity e = (Entity)entity;
                e._parent = this;
            }

            _children.Add(entity);
        }


        public void Destroy()
        {
            foreach (var component in _newComponents)
            {
                if (component.Value.Destroyed())
                {
                    return;
                }
            }

            foreach (var child in _children)
            {
                child.Destroy();
            }

            _parent?.RemoveChild(this);
            _children.Clear();
            _parent = null;
        }

        public virtual void Destroyed()
        {
            
        }

        public IEntity FindChild(string name)
        {
            return _children.FirstOrDefault(child => child.Name.Equals(name));
        }

        public T GetComponent<T>()
        {
            T component = (T)((object)_newComponents[typeof(T)]);
            return component;
        }

        public bool IsInViewRect(Entity other)
        {
            return SDL_IntersectRect(ref _bounds, ref other._bounds, out _) == SDL_bool.SDL_TRUE;
        }

        public void ReceiveRender()
        {
            if (!_active)
            {
                return;
            }

            foreach (var component in _newComponents)
            {
                if (component.Value is IRenderable)
                {
                    IRenderable renderable = (IRenderable)component.Value;

                    if (renderable.ShouldDraw)
                    {
                        component.Value.ReceiveRender();
                    }
                }
            }

            Entity mario = GetEntity("mario", true);

            foreach (var child in _children)
            {
                Entity e = (Entity)child;
                if (mario.IsInViewRect(e))
                {
                    child.ReceiveRender();
                }
            }
        }

        public bool RemoveChild(IEntity entity)
        {
            return _children.Remove(entity);
        }

        public void Rotate(float angle)
        {
            _angle += angle;

            foreach (var child in _children)
            {
                child.Rotate(angle);
            }
        }

        public void Spawned()
        {
        }

        public virtual void Tick(float dt)
        {
            if (!_active)
            {
                return;
            }

            foreach (var component in _newComponents)
            {
                component.Value.OnTick(dt);
            }
            Entity mario = Entity.GetEntity("mario", true);

            for (var i = 0; i < _children.Count; i++)
            {
                if (mario.IsInViewRect((Entity)_children[i]))
                {
                    _children[i].Tick(dt);
                }
            }

            if (PosY > SDLApp.GetInstance().GetAppHeight() /*&& !HasComponent<SkeletonComponent>()*/)
            {
                Destroy();
            }
        }

        public IEntity[] GetChildren()
        {
            return _children.ToArray();
        }

        public bool HasComponent<T>()
        {
            var x = _newComponents.ContainsKey(typeof(T));
            return x;
        }

        public void UndoLastTranslation()
        {
            PosX = lastX;
            PosY = lastY;
        }
    }
}
