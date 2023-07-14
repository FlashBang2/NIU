using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using static SDL2.SDL;

namespace WpfApp1
{
    public partial class Entity : IEntity
    {
        private float _posX;
        private float _posY;

        private SDL_Rect _bounds;
        private string _name;
        private bool _active = true;

        private readonly IList<IRenderable> _renderablesComponents = new List<IRenderable>();
        private readonly List<Component> _tickyComponents = new List<Component>();
        private readonly IDictionary<Type, Component> _newComponents = new Dictionary<Type, Component>();

        public float _lastX = 0;
        public float _lastY = 0;

        public static RootEntity rootEntity = new RootEntity();

        public Entity(string name)
        {
            _name = name;
            _bounds = new SDL_Rect();
            _posX = 0;
            _posY = 0;
        }

        public float posX
        {
            get => _posX; set
            {
                _lastY = _posX;
                _posX = value;
                SdlRectMath.FromXywh(_posX, _posY, width, height, out _bounds);
            }
        }

        public float posY
        {
            get => _posY; set
            {
                _lastY = _posY;
                _posY = value;
                
                SdlRectMath.FromXywh(_posX, _posY, width, height, out _bounds);
            }
        }

        public float width { get => _bounds.w; set { SdlRectMath.FromXywh(posX, posY, value, height, out _bounds); } }

        public string name { get => _name; set => _name = value; }
        public float height { get => _bounds.h; set { SdlRectMath.FromXywh(posX, posY, width, value, out _bounds); } }

        public bool isActive { get => _active; set => SetActive(value); }

        public SDL_Rect bounds { get => _bounds; }

        public void SetActive(bool active)
        {
            if (_active)
            {
                if (_active != active)
                {
                    foreach (var component in _newComponents.Values)
                    {
                        component.Deactivated();
                    }
                }
            }
            else
            {
                if (_active != active)
                {
                    foreach (var component in _newComponents.Values)
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
            posX += x;
            posY += y;

            SdlRectMath.FromXywh(_posX, _posY, _bounds.w, _bounds.h, out _bounds);
        }

        public virtual void Destroy()
        {
            foreach (var component in _newComponents)
            {
                if (component.Value.Destroyed())
                {
                    return;
                }
            }

            if (this != rootEntity)
            {
                rootEntity.RemoveChild(this);
            }
        }

        public T GetComponent<T>()
        {
            T component = (T)((object)_newComponents[typeof(T)]);
            return component;
        }

        public bool IsInViewRect(Entity other)
        {
            int w = SDLRendering._screenWidth;
            int h = SDLRendering._screenHeight;
            SdlRectMath.FromXywh(_bounds.x - w / 2 < 0 ? 0 : _bounds.x - w / 2, _bounds.y - h / 2 < 0 ? 0 : _bounds.y - h / 2, w, h, out SdlRectMath.DummyEndResult);
            return SDL_IntersectRect(ref SdlRectMath.DummyEndResult, ref other._bounds, out SdlRectMath.DummyEndResultAlternative) == SDL_bool.SDL_TRUE;
        }

        public virtual void ReceiveRender()
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

                    if (renderable.shouldDraw)
                    {
                        component.Value.ReceiveRender();
                    }
                }
            }

            if (Mario == null)
            {
                Mario = GetEntity("mario", true);
            }
        }

        public virtual void Tick(float deltaTime)
        {
            if (!_active)
            {
                return;
            }

            foreach (var component in _tickyComponents)
            {
                component.OnTick(deltaTime);
            }

            if (!ShowedCountOfTickyComponents)
            {
                NumTickyComponents += _tickyComponents.Count;
            }

            if (Mario == null)
            {
                Mario = GetEntity("mario", true);
            }

            if (posY > SDLApp.GetInstance().GetAppHeight() /*&& !HasComponent<SkeletonComponent>()*/)
            {
                Destroy();
            }
        }

        public bool HasComponent<T>()
        {
            var x = _newComponents.ContainsKey(typeof(T));
            return x;
        }

        public void UndoLastTranslation()
        {
            posX = _lastX;
            posY = _lastY;
        }

        public void AddToTickList(Component component)
        {
            _tickyComponents.Add(component);
        }

        public void RemoveFromTickList(Component component)
        {
            _tickyComponents.Remove(component);
        }
    }
}
