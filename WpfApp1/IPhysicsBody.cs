using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public interface IPhysicsBody
    {
        double PosX { get; set; }
        double PosY { get; set; }

        Rect Bounds { get; }

        bool IsStatic { get; set; }
        Vector Velocity { get; set; }

        bool ShouldApplyGravity { get; set; }
        double GravityScale { get; set; }

        bool IsOverlaping(IPhysicsBody other);
        void PhysicsUpdate();
        void AddOffset(Vector offset);

        Vector LastMove { get; }
    }
}
