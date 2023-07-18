using System;
using System.Collections.Generic;

namespace WpfApp1.Components
{
    public class FlagComponent : Component
    {
        private Entity _mario;

        public override void Spawned()
        {
            base.Spawned();
            shouldTick = true;
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);

            _mario = Entity.GetEntity("mario", true);
            owner.GetComponent<CollisionComponent>().onOverlaped += OnMarioInsideFlag;
            shouldTick = false;
        }

        private void OnMarioInsideFlag(CollisionComponent.OverlapEvent obj)
        {
            if (obj.lastContact.name.Equals("mario"))
            {
                _mario.Destroy();
            }
        }

        public override void OnSerialize(Dictionary<string, Tuple<int, string, bool>> keyValues)
        {
            // no saving has ticked
        }

        public override void OnDeserialize(Dictionary<string, Tuple<int, string, bool>> keyValues)
        {
            // no loading has ticked
        }
    }
}
