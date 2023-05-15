using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public struct Ray
    {
        public Vector Start;
        public Vector Delta; // Direction + Length

        public Vector Extend;
        public bool IsRay;

        public void Init(Vector start, Vector end)
        {
            Delta = end - start;
            Extend = new Vector();

            Start = start;
            IsRay = true;
        }

        public void Init(Vector start, Vector end, Rect bounds)
        {
            Delta = end - start;
            Extend = bounds.Extend;

            Start = start;
            IsRay = Extend.LengthSquared < 1e-6;
        }
    }
}
