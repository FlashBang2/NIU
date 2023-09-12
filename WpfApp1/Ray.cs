﻿using System.Windows;

namespace WpfApp1
{
    public struct Ray
    {
        public Vector start;
        public Vector delta; // Direction + Length

        public Vector extend;
        public bool isRay;

        public Vector end;

        public void Init(Vector start, Vector end)
        {
            delta = end - start;
            extend = new Vector();
            this.end = end;

            this.start = start;
            isRay = true;
        }
    }
}
