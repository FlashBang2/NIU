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
        private const int Speed = 3;
        float directionScale = -1.0f;

        private bool killed = false;
        private CharacterMovementComponent MovementComponent;

        Ray ray = new Ray();

        public override void Spawned()
        {
            base.Spawned();
            isActive = true;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            List<IEntity> e = new List<IEntity>
            {
                Owner
            };

            if (MovementComponent == null)
            {
                MovementComponent = Owner.GetComponent<CharacterMovementComponent>();
            }

            if (!MovementComponent.IsFalling && Owner.GetComponent<Sprite>().shouldMove)
            {
                Owner.AddWorldOffset(Speed * directionScale, 0);

                ray.Init(new System.Windows.Vector(Owner.PosX, Owner.PosY), new System.Windows.Vector(Owner.PosX + Owner.Width * directionScale + Speed * directionScale, Owner.PosY));

                OverlapEvent evt;
                if (RayCast(ray, e, out evt))
                {
                    if (!evt.LastContact.Name.Equals("mario")) //reaction to other stuff
                    {
                        directionScale *= -1;
                    }
                    else // reaction to mario
                    {
                        Entity mario = Entity.GetEntity("mario", true);
                        mario.PosX = 144;
                        if (mario.PosY < Owner.PosY)
                        {
                            Owner.GetComponent<Sprite>().shouldMove = false;
                            Owner.SetActive(false);
                        }
                        if (!killed)
                            mario.PosY = SDLApp.GetInstance().GetAppHeight() - 144;
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
