using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using static SDL2.SDL;

namespace WpfApp1
{
    public class CharacterMovementComponent : Component
    {
        public Vector velocity = new Vector();
        public float gravityScale = 6.0f;
        public bool isControlledByPlayer = false;

        public float yTotalVelocity = 0;
        public bool isFalling { get; private set; }

        private CollisionComponent _collision = null;
        private SkeletonComponent _skeleton = null;
        private Ray _ray = new Ray();
        
        private IList<IEntity> _rayCastIgnoreSelf = new List<IEntity>();

        public override void Spawned()
        {
            base.Spawned();
            shouldTick = true;
            _rayCastIgnoreSelf.Add(owner);
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);

            ApplyMovement(deltaTime);
        }
        private void ApplyMovement(float deltaTime)
        {
            if (owner.HasComponent<SkeletonComponent>())
            {
                if (_skeleton == null)
                {
                    _skeleton = owner.GetComponent<SkeletonComponent>();
                }

                if (_skeleton.state != SkeletonComponentState.GameRunning)
                {
                    return;
                }
            }

            
            owner.AddWorldOffset((float)velocity.X, (float)velocity.Y);
            velocity.Y += 9.81 * gravityScale * deltaTime;

            if (_collision == null)
            {
                _collision = owner.GetComponent<CollisionComponent>();
            }

            _collision.TestCollision();

            yTotalVelocity += (float)velocity.Y * deltaTime;

            if (_collision.hasOverlapedInLastFrame)
            {
                yTotalVelocity -= (float)velocity.Y * deltaTime;
                velocity.Y = 0;
            }

            _ray.Init(new Vector(owner.posX + owner.width / 2, owner.posY + owner.height), new Vector(owner.posX + owner.width / 2, owner.posY + owner.height + 30));
            isFalling = !CollisionComponent.RayCast(ref _ray, _rayCastIgnoreSelf, out _);
        }

        public override void Deactivated()
        {
            base.Deactivated();
            Console.WriteLine("Out of bounds maybe ?");
        }
    }
}
