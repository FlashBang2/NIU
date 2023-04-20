using System.Collections.Generic;

namespace WpfApp1
{
    public static class Physics
    {
        private static readonly List<IPhysicsBody> _dynamicBodies = new List<IPhysicsBody>();
        private static readonly List<IPhysicsBody> _staticBodies = new List<IPhysicsBody>();

        public static readonly double SecondsPerFrame = 1.0 / 60.0;

        public static double Gravity = 10 * SecondsPerFrame;

        public static void Update()
        {
            foreach (var body in _dynamicBodies)
            {
                body.PhysicsUpdate();
            }
        }

        public static bool IsCollidingWithAnyObject(IPhysicsBody physicsBody, out IPhysicsBody firstHit)
        {
            foreach (var body in _dynamicBodies)
            {
                if (body != physicsBody)
                {
                    if (physicsBody.IsOverlaping(body))
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
                    if (physicsBody.IsOverlaping(body))
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
    }
}
