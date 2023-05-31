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
        public float GravityScale = 0.1f;
        public bool IsControlledByPlayer = false;

        public float AccumulatedY = 0;

        public bool IsFalling { get; private set; }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            ApplyMovement();
        }

        private void ApplyMovement()
        {
            Owner.AddWorldOffset(Velocity.X, Velocity.Y);
            Velocity.Y += 9.81 * GravityScale;
            CollisionComponent collision = Owner.GetComponent<CollisionComponent>();
            collision.TestCollision();

            AccumulatedY += (float)Velocity.Y;

            if (collision.IsOverlaping)
            {
                AccumulatedY -= (float)Velocity.Y;
                Velocity.Y = 0;
            }

            Ray ray = new Ray();
            List<IEntity> ignore = new List<IEntity>
            {
                Owner
            };

            ray.Init(new Vector(Owner.PosX + Owner.Width / 2, Owner.PosY + Owner.Height), new Vector(Owner.PosX + Owner.Width / 2, Owner.PosY + Owner.Height) + 10 * Owner.Down);
            CollisionComponent.OverlapEvent overlap;
            IsFalling = !CollisionComponent.RayCast(ray, ignore, out overlap);
            SDLRendering.DrawLine(ray.Start, overlap.ContactPoint, Color.FromRgb(20, 80, 123));
        }

        public override void Deactivated()
        {
            base.Deactivated();
            Console.WriteLine("Out of bounds maybe ?");
        }
    }
}
