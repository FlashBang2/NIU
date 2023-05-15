using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public interface ICollideable
    {
        object LinkedEntity { get; }

        Rect Bounds { get; }

        int CollisionGroup { get; set; }

        bool TestCollision(Ray ray, uint mask);
    }
}
