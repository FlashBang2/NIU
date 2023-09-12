using System;
using System.Collections.Generic;

namespace WpfApp1.Components
{
    public class ComponentSerializableProxy
    {
        public string TypeName { get; set; }
        public Dictionary<string, Tuple<int, string, bool>> KeyValues { get; set; } = new Dictionary<string, Tuple<int, string, bool>>();

        public ComponentSerializableProxy(Component component)
        {
            TypeName = component.GetType().AssemblyQualifiedName;
            component.OnSerialize(KeyValues);
        }

        public ComponentSerializableProxy()
        {
        }
    }

    public class EntitySerializableProxy
    {
        public string EntityName { get; set; }
        public float PositionX { get; set; } = 0;
        public float PositionY { get; set; } = 0;

        public float Width { get; set; } = 0;
        public float Height { get; set; } = 0;
        public bool IsActive { get; set; } = false;

        public float LastX { get; set; } = 0;
        public float LastY { get; set; } = 0;

        public List<ComponentSerializableProxy> ComponentProxies { get; set; } = new List<ComponentSerializableProxy>();

        public EntitySerializableProxy() { }
        public EntitySerializableProxy(IEntity entity)
        {
            EntityName = entity.name;
            PositionX = entity.posX;
            PositionY = entity.posY;
            Width = entity.width;
            Height = entity.height;
            IsActive = entity.isActive;

            List<Component> components = entity.GetAllComponents();

            foreach (var component in components)
            {
                ComponentSerializableProxy proxy = new ComponentSerializableProxy(component);
                ComponentProxies.Add(proxy);
            }
        }
    }
}
