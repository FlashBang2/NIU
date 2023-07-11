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
        public bool IsStatic = true;
        public bool IsOverlaping = false;
        public bool IsTrigger = false;
        private List<IEntity> _contacts = new List<IEntity>();

        public struct OverlapEvent
        {
            public IEntity LastContact;
            public IEntity Self;
            public Vector ContactPoint;
        }

        public event Action<OverlapEvent> Overlaped;
        public event Action<OverlapEvent> StopOverlaping;

        public float AccumulatedY = 0;

        public static bool RayCast(Ray ray, out OverlapEvent evt)
        {
            List<IEntity> ignored = new List<IEntity>();
            return RayCast(ray, ignored, out evt);
        }

        public static bool RayCast(Ray ray, List<IEntity> ignored, out OverlapEvent evt)
        {
            double t = 0.00;
            IEntity[] children = Entity.RootEntity.GetChildren();


            if (TestPointInAnyComponent(children, ignored, ray.Start, out evt))
            {
                return true;
            }

            if (TestPointInAnyComponent(children, ignored, ray.End, out evt))
            {
                return true;
            }

            while (t < 1)
            {
                Vector current = (1 - t) * ray.Start + t * ray.End;

                if (TestPointInAnyComponent(children, ignored, current, out evt))
                {
                    return true;
                }

                t += 0.1f;
            }

            evt = new OverlapEvent();
            return false;
        }

        static bool TestPointInAnyComponent(IEntity[] children, IList<IEntity> ignored, Vector point, out OverlapEvent evt)
        {
            foreach (var child in children)
            {
                if (child.HasComponent<CollisionComponent>() && child.IsActive && ignored.FirstOrDefault(e => e == child) == null)
                {
                    var bounds = child.Bounds;

                    if (SdlRectMath.IsPointInRect(ref bounds, (float)point.X, (float)point.Y))
                    {
                        evt.Self = null;
                        evt.LastContact = child;
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
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            TestCollision();
        }

        public void TestCollision()
        {
            return;
            IEntity[] children = Entity.RootEntity.GetChildren();

            SDL_Rect ownerBounds = Owner.Bounds;
            IsOverlaping = false;

            foreach (var child in children)
            {
                if (child.HasComponent<CollisionComponent>() && child != Owner && child.IsActive)
                {
                    if (IsTrigger)
                    {
                        SDL_Rect cb = child.Bounds;

                        if (_contacts.Find(e => e == child) != null)
                        {
                            if (SDL_IntersectRect(ref cb, ref ownerBounds, out SdlRectMath.DummyEndResult) != SDL_bool.SDL_TRUE)
                            {
                                OverlapEvent evt = new OverlapEvent();
                                evt.Self = Owner;
                                evt.LastContact = child;

                                StopOverlaping?.Invoke(evt);

                                _contacts.Remove(child);
                            }
                        }
                        else
                        {
                            if (SDL_IntersectRect(ref cb, ref ownerBounds, out SdlRectMath.DummyEndResult) == SDL_bool.SDL_TRUE)
                            {
                                if (child.Name.Equals("mario"))
                                {
                                    Console.WriteLine(Owner.Name);
                                }
                                OverlapEvent evt = new OverlapEvent();
                                evt.Self = Owner;
                                evt.LastContact = child;

                                Overlaped?.Invoke(evt);
                                _contacts.Add(child);
                            }
                        }

                        continue;
                    }

                    CollisionComponent c = child.GetComponent<CollisionComponent>();

                    SDL_Rect childBounds = child.Bounds;

                    if (SDL_IntersectRect(ref childBounds, ref ownerBounds, out SdlRectMath.DummyEndResult) == SDL_bool.SDL_TRUE && !c.IsTrigger)
                    {
                        SDL_Rect rect = SdlRectMath.DummyEndResult;

                        if (Math.Abs(rect.w) < Math.Abs(rect.h))
                        {
                            var x = Owner.PosX - child.PosX;


                            if (!IsStatic && rect.w > 0)
                            {
                                Owner.AddWorldOffset(Math.Sign(x) * (rect.w + Math.Sign(rect.w)), 0);
                                IsOverlaping = true;
                            }
                        }
                        else
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

                        OverlapEvent evt = new OverlapEvent();
                        evt.Self = Owner;
                        evt.LastContact = child;

                        Overlaped?.Invoke(evt);
                        _contacts.Add(child);

                        break;
                    }
                }
            }
        }
    }
}
