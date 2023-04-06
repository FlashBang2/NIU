using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public struct Rect
    {
        private Vector _min;
        private Vector _max;

        public Rect(Vector min, Vector max)
        {
            _min = min;
            _max = max;
        }

        public static Rect FromOriginAndExtend(Vector origin, Vector extend)
        {
            var halfExtend = extend / 2;

            return new Rect(origin - halfExtend, origin + halfExtend);
        }

        public bool IsOverlaping(Rect rect)
        {
            return (_min.X <= rect._max.X) && (_max.X >= rect._min.X) &&
                (_min.Y <= rect._max.Y) && (_max.Y >= rect._min.Y);
        }

        public bool IsOverlaping(Vector point)
        {
            return (point.X >= _min.X && point.X <= _max.X)
                && (point.Y >= _min.Y && point.Y <= _max.Y);
        }

        public Rect GetOverlap(Rect rect)
        {
            Vector minVector = new Vector();
            Vector maxVector = new Vector();

            minVector.X = Math.Max(_min.X, rect._min.X);
            maxVector.X = Math.Min(_max.X, rect._max.X);

            minVector.Y = Math.Max(_min.Y, rect._min.Y);
            maxVector.Y = Math.Min(_max.Y, rect._max.Y);

            return new Rect(minVector, maxVector);
        }
    }
}
