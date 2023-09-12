using System;
using System.Collections.Generic;
using System.Windows;

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
        private bool _isSkeletonComponentCached = false;

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

            Move(deltaTime);
        }
        private void Move(float deltaTime)
        {
            if (!_isSkeletonComponentCached)
            {
                if (owner.HasComponent<SkeletonComponent>())
                {
                    _skeleton = owner.GetComponent<SkeletonComponent>();
                    if (_skeleton.state != SkeletonComponentState.GameRunning)
                    {
                        return;
                    }
                }

                _isSkeletonComponentCached = true;
            }

            ApplyMovement(deltaTime);

            // check for up and down collision only when falling
            _collision.verticalCollisionEnabled = isFalling;
        }

        private void ApplyMovement(float deltaTime)
        {
            owner.AddWorldOffset((float)velocity.X, (float)velocity.Y);

            if (_collision == null)
            {
                _collision = owner.GetComponent<CollisionComponent>();
            }

            _collision.TestCollision();

            velocity.Y += 9.81 * gravityScale * deltaTime;
            yTotalVelocity += (float)velocity.Y * deltaTime;

            _ray.Init(new Vector(owner.posX + owner.width / 2, owner.posY + owner.height), new Vector(owner.posX + owner.width / 2, owner.posY + owner.height + 2));
            isFalling = !CollisionComponent.RayCast(ref _ray, _rayCastIgnoreSelf, out _);

            if (!isFalling && velocity.Y > 0)
            {
                owner.AddWorldOffset(0, -(float)velocity.Y);
                velocity.Y = 0;
            }
        }

        public override void Deactivated()
        {
            base.Deactivated();
            Console.WriteLine("Out of bounds maybe ?");
        }

        public override void OnSerialize(Dictionary<string, Tuple<int, string, bool>> keyValues)
        {
            base.OnSerialize(keyValues);
            keyValues.Add("IsFalling", new Tuple<int, string, bool>(0, string.Empty, isFalling));
        }

        public override void OnDeserialize(Dictionary<string, Tuple<int, string, bool>> keyValues)
        {
            base.OnDeserialize(keyValues);
            isFalling = keyValues["IsFalling"].Item3;
        }
    }
}
