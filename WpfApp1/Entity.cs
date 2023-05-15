using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class Entity : IEntity
    {
        private IRenderable _renderable;
        private ICollideable _collideable;
        private IEntity _parent;
        private double _posX;
        private double _posY;
        private string _name;
        private bool _active = true;

        private double _angle = 0;

        private List<IEntity> _children = new List<IEntity>();

        public Entity(string name)
        {
            _renderable = new NullRenderable(this);
            _parent = null;
            _name = name;
            _collideable = new NullCollision(this, NullCollision.NoCollisionGroup);
        }

        public IRenderable Renderable
        {
            get => _renderable; set
            {
                SDLRendering.RemoveRenderable(_renderable);
                _renderable = value;

                if (!(_renderable is NullRenderable))
                {
                    SDLRendering.AddRenderable(_renderable);
                }
            }
        }

        public IEntity Parent
        {
            get => _parent; set
            {
                _parent?.RemoveChild(this);

                _parent = value;
                _parent.AttachChild(this);
            }
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
            }
        }

        public double Width { get => Bounds.Width; set => _renderable.Bounds = Rect.FromOriginAndExtend(Bounds.Center, new Vector(value, Bounds.Height)); }
        public string Name { get => _name; set => _name = value; }
        public double Height { get => Bounds.Height; set => _renderable.Bounds = Rect.FromOriginAndExtend(Bounds.Center, new Vector(Bounds.Width, value)); }

        public Vector Right => new Vector(Math.Cos(_angle / 180.0 * Math.PI), Math.Sin(_angle / 180.0f * Math.PI));

        public Vector Left => -Right;

        public Vector Up => new Vector(-Math.Sin(_angle / 180.0 * Math.PI), Math.Cos(_angle / 180.0 * Math.PI));
        public Vector Down => -Up;

        public bool IsActive { get => _active; set => _active = value; }

        public Rect Bounds { get => _renderable.Bounds; }
        public ICollideable Collideable
        {
            get => _collideable; set
            {
                Physics.RemovePhysicsBody(_collideable);
                _collideable = value;

                if (!(_collideable is NullCollision))
                {
                    Physics.AddPhysicsBody(_collideable);
                }
            }
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

            if (!Physics.TestRay(ray, _collideable))
            {
                PosX += x;
                PosY += y;
                
                foreach (var child in _children)
                {
                    child.AddWorldOffset(x, y);
                }
            }
        }

        public void AttachChild(IEntity entity)
        {
            entity.Parent = this;
            _children.Add(entity);
        }


        public void Destroy()
        {
            foreach (var child in _children)
            {
                child.Destroy();
            }

            _children.Clear();
            Parent = null;
            Destroyed();
        }

        public virtual void Destroyed()
        {
        }

        public IEntity FindChild(string name)
        {
            return _children.Find(child => child.Name.Equals(name));
        }

        public bool RemoveChild(IEntity entity)
        {
            return _children.Remove(entity);
        }

        public void Rotate(double angle)
        {

            Ray ray = new Ray();




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
            foreach (var child in _children)
            {
                child.Tick(dt);
            }
        }
    }
}
