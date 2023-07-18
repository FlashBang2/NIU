using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static SDL2.SDL;

namespace WpfApp1
{
    public class CollisionComponent : Component
    {
        public struct OverlapEvent
        {
            public IEntity lastContact;
            public IEntity self;
            public float contactPointX;
            public float contactPointY;

            public static OverlapEvent Empty = new OverlapEvent();
        }

        public bool isStatic
        {
            get => !shouldTick;
            set => shouldTick = !value;
        }

        public bool hasOverlapedInLastFrame = false;
        public bool shouldDraw => true;
        public bool lastHitWasVertical = false;

        public bool verticalCollisionEnabled = false;
        public bool horizontalCollisionEnabled = true;

        public event Action<OverlapEvent> onOverlaped;
        public event Action<OverlapEvent> onStopsOverlaping;

        private IList<IEntity> _overlapContacts = new List<IEntity>();
        private static readonly IList<CollisionComponent> _collisionComponents = new List<CollisionComponent>();
        private static readonly IList<IEntity> _emptyEntitiesList = new List<IEntity>();

        private bool _isTrigger = false;
        private CharacterMovementComponent _movementComponent = null;
        private bool _isMovementComponentCached = false;

        private bool _isFirstFrameEnded = true;
        private OverlapEvent _overlapEvent;
        private Ray _ray;
        private IList<IEntity> _ignoredSelf;

        public static bool RayCast(ref Ray ray, out OverlapEvent outOverlapEvent)
        {
            return RayCast(ref ray, _emptyEntitiesList, out outOverlapEvent);
        }

        public override void Activated()
        {
            base.Activated();
            _collisionComponents.Add(this);
        }

        public override void Deactivated()
        {
            base.Deactivated();
            _collisionComponents.Remove(this);
        }

        public override bool Destroyed()
        {
            base.Deactivated();
            _collisionComponents.Remove(this);
            return false;
        }

        public static bool RayCast(ref Ray ray, IList<IEntity> ignoredEntities, out OverlapEvent outOverlapEvent)
        {
            double time = 0.00;

            if (TestPointInAnyComponent(ignoredEntities, ref ray.start, out outOverlapEvent))
            {
                return true;
            }

            if (TestPointInAnyComponent(ignoredEntities, ref ray.end, out outOverlapEvent))
            {
                return true;
            }

            while (time < 1)
            {
                Vector current = (1 - time) * ray.start + time * ray.end;

                if (TestPointInAnyComponent(ignoredEntities, ref current, out outOverlapEvent))
                {
                    return true;
                }

                time += 0.1;
            }

            outOverlapEvent = new OverlapEvent();
            return false;
        }

        static bool TestPointInAnyComponent(IList<IEntity> ignoredEntities, ref Vector point, out OverlapEvent outOverlapEvent)
        {
            foreach (var collisionComponent in _collisionComponents)
            {
                if (collisionComponent.owner.isActive && ignoredEntities.FirstOrDefault(e => e == collisionComponent.owner) == null)
                {
                    var bounds = collisionComponent.owner.bounds;

                    if (SdlRectMath.IsPointInRect(ref bounds, (float)point.X, (float)point.Y))
                    {
                        outOverlapEvent.self = null;
                        outOverlapEvent.lastContact = collisionComponent.owner;
                        outOverlapEvent.contactPointX = (float)point.X;
                        outOverlapEvent.contactPointY = (float)point.Y;
                        return true;
                    }
                }

            }

            outOverlapEvent = new OverlapEvent();
            return false;
        }

        public override void Spawned()
        {
            base.Spawned();
            _collisionComponents.Add(this);
            isStatic = true;
            _overlapEvent = new OverlapEvent
            {
                self = owner
            };

            _ray = new Ray();
            _ignoredSelf = new List<IEntity>
            {
                owner
            };
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            if (_isFirstFrameEnded)
            {
                _isFirstFrameEnded = false;
                return;
            }

            if (!_isMovementComponentCached)
            {
                if (owner.HasComponent<CharacterMovementComponent>())
                {
                    _movementComponent = owner.GetComponent<CharacterMovementComponent>();
                }

                _isMovementComponentCached = true;
            }

            TestCollision();
        }

        public void TestCollision()
        {
            SDL_Rect ownerBounds = owner.bounds;
            hasOverlapedInLastFrame = false;

            foreach (var collisionComponent in _collisionComponents)
            {
                if (collisionComponent != this && collisionComponent.owner.isActive)
                {
                    if (_isTrigger)
                    {
                        TestForTrigger(ref ownerBounds, collisionComponent);
                        continue;
                    }

                    TestForCollision(ref ownerBounds, collisionComponent);
                }
            }
        }

