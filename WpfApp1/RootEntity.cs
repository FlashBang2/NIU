using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WpfApp1.Components;

using static SDL2.SDL;

namespace WpfApp1
{
    public partial class Entity
    {
        public static Entity Mario = null;
        public static bool ShowedCountOfTickyComponents = false;
        public static int NumTickyComponents = 0;
        public static int totalFrames = 0;
        public static readonly int FrameDelay = 8;
        private static int numFramesDelayLeft = FrameDelay;

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
                    if (CanUpdateChild(i))
                    {
                        _children[i].Tick(dt);
                    }
                }

                base.Tick(dt);

                numFramesDelayLeft--;

                if (numFramesDelayLeft < 0)
                {
                    numFramesDelayLeft = 4;
                }

                if (!ShowedCountOfTickyComponents)
                {
                    if (name.Equals("Root"))
                    {
                        Console.WriteLine("Num ticky components in frame " + totalFrames + ": " + NumTickyComponents);
                        ShowedCountOfTickyComponents = true;
                    }
                }

                if (_numFrame > 500)
                {
                    _numFrame = 0;
                    ShowedCountOfTickyComponents = false;
                }

                _numFrame++;
                totalFrames++;
            }

            private bool CanUpdateChild(int childIndex)
            {
                // update when frame delay has passed
                return numFramesDelayLeft == 0
                    // or update entities when is visible
                    || Mario.IsInViewRect((Entity)_children[childIndex]);
            }

            public IEntity[] GetChildren()
            {
                return _children.ToArray();
            }

            public void ReceiveSerialize()
            {
                List<EntitySerializableProxy> entities = new List<EntitySerializableProxy>
                {
                    new EntitySerializableProxy(this)
                };

                foreach (var child in _children)
                {
                    entities.Add(new EntitySerializableProxy(child));
                }

                File.WriteAllText("Scene.json", JsonConvert.SerializeObject(entities));
            }

            public void ReceiveLoad()
            {
                List<EntitySerializableProxy> entities = JsonConvert.DeserializeObject<List<EntitySerializableProxy>>(File.ReadAllText("Scene.json"));

                var self = entities.FirstOrDefault(e => e.EntityName.Equals("Root"));
                entities.Remove(self);
                _posX = self.PositionX;
                _posY = self.PositionY;
                width = self.Width;
                height = self.Height;
                isActive = self.IsActive;

                AddComponentsFromProxy(self);

                foreach (var entity in entities)
                {
                    Entity e = CreateEntity(entity.EntityName);

                    e._posX = entity.PositionX;
                    e._posY = entity.PositionY;
                    e.width = entity.Width;
                    e.height = entity.Height;
                    e.isActive = entity.IsActive;
                    e.AddComponentsFromProxy(entity);
                }

                SDL_QueryTexture(SDLApp.backgroundTexture, out uint format, out int access, out int w, out int h);
                SDLRendering.SetWorldBounds(w, h);
            }
        }

        public void AddComponentsFromProxy(EntitySerializableProxy proxy)
        {
            foreach (var component in proxy.ComponentProxies)
            {
                Type type = Type.GetType(component.TypeName);

                if (!HasComponent(type))
                {
                    ConstructorInfo info = type.GetConstructors().Where(constructor => constructor.GetParameters().Length == 0).FirstOrDefault();

                    if (info == null)
                    {
                        throw new ArgumentException(type.Name + " doesn't contain default constructor");
                    }

                    Component c = (Component)info.Invoke(null);
                    AddInDirectComponent(c, type);
                    c.OnDeserialize(component.KeyValues);
                }
                else
                {
                    Component c = GetComponent(type);
                    c.OnDeserialize(component.KeyValues);
                }
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
