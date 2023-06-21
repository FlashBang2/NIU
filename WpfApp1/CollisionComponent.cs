using System;
using System.Collections.Generic;
using System.Windows;

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
            evt = new OverlapEvent();

            double t = 0.00;
            IEntity[] children = Entity.RootEntity.GetChildren();


            while (t < 1)
            {
                Vector current = (1 - t) * ray.Start + t * ray.End;

                foreach (var child in children)
                {
                    if (child.HasComponent<CollisionComponent>() && child.IsActive && ignored.Find(e => e == child) == null)
                    {
                        var bounds = child.Bounds;

                        if (bounds.IsOverlaping(current))
                        {
                            evt.Self = null;
                            evt.LastContact = child;
                            evt.ContactPoint = current;
                            return true;
                        }
                    }

                }
                t += 0.01f;
            }

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
            IEntity[] children = Entity.RootEntity.GetChildren();

            Rect ownerBounds = Owner.Bounds;
            IsOverlaping = false;

            foreach (var child in children)
            {
                if (child.HasComponent<CollisionComponent>() && child != Owner && child.IsActive)
                {
                    if (IsTrigger)
                    {
                        Rect cb = child.Bounds;

                        if (_contacts.Find(e => e == child) != null)
                        {
                            if (cb.IsOverlaping(ownerBounds) && ownerBounds.IsOverlaping(cb))
                            {
                            }
                            else
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
                            if (cb.IsOverlaping(ownerBounds) && ownerBounds.IsOverlaping(cb))
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

                    Rect childBounds = child.Bounds;

                    if (childBounds.IsOverlaping(ownerBounds) && ownerBounds.IsOverlaping(childBounds) && !c.IsTrigger)
                    {
                        Rect rect = childBounds.GetOverlap(ownerBounds);

                        if (Math.Abs(rect.Width) < Math.Abs(rect.Height))
                        {
                            var x = Owner.PosX - child.PosX;


                            if (!IsStatic && rect.Width > 0)
                            {
                                Owner.AddWorldOffset(Math.Sign(x) * (rect.Width + Math.Sign(rect.Width)), 0);
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
                                    AccumulatedY = (float)rect.Height + 0.01f;
                                    Owner.AddWorldOffset(0, -AccumulatedY);
                                    IsOverlaping = true;
                                }
                                else if (movementComponent.Velocity.Y < 0)
                                {
                                    AccumulatedY = (float)rect.Height + 0.01f;
                                    Owner.AddWorldOffset(0, AccumulatedY);
                                    IsOverlaping = true;
                                }
                            }
                            else
                            {
                                if (!IsStatic)
                                {
                                    AccumulatedY = (float)rect.Height + 0.01f;
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
