using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace WpfApp1
{
    public class Entity : IEntity
    {
        private IEntity _parent;
        private double _posX;
        private double _posY;
        private Rect _bounds;
        private string _name;
        private bool _active = true;

        private double _angle = 0;

        private readonly IList<IEntity> _children = new List<IEntity>();
        private readonly IList<IRenderable> _renderablesComponents = new List<IRenderable>();
        private readonly List<Component> _components = new List<Component>();

        public static Entity RootEntity = new Entity("Root");
        public Vector _lastPos = new Vector();

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
            _bounds = new Rect(new Vector(), new Vector());
            _posX = 0;
            _posY = 0;
        }

        public double PosX
        {
            get => _posX; set
            {
                double offset = _posX - value;
                _posX = value;

                foreach (IEntity entity in _children)
                {
                    entity.AddWorldOffset(offset, 0);
                }

                _lastPos = new Vector(offset + value, _lastPos.Y);
            }
        }

        public double PosY
        {
            get => _posY; set
            {
                double offset = _posY - value;
                _posY = value;

                foreach (IEntity entity in _children)
                {
                    entity.AddWorldOffset(0, offset);
                }


                _lastPos = new Vector(_lastPos.X, offset + value);
            }
        }

        public double Width { get => _bounds.Width; set => _bounds = Rect.FromOriginAndExtend(_bounds.Center, new Vector(value, _bounds.Height)); }
        public string Name { get => _name; set => _name = value; }
        public double Height { get => _bounds.Height; set => _bounds = Rect.FromOriginAndExtend(_bounds.Center, new Vector(_bounds.Width, value)); }

        public Vector Right => new Vector(Math.Cos(_angle / 180.0 * Math.PI), Math.Sin(_angle / 180.0f * Math.PI));

        public Vector Left => -Right;

        public Vector Up => new Vector(-Math.Sin(_angle / 180.0 * Math.PI), Math.Cos(_angle / 180.0 * Math.PI));
        public Vector Down => -Up;

        public bool IsActive { get => _active; set => _active = value; }

        public Rect Bounds { get => new Rect(_posX, _posY, Width, Height); }

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
            _components.Add(component);
        }

        public void AddLocalOffset(double x, double y)
        {
            var totalDirVector = Up + Right;

            AddWorldOffset(totalDirVector.X * x, totalDirVector.Y * y);
        }

        public void AddWorldOffset(double x, double y)
        {
            Ray ray = new Ray();
            ray.Init(new Vector(PosX, PosY), new Vector(PosX + x, PosY + y));
            PosX += x;
            PosY += y;
#if false
            if (!Physics.TestRay(ray, _collideable))
            {
                PosX += x;
                PosY += y;
                
                foreach (var child in _children)
                {
                    child.AddWorldOffset(x, y);
                }
            }
#endif
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
            Destroyed();
            foreach (var child in _children)
            {
                child.Destroy();
            }

            foreach (var component in _components)
            {
                component.Destroyed();
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
            T component = (T)(object)_components.FindAll(v => typeof(T).IsInstanceOfType(v)).First();
            return component;
        }

        public void ReceiveRender()
        {
            foreach (var component in _components)
            {
                component.ReceiveRender();
            }

            foreach (var child in _children)
            {
                child.ReceiveRender();
            }
        }

        public bool RemoveChild(IEntity entity)
        {
            return _children.Remove(entity);
        }

        public void Rotate(double angle)
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

        public virtual void Tick(double dt)
        {
            foreach (var component in _components)
            {
                component.OnTick((float)dt);
            }

            foreach (var child in _children)
            {
                child.Tick(dt);
            }

        }

        public IEntity[] GetChildren()
        {
            return _children.ToArray();
        }

        public bool HasComponent<T>()
        {
            return GetComponent<T>() != null;
        }

        public void UndoLastTranslation()
        {
            PosX = _lastPos.X;
            PosY = _lastPos.Y;
        }
    }
}
