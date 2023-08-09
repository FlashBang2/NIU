﻿using System;
using System.Collections.Generic;
using System.Windows;
using static WpfApp1.CollisionComponent;

namespace WpfApp1
{
    public class Gumba : Component
    {
        private float _directionScale = -1.0f;
        private bool _isKilled = false;
        private CharacterMovementComponent _movementComponent;

        private const int Speed = 120;
        private Ray _ray = new Ray();
        private IList<IEntity> _ignoreSelfList = new List<IEntity>();

        private Sprite _sprite;
        private IEntity _mario;

        public override void Spawned()
        {
            base.Spawned();
            shouldTick = true;
            _ignoreSelfList.Add(owner);
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);

            if (_mario == null)
            {
                _mario = Entity.rootEntity.FindChild("mario");
            }

            if (_movementComponent == null)
            {
                _movementComponent = owner.GetComponent<CharacterMovementComponent>();
                _sprite = owner.GetComponent<Sprite>();
            }

            _sprite.shouldMove = Math.Abs(owner.posX - _mario.posX) < 500 && SDLApp.GetInstance().canStartGoomba;

            if (!_movementComponent.isFalling && _sprite.shouldMove)
            {
                DoGumbaThink(deltaTime);
            }
        }

        private void DoGumbaThink(float deltaTime)
        {
            _movementComponent.velocity.X = Speed * _directionScale * deltaTime;

            _ray.Init(new Vector(owner.posX, owner.posY), new Vector(owner.posX + owner.width * _directionScale + 3 * _directionScale, owner.posY));

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

        public override void OnSerialize(Dictionary<string, Tuple<int, string, bool>> keyValues)
        {
            base.OnSerialize(keyValues);
            keyValues.Add("directionScale", new Tuple<int, string, bool>(0, _directionScale.ToString(), false));
            keyValues.Add("isKilled", new Tuple<int, string, bool>(0, string.Empty, _isKilled));
        }

        public override void OnDeserialize(Dictionary<string, Tuple<int, string, bool>> keyValues)
        {
            base.OnDeserialize(keyValues);
            _directionScale = float.Parse(keyValues["directionScale"].Item2);
            _isKilled = keyValues["isKilled"].Item3;
        }
    }
}
