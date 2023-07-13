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
        public Vector start;
        public Vector delta; // Direction + Length

        public Vector extend;
        public bool isRay;

        public Vector end { get => start + delta; }

        public void Init(Vector start, Vector end)
        {
            delta = end - start;
            extend = new Vector();

            this.start = start;
            isRay = true;
        }
    }
}
