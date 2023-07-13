using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media;
using static SDL2.SDL;

namespace WpfApp1
{
    public class CollisionComponent : Component
    {
        public struct OverlapEvent
        {
            public IEntity lastContact;
            public IEntity self;
            public Vector contactPoint;
        }

        public bool isStatic = true;
        public bool isOverlaping = false;
        public bool isTrigger = false;

        public float accumulatedY = 0;

        public event Action<OverlapEvent> overlaped;
        public event Action<OverlapEvent> stopsOverlaping;

        private List<IEntity> _contacts = new List<IEntity>();
        private static List<CollisionComponent> _collisionComponents = new List<CollisionComponent>();
        private static IList<IEntity> _emptyList = new List<IEntity>();

        public static bool RayCast(Ray ray, out OverlapEvent evt)
        {
            return RayCast(ray, _emptyList, out evt);
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

        public static bool RayCast(Ray ray, IList<IEntity> ignored, out OverlapEvent evt)
        {
            double t = 0.00;

            if (TestPointInAnyComponent(ignored, ray.start, out evt))
            {
                return true;
            }

            if (TestPointInAnyComponent(ignored, ray.end, out evt))
            {
                return true;
            }

            while (t < 1)
            {
                Vector current = (1 - t) * ray.start + t * ray.end;

                if (TestPointInAnyComponent(ignored, current, out evt))
                {
                    return true;
                }

                t += 0.1f;
            }

            evt = new OverlapEvent();
            return false;
        }

        static bool TestPointInAnyComponent(IList<IEntity> ignored, Vector point, out OverlapEvent evt)
        {
            foreach (var child in _collisionComponents)
            {
                if (child.owner.isActive && ignored.FirstOrDefault(e => e == child.owner) == null)
                {
                    var bounds = child.owner.bounds;

                    if (SdlRectMath.IsPointInRect(ref bounds, (float)point.X, (float)point.Y))
                    {
                        evt.self = null;
                        evt.lastContact = child.owner;
                        evt.contactPoint = point;
                        return true;
                    }
                }

            }

            evt = new OverlapEvent();
            return false;
        }

        public override void Spawned()
        {
            base.Spawned();
            _collisionComponents.Add(this);
            isActive = true;
        }

        bool firstFrame = true;

        public bool shouldDraw => true;

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            if (firstFrame)
            {
                firstFrame = false;
                return;
            }

            TestCollision();
        }

        public void TestCollision()
        {
            if (isStatic)
            {
                return;
            }

            SDL_Rect ownerBounds = owner.bounds;
            isOverlaping = false;

            foreach (var child in _collisionComponents)
            {
                if (child != this && child.owner.isActive)
                {
                    if (isTrigger)
                    {
                        TestForTrigger(ownerBounds, child);
                        continue;
                    }

                    TestForCollision(ref ownerBounds, child);
                }
            }
        }

        private void TestForCollision(ref SDL_Rect ownerBounds, CollisionComponent child)
        {
            SDL_Rect childBounds = child.owner.bounds;

#if false
            float contactPointX, contactPointY;
            int contactNormalX, contactNormalY;
            float hitNear = 0;

            IsOverlaping = false;
            
            if (Math.Abs(child.Owner.PosX - ownerBounds.x) < ownerBounds.w &&
                Math.Abs(child.Owner.PosY - ownerBounds.y) < ownerBounds.h)
            {
                return;
            }

            if (SdlRectMath.RayVsRect(ownerBounds.x, ownerBounds.y, child.Owner.PosX - ownerBounds.x + ownerBounds.w, child.Owner.PosY - ownerBounds.y + ownerBounds.h, ref childBounds, out contactPointX, out contactPointY, out contactNormalX, out contactNormalY, out hitNear))
            {
                if (!IsStatic && hitNear <= 1)
                {
                    if (SDL_IntersectRect(ref childBounds, ref ownerBounds, out SdlRectMath.DummyEndResult) != SDL_bool.SDL_FALSE)
                    {
                        var x = contactNormalX * SdlRectMath.DummyEndResult.w;
                        var y = contactNormalY * SdlRectMath.DummyEndResult.h;

                        if (Owner.Name.Equals("mario"))
                        {
                            SDL_IntersectRect(ref childBounds, ref ownerBounds, out SdlRectMath.DummyEndResult);

                            Console.WriteLine("Test");
                        }

                        Owner.AddWorldOffset(x, y);

                        IsOverlaping = true;
                    }
                }
            }


#else
            if (SDL_IntersectRect(ref childBounds, ref ownerBounds, out SdlRectMath.DummyEndResult) == SDL_bool.SDL_TRUE && !child.isTrigger)
            {
                SDL_Rect rect = SdlRectMath.DummyEndResult;

                if (Math.Abs(rect.w) < Math.Abs(rect.h))
                {
                    CorrectHorizontalLocation(child, rect);
                }
                else
                {
                    CorrectVerticalLocation(rect);
                }

                OverlapEvent evt = new OverlapEvent();
                evt.self = owner;
                evt.lastContact = child.owner;

                overlaped?.Invoke(evt);
                _contacts.Add(child.owner);
            }
#endif
        }

        private void CorrectVerticalLocation(SDL_Rect rect)
        {
            Entity o = (Entity)owner;

            if (o.HasComponent<CharacterMovementComponent>())
            {
                CharacterMovementComponent movementComponent = o.GetComponent<CharacterMovementComponent>();
                if (movementComponent.velocity.Y > 0)
                {
                    accumulatedY = rect.h + 0.05f;
                    owner.AddWorldOffset(0, -accumulatedY);
                    isOverlaping = true;
                }
                else if (movementComponent.velocity.Y < 0)
                {
                    accumulatedY = rect.h + 0.05f;
                    owner.AddWorldOffset(0, accumulatedY);
                    isOverlaping = true;
                }
            }
            else
            {
                accumulatedY = rect.h + 0.01f;
                owner.AddWorldOffset(0, -accumulatedY);
                isOverlaping = true;
            }
        }

        private void CorrectHorizontalLocation(CollisionComponent child, SDL_Rect rect)
        {
            var x = owner.posX - child.owner.posX;


            if (!isStatic && rect.w > 0)
            {
                owner.AddWorldOffset(Math.Sign(x) * (rect.w + Math.Sign(rect.w)), 0);
                isOverlaping = true;
            }
        }

        private void TestForTrigger(SDL_Rect ownerBounds, CollisionComponent child)
        {
            SDL_Rect cb = child.owner.bounds;

            if (_contacts.Find(e => e == child) != null)
            {
                if (SDL_IntersectRect(ref cb, ref ownerBounds, out SdlRectMath.DummyEndResult) != SDL_bool.SDL_TRUE)
                {
                    OverlapEvent evt = new OverlapEvent();
                    evt.self = owner;
                    evt.lastContact = child.owner;

                    stopsOverlaping?.Invoke(evt);

                    _contacts.Remove(child.owner);
                }
            }
            else
            {
                if (SDL_IntersectRect(ref cb, ref ownerBounds, out SdlRectMath.DummyEndResult) == SDL_bool.SDL_TRUE)
                {
                    if (child.owner.name.Equals("mario"))
                    {
                        Console.WriteLine(owner.name);
                    }
                    OverlapEvent evt = new OverlapEvent();
                    evt.self = owner;
                    evt.lastContact = child.owner;

                    overlaped?.Invoke(evt);
                    _contacts.Add(child.owner);
                }
            }
        }
    }
}
