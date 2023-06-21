using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Component
    {
        public IEntity Owner { get; private set; }

        private bool _isActive = true;

        public Component()
        {
        }

        public virtual void Spawned()
        {
        }

        public virtual bool Destroyed()
        {
            return false;
        }

        public void OnComponentRegistered(IEntity owner)
        {
            Owner = owner;
        }

        public virtual void OnTick(float dt)
        {
        }

        public virtual void Activated()
        {
        }

        public virtual void ReceiveRender()
        {
        }

        public virtual void Deactivated()
        {
        }

        public void Activate()
        {
            if (!_isActive)
            {
                Activated();
            }

            _isActive = true;
        }

        public void Deactivate()
        {
            if (_isActive)
            {
                Deactivated();
            }

            _isActive = false;
        }
    }
}
