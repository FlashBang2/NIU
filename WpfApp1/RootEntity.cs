using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1
{
    public partial class Entity
    {
        public static Entity Mario = null;
        public static bool ShowedCountOfTickyComponents = false;
        public static int NumTickyComponents = 0;
        private static int FramesToSkip = 4;

        public class RootEntity : Entity
        {
            public readonly IList<IEntity> _children = new List<IEntity>();
            private int _numFrame = 0;

            public RootEntity() : base("Root")
            {
                AddComponent<SharedAnimationManager>();
                if (SDLApp.ShouldShowFps)
                {
                    AddComponent<FrameRateRendererComponent>();
                }
            }

            public void AttachChild(IEntity entity)
            {
                if (entity is Entity)
                {
                    Entity e = (Entity)entity;
                }

                _children.Add(entity);
            }

            public override void Destroy()
            {
                base.Destroy();
                foreach (var child in _children)
                {
                    child.Destroy();
                }

                _children.Clear();
            }

            public IEntity FindChild(string name)
            {
                return _children.FirstOrDefault(child => child.name.Equals(name));
            }

            public override void ReceiveRender()
            {
                base.ReceiveRender();
                foreach (var child in _children)
                {
                    Entity e = (Entity)child;
                    if (Mario.IsInViewRect(e))
                    {
                        child.ReceiveRender();
                    }
                }
            }

            public bool RemoveChild(IEntity entity)
            {
                return _children.Remove(entity);
            }

            public override void Tick(float dt)
            {
                NumTickyComponents = 0;

                if (Mario == null)
                {
                    Mario = GetEntity("mario", true);
                }

                for (var i = 0; i < _children.Count; i++)
                {
                    if (FramesToSkip == 0 || Mario.IsInViewRect((Entity)_children[i]))
                    {
                        _children[i].Tick(dt);
                    }
                }

                base.Tick(dt);

                FramesToSkip--;

                if (FramesToSkip < 0)
                {
                    FramesToSkip = 4;
                }

                if (!ShowedCountOfTickyComponents)
                {
                    if (name.Equals("Root"))
                    {
                        Console.WriteLine("Num ticky components: " + NumTickyComponents);
                        ShowedCountOfTickyComponents = true;
                    }
                }

                if (_numFrame > 500)
                {
                    _numFrame = 0;
                    ShowedCountOfTickyComponents = false;
                }

                _numFrame++;
            }

            public IEntity[] GetChildren()
            {
                return _children.ToArray();
            }
        }

        public static Entity CreateEntity(string name)
        {
            Entity e = new Entity(name);
            rootEntity.AttachChild(e);
            return e;
        }

        public static Entity GetEntity(string name, bool shouldOnlyTop)
        {
            IEntity e = rootEntity.FindChild(name);
            return (Entity)e;
        }
    }
}
