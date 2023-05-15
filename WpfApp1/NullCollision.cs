using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace WpfApp1
{
    public class NullCollision : ICollideable
    {
        public static readonly int NoCollisionGroup = -1;

        private IEntity _entity;
        private int _collisionGroup;

        public NullCollision(IEntity entity, int collisionGroup)
        {
            _entity = entity;
            CollisionGroup = collisionGroup;
        }   

        public object LinkedEntity => _entity;

        public Rect Bounds => _entity.Bounds;

        public int CollisionGroup { get => _collisionGroup; set => _collisionGroup = value; }

        public bool TestCollision(Ray ray, uint mask)
        {
            return false;
        }
    }
}
