using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfApp1
{
    public static class Physics
    {
        private static readonly List<IPhysicsBody> _dynamicBodies = new List<IPhysicsBody>();
        private static readonly List<IPhysicsBody> _staticBodies = new List<IPhysicsBody>();

        private static readonly List<OverlapResult> _lastOverlaps = new List<OverlapResult>();

        private static readonly Dictionary<int, List<ICollideable>> _collision = new Dictionary<int, List<ICollideable>>();


        public static readonly double SecondsPerFrame = 1.0 / 60.0;

        public static double Gravity = 10 * SecondsPerFrame;

        public struct OverlapResult : IEquatable<OverlapResult>
        {
            public readonly IPhysicsBody DynamicBody;
            public readonly IPhysicsBody Trigger;

            public OverlapResult(IPhysicsBody dynamicBody, IPhysicsBody trigger)
            {
                this.DynamicBody = dynamicBody;
                this.Trigger = trigger;
            }

            public bool Equals(OverlapResult other)
            {
                return this == other;
            }

            public static bool operator ==(OverlapResult lhs, OverlapResult rhs) { return lhs.DynamicBody == rhs.DynamicBody && rhs.Trigger == lhs.Trigger; }
            public static bool operator !=(OverlapResult lhs, OverlapResult rhs) { return !(lhs.DynamicBody == rhs.DynamicBody && rhs.Trigger == lhs.Trigger); }
        }

        public static event Action<OverlapResult> OverlapedBody;
        public static event Action<OverlapResult> StopOverlaping;

        public static void Update()
        {
            foreach (var body in _dynamicBodies)
            {
                body.PhysicsUpdate();
            }

            List<OverlapResult> _toRemove = _lastOverlaps.ToList();

            foreach (var body in _staticBodies)
            {
                if (body.IsTrigger)
                {
                    foreach (var dynamic in _dynamicBodies)
                    {
                        if (dynamic.IsOverlaping(body))
                        {
                            var overlap = new OverlapResult(dynamic, body);

                            if (_toRemove.FindIndex(v => v.Equals(overlap)) != -1)
                            {
                                _toRemove.Remove(overlap);
                            }
                            else
                            {
                                _lastOverlaps.Add(overlap);
                                OverlapedBody?.Invoke(overlap);
                            }
                        }
                    }
                }
            }

            foreach (var overlap in _toRemove)
            {
                _lastOverlaps.Remove(overlap);
                StopOverlaping?.Invoke(overlap);
            }
        }

        public static bool IsCollidingWithAnyObject(IPhysicsBody physicsBody, out IPhysicsBody firstHit)
        {
            foreach (var body in _dynamicBodies)
            {
                if (body != physicsBody)
                {
                    if (!body.IsTrigger && physicsBody.IsOverlaping(body))
                    {
                        firstHit = body;
                        return true;
                    }
                }
            }

            foreach (var body in _staticBodies)
            {
                if (body != physicsBody)
                {
                    if (!body.IsTrigger && physicsBody.IsOverlaping(body))
                    {
                        firstHit = body;
                        return true;
                    }
                }
            }

            firstHit = null;
            return false;
        }

        public static void AddPhysicsBody(IPhysicsBody body)
        {
            if (body.IsStatic)
            {
                _staticBodies.Add(body);
            }
            else
            {
                _dynamicBodies.Add(body);
            }
        }

        public static void AddPhysicsBody(ICollideable collideable)
        {
            if (!_collision.TryGetValue(collideable.CollisionGroup, out _))
            {
                _collision.Add(collideable.CollisionGroup, new List<ICollideable> { collideable });
            }
            else
            {
                _collision[collideable.CollisionGroup].Add(collideable);
            }
        }


        public static void RemovePhysicsBody(IPhysicsBody body)
        {
            if (body.IsStatic)
            {
                _staticBodies.Remove(body);
            }
            else
            {
                _dynamicBodies.Remove(body);
            }
        }

        public static void RemovePhysicsBody(ICollideable collideable)
        {
            if (_collision.TryGetValue(collideable.CollisionGroup, out _))
            {
                _collision[collideable.CollisionGroup].Remove(collideable);
            }
        }

        public static bool TestRay(Ray ray, ICollideable toIgnore)
        {
            foreach (var body in _collision[toIgnore.CollisionGroup])
            {
                if (body != toIgnore)
                {
                    if (body.TestCollision(ray, 0))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
