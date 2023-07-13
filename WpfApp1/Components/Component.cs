using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Component
    {
        public IEntity owner { get; private set; }
        private bool _isActive = false;

        public bool isActive { get => _isActive; set { SetActive(value); } }
        
        public Component()
        {
        }

        public virtual void Spawned()
        {
        }

        // should return true, if object will handle respawn
        public virtual bool Destroyed()
        {
            return false;
        }

        public void OnComponentRegistered(IEntity owner)
        {
            this.owner = owner;
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

        public void SetActive(bool b)
        {
            if (b)
            {
                if (b != _isActive)
                {
                    Activate();   
                }
            }
            else
            {
                if (b != _isActive)
                {
                    Deactivate();
                }
            }
        }

        public void Activate()
        {
            if (!_isActive)
            {
                Activated();
                owner.AddToTickList(this);
            }

            _isActive = true;
        }

        public void Deactivate()
        {
            if (_isActive)
            {
                Deactivated();
                owner.RemoveFromTickList(this);
            }

            _isActive = false;
        }
    }
}
