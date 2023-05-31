using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class CharacterMovementComponent : Component
    {
        public Vector Velocity = new Vector();
        public float GravityScale = 0.1f;
        public bool IsControlledByPlayer = false;

        public float AccumulatedY = 0;

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

            AccumulatedY += (float) Velocity.Y;

            if (collision.IsOverlaping)
            {
                AccumulatedY -= (float) Velocity.Y;
                Velocity.Y = 0;
            }
        }

        public override void Deactivated()
        {
            base.Deactivated();
            Console.WriteLine("Out of bounds maybe ?");
        }
    }
}
