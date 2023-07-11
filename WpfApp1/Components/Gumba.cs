using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WpfApp1.CollisionComponent;
using System.Windows;

namespace WpfApp1
{
    public class Gumba : Component
    {
        private float directionScale = -1.0f;
        private bool killed = false;
        private CharacterMovementComponent movementComponent;
        
        private const int Speed = 3;
        private Ray ray = new Ray();
        private List<IEntity> ignoredEntities = new List<IEntity>();

        private Sprite sprite;

        public override void Spawned()
        {
            base.Spawned();
            isActive = true;
            ignoredEntities.Add(Owner);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (movementComponent == null)
            {
                movementComponent = Owner.GetComponent<CharacterMovementComponent>();
                sprite = Owner.GetComponent<Sprite>();
            }

            if (!movementComponent.IsFalling && sprite.shouldMove)
            {
                Owner.AddWorldOffset(Speed * directionScale, 0);

                ray.Init(new Vector(Owner.PosX, Owner.PosY), new Vector(Owner.PosX + Owner.Width * directionScale + Speed * directionScale, Owner.PosY));

                OverlapEvent evt;
                if (RayCast(ray, ignoredEntities, out evt))
                {
                    ReactToEntity(evt);
                }

                ray.Init(new Vector(Owner.PosX + Owner.Width * directionScale + Speed * directionScale, Owner.PosY + Owner.Height), 
                    new Vector(Owner.PosX + Owner.Width * directionScale + Speed * directionScale, Owner.PosY + Owner.Height + 40));

                bool isObstacleInFront = !RayCast(ray, ignoredEntities, out evt);
                if (isObstacleInFront)
                {
                    directionScale *= -1;
                }
            }
        }

        private void ReactToEntity(OverlapEvent evt)
        {
            bool isReactedWithMario = evt.LastContact.Name.Equals("mario");
            if (isReactedWithMario) //reaction to other stuff
            {
                Entity mario = Entity.GetEntity("mario", true);
                mario.PosX = 144;

                if (mario.PosY < Owner.PosY)
                {
                    Owner.GetComponent<Sprite>().shouldMove = false;
                    Owner.SetActive(false);
                }

                if (!killed)
                {
                    mario.PosY = SDLApp.GetInstance().GetAppHeight() - 144;
                }
            }
            else // reaction to mario
            {
                directionScale *= -1;
            }
        }
    }
}
