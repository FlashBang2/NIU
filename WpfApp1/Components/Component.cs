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
        private bool _shouldTick = false;

        public bool shouldTick { get => _shouldTick; set { SetSimulateTick(value); } }
        
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

        public virtual void OnTick(float deltaTime)
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

        public void SetSimulateTick(bool shouldSimulateTick)
        {
            if (shouldSimulateTick)
            {
                if (shouldSimulateTick != _shouldTick)
                {
                    Activate();   
                }
            }
            else
            {
                if (shouldSimulateTick != _shouldTick)
                {
                    Deactivate();
                }
            }
        }

        public void Activate()
        {
            if (!_shouldTick)
            {
                Activated();
                owner.AddToTickList(this);
            }

            _shouldTick = true;
        }

        public void Deactivate()
        {
            if (_shouldTick)
            {
                Deactivated();
                owner.RemoveFromTickList(this);
            }

            _shouldTick = false;
        }
    }
}
