using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using static SDL2.SDL;

namespace WpfApp1
{
    public class CharacterMovementComponent : Component, IRenderable
    {
        public Vector Velocity = new Vector();
        public float GravityScale = 6.0f;
        public bool IsControlledByPlayer = false;

        public float AccumulatedY = 0;

        public bool IsFalling { get; private set; }

        public bool ShouldDraw => true;

        public override void Spawned()
        {
            base.Spawned();
            isActive = true;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            ApplyMovement(dt);
        }
        Ray ray = new Ray();
        private void ApplyMovement(float dt)
        {
            if (Owner.HasComponent<SkeletonComponent>())
            {
                if (Owner.GetComponent<SkeletonComponent>().State != SkeletonComponentState.GameRunning)
                {
                    return;
                }
            }

            Owner.AddWorldOffset((float)Velocity.X, (float)Velocity.Y);
            Velocity.Y += 9.81 * GravityScale * dt;
            CollisionComponent collision = Owner.GetComponent<CollisionComponent>();
            collision.TestCollision();

            AccumulatedY += (float)Velocity.Y * dt;

            if (collision.IsOverlaping)
            {
                AccumulatedY -= (float)Velocity.Y * dt;
                Velocity.Y = 0;
            }

            
            List<IEntity> ignore = new List<IEntity>
            {
                Owner
            };

            ray.Init(new Vector(Owner.PosX + Owner.Width / 2, Owner.PosY + Owner.Height), new Vector(Owner.PosX + Owner.Width / 2, Owner.PosY + Owner.Height + 30));

            
            IsFalling = !CollisionComponent.RayCast(ray, ignore, out _);
        }

        public override void ReceiveRender()
        {
            base.ReceiveRender();
            SDLRendering.DrawLine(ray.Start, ray.End, Color.FromRgb(255, 255, 255));
        }

        public override void Deactivated()
        {
            base.Deactivated();
            Console.WriteLine("Out of bounds maybe ?");
        }
    }
}
