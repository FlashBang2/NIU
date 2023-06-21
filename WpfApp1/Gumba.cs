using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WpfApp1.CollisionComponent;

namespace WpfApp1
{
    public class Gumba : Component
    {
        private const int Speed = 5;
        float directionScale = -1.0f;

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            List<IEntity> e = new List<IEntity>();
            e.Add(Owner);

            if (!Owner.GetComponent<CharacterMovementComponent>().IsFalling)
            {
                Owner.AddWorldOffset(Speed * directionScale, 0);

                Ray ray = new Ray();
                ray.Init(new System.Windows.Vector(Owner.PosX, Owner.PosY), new System.Windows.Vector(Owner.PosX + Owner.Width * directionScale + Speed * directionScale, Owner.PosY));

                OverlapEvent evt;
                if (RayCast(ray, e, out evt))
                {
                    if (!evt.LastContact.Name.Equals("mario"))
                    {
                        directionScale *= -1;
                    }
                    else
                    {
                        // reaction to mario
                    }
                }

                ray.Init(new System.Windows.Vector(Owner.PosX + Owner.Width * directionScale + Speed * directionScale, Owner.PosY + Owner.Height), new System.Windows.Vector(Owner.PosX + Owner.Width * directionScale + Speed * directionScale, Owner.PosY + Owner.Height + 40));

                if (!RayCast(ray, e, out evt))
                {
                    directionScale *= -1;
                }
            }
        }
    }
}
