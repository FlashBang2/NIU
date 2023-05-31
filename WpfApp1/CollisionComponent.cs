using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace WpfApp1
{
    public class CollisionComponent : Component
    {
        public bool IsStatic = true;
        public bool IsOverlaping = false;

        public float AccumulatedY = 0;

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
                if (child.HasComponent<CollisionComponent>() && child != Owner)
                {
                    CollisionComponent c = child.GetComponent<CollisionComponent>();

                    Rect childBounds = child.Bounds;

                    if (childBounds.IsOverlaping(ownerBounds) && ownerBounds.IsOverlaping(childBounds))
                    {
                        Rect rect = childBounds.GetOverlap(ownerBounds);

                        if (Math.Abs(rect.Width) < Math.Abs(rect.Height))
                        {
                            if (!IsStatic && rect.Width > 0.01)
                            {
                                Owner.AddWorldOffset(-rect.Width - Math.Sign(rect.Width) * 0.01f, 0);

                                IsOverlaping = true;
                            }
                        }
                        else
                        {
                            if (!IsStatic && rect.Height > 0.01)
                            {
                                AccumulatedY = (float)-rect.Height - Math.Sign(rect.Height) * 0.01f;
                                Owner.AddWorldOffset(0, AccumulatedY);
                                IsOverlaping = true;   
                            }
                        }


                        break;
                    }
                }
            }
        }
    }
}
