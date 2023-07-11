﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Windows;
using static SDL2.SDL;

namespace WpfApp1
{
    public class CollisionComponent : Component
    {
        public struct OverlapEvent
        {
            public IEntity LastContact;
            public IEntity Self;
            public Vector ContactPoint;
        }

        public bool IsStatic = true;
        public bool IsOverlaping = false;
        public bool IsTrigger = false;

        public float AccumulatedY = 0;

        public event Action<OverlapEvent> Overlaped;
        public event Action<OverlapEvent> StopOverlaping;

        private List<IEntity> _contacts = new List<IEntity>();
        private static List<CollisionComponent> collisionComponents = new List<CollisionComponent>();
        private static IList<IEntity> EmptyList = new List<IEntity>();

        public static bool RayCast(Ray ray, out OverlapEvent evt)
        {
            return RayCast(ray, EmptyList, out evt);
        }

        public override void Activated()
        {
            base.Activated();
            collisionComponents.Add(this);
        }

        public override void Deactivated()
        {
            base.Deactivated();
            collisionComponents.Remove(this);
        }

        public override bool Destroyed()
        {
            base.Deactivated();
            collisionComponents.Remove(this);
            return false;
        }

        public static bool RayCast(Ray ray, IList<IEntity> ignored, out OverlapEvent evt)
        {
            double t = 0.00;

            if (TestPointInAnyComponent(ignored, ray.Start, out evt))
            {
                return true;
            }

            if (TestPointInAnyComponent(ignored, ray.End, out evt))
            {
                return true;
            }

            while (t < 1)
            {
                Vector current = (1 - t) * ray.Start + t * ray.End;

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
            foreach (var child in collisionComponents)
            {
                if (child.Owner.IsActive && ignored.FirstOrDefault(e => e == child.Owner) == null)
                {
                    var bounds = child.Owner.Bounds;

                    if (SdlRectMath.IsPointInRect(ref bounds, (float)point.X, (float)point.Y))
                    {
                        evt.Self = null;
                        evt.LastContact = child.Owner;
                        evt.ContactPoint = point;
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
            collisionComponents.Add(this);
            isActive = true;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            TestCollision();
        }

        public void TestCollision()
        {
            SDL_Rect ownerBounds = Owner.Bounds;
            IsOverlaping = false;

            foreach (var child in collisionComponents)
            {
                if (child != this && child.Owner.IsActive)
                {
                    if (IsTrigger)
                    {
                        TestForTrigger(ownerBounds, child);
                        continue;
                    }
                 
                    TestForCollision(ownerBounds, child);
                }
            }
        }

        private void TestForCollision(SDL_Rect ownerBounds, CollisionComponent child)
        {
            SDL_Rect childBounds = child.Owner.Bounds;

            if (SDL_IntersectRect(ref childBounds, ref ownerBounds, out SdlRectMath.DummyEndResult) == SDL_bool.SDL_TRUE && !child.IsTrigger)
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
                evt.Self = Owner;
                evt.LastContact = child.Owner;

                Overlaped?.Invoke(evt);
                _contacts.Add(child.Owner);
            }
        }

        private void CorrectVerticalLocation(SDL_Rect rect)
        {
            Entity o = (Entity)Owner;

            if (o.HasComponent<CharacterMovementComponent>())
            {
                CharacterMovementComponent movementComponent = o.GetComponent<CharacterMovementComponent>();
                if (movementComponent.Velocity.Y > 0)
                {
                    AccumulatedY = (float)rect.h + 0.01f;
                    Owner.AddWorldOffset(0, -AccumulatedY);
                    IsOverlaping = true;
                }
                else if (movementComponent.Velocity.Y < 0)
                {
                    AccumulatedY = (float)rect.h + 0.01f;
                    Owner.AddWorldOffset(0, AccumulatedY);
                    IsOverlaping = true;
                }
            }
            else
            {
                if (!IsStatic)
                {
                    AccumulatedY = (float)rect.h + 0.01f;
                    Owner.AddWorldOffset(0, -AccumulatedY);
                    IsOverlaping = true;
                }
            }
        }

        private void CorrectHorizontalLocation(CollisionComponent child, SDL_Rect rect)
        {
            var x = Owner.PosX - child.Owner.PosX;


            if (!IsStatic && rect.w > 0)
            {
                Owner.AddWorldOffset(Math.Sign(x) * (rect.w + Math.Sign(rect.w)), 0);
                IsOverlaping = true;
            }
        }

        private void TestForTrigger(SDL_Rect ownerBounds, CollisionComponent child)
        {
            SDL_Rect cb = child.Owner.Bounds;

            if (_contacts.Find(e => e == child) != null)
            {
                if (SDL_IntersectRect(ref cb, ref ownerBounds, out SdlRectMath.DummyEndResult) != SDL_bool.SDL_TRUE)
                {
                    OverlapEvent evt = new OverlapEvent();
                    evt.Self = Owner;
                    evt.LastContact = child.Owner;

                    StopOverlaping?.Invoke(evt);

                    _contacts.Remove(child.Owner);
                }
            }
            else
            {
                if (SDL_IntersectRect(ref cb, ref ownerBounds, out SdlRectMath.DummyEndResult) == SDL_bool.SDL_TRUE)
                {
                    if (child.Owner.Name.Equals("mario"))
                    {
                        Console.WriteLine(Owner.Name);
                    }
                    OverlapEvent evt = new OverlapEvent();
                    evt.Self = Owner;
                    evt.LastContact = child.Owner;

                    Overlaped?.Invoke(evt);
                    _contacts.Add(child.Owner);
                }
            }
        }
    }
}
