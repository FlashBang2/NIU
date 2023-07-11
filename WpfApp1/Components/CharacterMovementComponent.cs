using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using static SDL2.SDL;

namespace WpfApp1
{
    public class CharacterMovementComponent : Component
    {
        public Vector Velocity = new Vector();
        public float GravityScale = 6.0f;
        public bool IsControlledByPlayer = false;

        public float AccumulatedY = 0;
        public CollisionComponent collision = null;
        public SkeletonComponent skeleton = null;

        public bool IsFalling { get; private set; }
        Ray ray = new Ray();
        List<IEntity> ignore = new List<IEntity>();

        public override void Spawned()
        {
            base.Spawned();
            isActive = true;
            ignore.Add(Owner);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            ApplyMovement(dt);
        }
        private void ApplyMovement(float dt)
        {
            if (Owner.HasComponent<SkeletonComponent>())
            {
                if (skeleton == null)
                {
                    skeleton = Owner.GetComponent<SkeletonComponent>();
                }

                if (skeleton.State != SkeletonComponentState.GameRunning)
                {
                    return;
                }
            }

            Owner.AddWorldOffset((float)Velocity.X, (float)Velocity.Y);
            Velocity.Y += 9.81 * GravityScale * dt;

            if (collision == null)
            {
                collision = Owner.GetComponent<CollisionComponent>();
            }

            collision.TestCollision();

            AccumulatedY += (float)Velocity.Y * dt;

            if (collision.IsOverlaping)
            {
                AccumulatedY -= (float)Velocity.Y * dt;
                Velocity.Y = 0;
            }

            ray.Init(new Vector(Owner.PosX + Owner.Width / 2, Owner.PosY + Owner.Height), new Vector(Owner.PosX + Owner.Width / 2, Owner.PosY + Owner.Height + 30));
            IsFalling = !CollisionComponent.RayCast(ray, ignore, out _);
        }

        public override void Deactivated()
        {
            base.Deactivated();
            Console.WriteLine("Out of bounds maybe ?");
        }
    }
}