        private void TestForCollision(ref SDL_Rect ownerBounds, CollisionComponent collisionComponent)
        {
            SDL_Rect childBounds = collisionComponent.owner.bounds;

            if (collisionComponent._isTrigger)
            {
                return;
            }

            if (SDL_IntersectRect(ref childBounds, ref ownerBounds, out SdlRectMath.DummyEndResult) == SDL_bool.SDL_TRUE)
            {
                // SdlRectMath.DummyEndResult contains overlap rect
                if (IsCollisionImpactFromLeftOrRight(ref SdlRectMath.DummyEndResult))
                {
                    if (horizontalCollisionEnabled)
                    {
                        CorrectHorizontalLocation(collisionComponent, ref SdlRectMath.DummyEndResult);
                    }
                }
                else
                {
                    if (verticalCollisionEnabled)
                    {
                        CorrectVerticalLocation(ref SdlRectMath.DummyEndResult);
                    }
                }

                _overlapEvent.lastContact = collisionComponent.owner;

                onOverlaped?.Invoke(_overlapEvent);
                _overlapContacts.Add(collisionComponent.owner);
            }
        }

        private static bool IsCollisionImpactFromLeftOrRight(ref SDL_Rect overlapRect)
        {
            return Math.Abs(overlapRect.w) < Math.Abs(overlapRect.h);
        }

        public void MarkItAsTrigger()
        {
            _isTrigger = true;
            shouldTick = true;
        }

        private void CorrectVerticalLocation(ref SDL_Rect overlapRect)
        {
            if (_movementComponent != null)
            {
                PushOutOfWall(ref overlapRect);
            }
            else
            {
                owner.AddWorldOffset(0, -(overlapRect.h + 1));
                hasOverlapedInLastFrame = true;
            }
        }

        private void PushOutOfWall(ref SDL_Rect overlapRect)
        {
            _ray.Init(new Vector(owner.posX + owner.width / 2, owner.posY + owner.height), new Vector(owner.posX + owner.width / 2, owner.posY + owner.height + 30));
            lastHitWasVertical = false;

            // check has hit ground
            if (RayCast(ref _ray, _ignoredSelf, out OverlapEvent.Empty))
            {
                lastHitWasVertical = true;
                return;
            }

            // test has hit ceiling
            if (_movementComponent.velocity.Y < 0)
            {
                _movementComponent.velocity.Y = 0;
                owner.AddWorldOffset(0, (overlapRect.h + 1));
                hasOverlapedInLastFrame = true;
            }
        }

        private void CorrectHorizontalLocation(CollisionComponent collisionComponent, ref SDL_Rect overlapRect)
        {
            int directionOfHit = Math.Sign(owner.posX - collisionComponent.owner.posX);

            owner.AddWorldOffset(directionOfHit * (overlapRect.w + 1), 0);
            hasOverlapedInLastFrame = true;
        }

        private void TestForTrigger(ref SDL_Rect ownerBounds, CollisionComponent collisionComponent)
        {
            SDL_Rect childBounds = collisionComponent.owner.bounds;

            if (_overlapContacts.Contains(collisionComponent.owner))
            {
                TestIsStillOverlapping(ref ownerBounds, collisionComponent, ref childBounds);
            }
            else
            {
                TestIsOverlappingThisComponent(ref ownerBounds, collisionComponent, ref childBounds);
            }
        }

        private void TestIsOverlappingThisComponent(ref SDL_Rect ownerBounds, CollisionComponent collisionComponent, ref SDL_Rect childBounds)
        {
            if (SDL_IntersectRect(ref childBounds, ref ownerBounds, out SdlRectMath.DummyEndResult) == SDL_bool.SDL_TRUE)
            {
                _overlapEvent.lastContact = collisionComponent.owner;

                onOverlaped?.Invoke(_overlapEvent);
                _overlapContacts.Add(collisionComponent.owner);
            }
        }

        private void TestIsStillOverlapping(ref SDL_Rect ownerBounds, CollisionComponent collisionComponent, ref SDL_Rect childBounds)
        {
            if (SDL_IntersectRect(ref childBounds, ref ownerBounds, out SdlRectMath.DummyEndResult) != SDL_bool.SDL_TRUE)
            {
                _overlapEvent.lastContact = collisionComponent.owner;

                onStopsOverlaping?.Invoke(_overlapEvent);

                _overlapContacts.Remove(collisionComponent.owner);
            }
        }
    }
}
