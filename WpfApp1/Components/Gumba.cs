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
        private float _directionScale = -1.0f;
        private bool _isKilled = false;
        private CharacterMovementComponent _movementComponent;
        
        private const int Speed = 3;
        private Ray _ray = new Ray();
        private IList<IEntity> _ignoreSelfList = new List<IEntity>();

        private Sprite _sprite;

        public override void Spawned()
        {
            base.Spawned();
            shouldTick = true;
            _ignoreSelfList.Add(owner);
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);

            if (_movementComponent == null)
            {
                _movementComponent = owner.GetComponent<CharacterMovementComponent>();
                _sprite = owner.GetComponent<Sprite>();
            }

            if (!_movementComponent.isFalling && _sprite.shouldMove)
            {
                DoGumbaThink();
            }
        }

        private void DoGumbaThink()
        {
            owner.AddWorldOffset(Speed * _directionScale, 0);

            _ray.Init(new Vector(owner.posX, owner.posY), new Vector(owner.posX + owner.width * _directionScale + Speed * _directionScale, owner.posY));

            if (RayCast(ref _ray, _ignoreSelfList, out OverlapEvent evt))
            {
                ReactToEntity(evt);
            }

            _ray.Init(new Vector(owner.posX + owner.width * _directionScale + Speed * _directionScale, owner.posY + owner.height),
                new Vector(owner.posX + owner.width * _directionScale + Speed * _directionScale, owner.posY + owner.height + 40));

            bool isObstacleInFront = !RayCast(ref _ray, _ignoreSelfList, out _);
            if (isObstacleInFront)
            {
                _directionScale *= -1;
            }
        }

        private void ReactToEntity(OverlapEvent evt)
        {
            bool isReactedWithMario = evt.lastContact.name.Equals("mario");

            if (isReactedWithMario) //reaction to other stuff
            {
                Entity mario = Entity.GetEntity("mario", true);
                mario.posX = 144;

                if (mario.posY < owner.posY)
                {
                    owner.GetComponent<Sprite>().shouldMove = false;
                    owner.SetActive(false);
                }

                if (!_isKilled)
                {
                    mario.posY = SDLApp.GetInstance().GetAppHeight() - 144;
                }
            }
            else
            {
                // other obstacle
                _directionScale *= -1;
            }
        }
    }
}
