using System.Collections.Generic;

namespace WpfApp1
{
    public static class Physics
    {
        private static readonly List<IPhysicsBody> _dynamicBodies = new List<IPhysicsBody>();
        private static readonly List<IPhysicsBody> _staticBodies = new List<IPhysicsBody>();

        public static double Gravity = 5;

        public static void Update()
        {
            foreach (var body in _dynamicBodies)
            {
                body.PhysicsUpdate();
            }
        }

        public static bool IsCollidingWithAnyObject(IPhysicsBody physicsBody)
        {
            foreach (var body in _dynamicBodies)
            {
                if (body != physicsBody)
                {
                    if (physicsBody.IsOverlaping(body))
                    {
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
                        return true;
                    }
                }
            }

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
