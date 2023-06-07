﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    public class CharacterMovementComponent : Component
    {
        public Vector Velocity = new Vector();
        public float GravityScale = 6.0f;
        public bool IsControlledByPlayer = false;

        public float AccumulatedY = 0;

        public bool IsFalling { get; private set; }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            ApplyMovement(dt);
        }

        private void ApplyMovement(float dt)
        {
            if (Owner.HasComponent<SkeletonComponent>())
            {
                if (Owner.GetComponent<SkeletonComponent>().State != SkeletonComponentState.GameRunning)
                {
                    return;
                }
            }

            Owner.AddWorldOffset(Velocity.X, Velocity.Y);
            Velocity.Y += 9.81 * GravityScale * dt;
            CollisionComponent collision = Owner.GetComponent<CollisionComponent>();
            collision.TestCollision();

            AccumulatedY += (float)Velocity.Y * dt;

            if (collision.IsOverlaping)
            {
                AccumulatedY -= (float)Velocity.Y * dt;
                Velocity.Y = 0;
            }

            Ray ray = new Ray();
            List<IEntity> ignore = new List<IEntity>
            {
                Owner
            };

            ray.Init(new Vector(Owner.PosX + Owner.Width / 2, Owner.PosY + Owner.Height), new Vector(Owner.PosX + Owner.Width / 2, Owner.PosY + Owner.Height) + 10 * Owner.Down);
            IsFalling = !CollisionComponent.RayCast(ray, ignore, out _);
        }

        public override void Deactivated()
        {
            base.Deactivated();
            Console.WriteLine("Out of bounds maybe ?");
        }
    }
}