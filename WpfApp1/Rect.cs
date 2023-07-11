using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static SDL2.SDL;

namespace WpfApp1
{
    public static class SdlRectMath
    {
        public static readonly SDL_Rect UnlimitedRect = new SDL_Rect();
        public static SDL_Rect DummyEndResult = new SDL_Rect();

        static SdlRectMath()
        {
            UnlimitedRect.w = int.MaxValue;
            UnlimitedRect.h = int.MaxValue;
        }

        static public void FromMinAndMax(Vector Min, Vector Max, out SDL_Rect OutRect)
        {
            FromMinAndMax((float)Min.X, (float)Min.Y, (float)Max.X, (float)Max.Y, out OutRect);
        }

        static public void FromMinAndMax(float MinX, float MinY, float MaxX, float MaxY, out SDL_Rect OutRect)
        {
            bool bHasUserMistakenMinAndMax = MinX > MaxX && MinY > MaxY;

            if (bHasUserMistakenMinAndMax)
            {
                (MaxX, MinX) = (MinX, MaxX);
                (MaxY, MinY) = (MinY, MaxY);
            }

            OutRect.x = (int)MinX;
            OutRect.y = (int)MinY;
            OutRect.w = (int)(MaxX - MinX);
            OutRect.h = (int)(MaxY - MinY);
        }

        static public void FromMinAndMaxByRef(float MinX, float MinY, float MaxX, float MaxY, ref SDL_Rect OutRect)
        {
            bool bHasUserMistakenMinAndMax = MinX > MaxX && MinY > MaxY;

            if (bHasUserMistakenMinAndMax)
            {
                (MaxX, MinX) = (MinX, MaxX);
                (MaxY, MinY) = (MinY, MaxY);
            }

            OutRect.x = (int)MinX;
            OutRect.y = (int)MinY;
            OutRect.w = (int)(MaxX - MinX);
            OutRect.h = (int)(MaxY - MinY);
        }

        static public void FromOriginAndExtend(float OriginX, float OriginY, float ExtendX, float ExtendY, out SDL_Rect OutRect)
        {
            OutRect.x = (int)(OriginX - ExtendX);
            OutRect.y = (int)(OriginY - ExtendY);
            OutRect.w = (int)(OriginX);
            OutRect.h = (int)(OriginY);
        }

        static public void FromXywh(float X, float Y, float W, float H, out SDL_Rect OutRect)
        {
            OutRect.x = (int)(X);
            OutRect.y = (int)(Y);
            OutRect.w = (int)(W);
            OutRect.h = (int)(H);
        }

        public static Vector PendicularVector(Vector v)
        {
            return new Vector(-v.Y, v.X);
        }

        public static bool IsPointInRect(ref SDL_Rect RectToTest, float X, float Y)
        {
            return X >= RectToTest.x && Y >= RectToTest.y && X <= RectToTest.x + RectToTest.w && Y <= RectToTest.y + RectToTest.h;
        }
    }

    public struct Rect
    {
        private readonly Vector[] _vertices;
        private Vector _center;
        private readonly double _angle;

        public static readonly int TopLeftIndex = 0;
        public static readonly int TopRightIndex = 1;
        public static readonly int BottomLeftIndex = 2;
        public static readonly int BottomRightIndex = 3;

        public Rect(Vector min, Vector max)
        {
            bool hasUserMistakenMinAndMax = min.X > max.X && min.Y > max.Y;

            if (hasUserMistakenMinAndMax)
            {
                (max, min) = (min, max);
            }

            _vertices = new Vector[BottomRightIndex + 1];

            _vertices[TopLeftIndex] = min;
            _vertices[TopRightIndex] = new Vector(max.X, min.Y);
            _vertices[BottomLeftIndex] = new Vector(min.X, max.Y);
            _vertices[BottomRightIndex] = max;

            _angle = 0;
            _center = 0.5 * (max + min);
        }

        public Rect(Vector origin, Vector extend, double angle)
        {
            _vertices = new Vector[BottomRightIndex + 1];
            _angle = angle;

            _vertices[TopLeftIndex] = new Vector(origin.X - extend.X, origin.Y - extend.Y);
            _vertices[TopRightIndex] = new Vector(origin.X + extend.X, origin.Y - extend.Y);
            _vertices[BottomLeftIndex] = new Vector(origin.X - extend.X, origin.Y + extend.Y);
            _vertices[BottomRightIndex] = new Vector(origin.X + extend.X, origin.Y + extend.Y);

            for (int i = 0; i < _vertices.Length; i++)
            {
                double x = _vertices[i].X;

                // rotate vector
                _vertices[i].X = Math.Cos(angle) * x - Math.Sin(angle) * _vertices[i].Y;
                _vertices[i].Y = Math.Sin(angle) * x + Math.Cos(angle) * _vertices[i].Y;
            }

            _center = 0.5 * (_vertices[TopLeftIndex] + _vertices[BottomRightIndex]);
        }

        public Rect(double x, double y, double w, double h)
        {
            _vertices = new Vector[BottomRightIndex + 1];

            _vertices[TopLeftIndex] = new Vector(x, y);
            _vertices[TopRightIndex] = new Vector(x + w, y);
            _vertices[BottomLeftIndex] = new Vector(x, y + h);
            _vertices[BottomRightIndex] = new Vector(x + w, y + h);

            _center = new Vector(x + w / 2, y + h / 2);
            _angle = 0;
        }

        public Rect(Vector[] vertices)
        {
            Debug.Assert(vertices.Length >= 4);

            _vertices = new Vector[BottomRightIndex + 1];
            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i] = vertices[i];
            }

            var x = (_vertices[BottomRightIndex] - _vertices[BottomLeftIndex]);
            x.Normalize();

            var y = new Vector(Math.Max(_vertices[BottomRightIndex].X, -_vertices[BottomLeftIndex].X), Math.Max(_vertices[BottomRightIndex].Y, -_vertices[BottomLeftIndex].Y));
            _angle = x * y;
            _center = 0.5 * (_vertices[TopLeftIndex] + _vertices[BottomRightIndex]);
        }

        public double Width => (_vertices[TopRightIndex] - _vertices[TopLeftIndex]).Length;
        public double Height => (_vertices[BottomLeftIndex] - _vertices[TopLeftIndex]).Length;

        public double Left => _vertices[TopLeftIndex].X;
        public double Top => _vertices[TopLeftIndex].Y;
        public double Right => _vertices[TopRightIndex].X;
        public double Down => _vertices[BottomLeftIndex].Y;

        public Vector Extend => new Vector(Width, Height) / 2;
        public Vector Center => _center;

        public double Area => Width * Height;
        public static readonly Rect Unlimited = new Rect(new Vector(int.MinValue, int.MinValue), new Vector(int.MaxValue, int.MaxValue));
        public double Angle => _angle;

        public SDL_Rect AsSDLRect
        {
            get
            {
                if (this == Unlimited)
                {
                    SDL_Rect rect = new SDL_Rect();
                    rect.w = 99999;
                    rect.h = 99999;
                    rect.x = 0;
                    rect.y = 0;
                    return rect;
                }

                SDL_Rect r = new SDL_Rect();
                r.w = (int)Width;
                r.h = (int)Height;
                r.x = (int)Left;
                r.y = (int)Top;

                return r;
            }
        }

        public static Rect FromOriginAndExtend(Vector origin, Vector extend)
        {
            Vector halfExtend = extend / 2;

            return new Rect(origin - halfExtend, origin + halfExtend);
        }

        public bool IsOverlaping(Rect rect)
        {
            var _min = _vertices[TopLeftIndex];
            var rect_min = rect._vertices[TopLeftIndex];
            var _max = _vertices[BottomRightIndex];
            var rect_max = rect._vertices[BottomRightIndex];


            return Area > 0 && rect.Area > 0 && (_min.X <= rect_max.X) && (_max.X >= rect_min.X) &&
               (_min.Y <= rect_max.Y) && (_max.Y >= rect_min.Y);
        }

        private double FindSeparation(Rect rect)
        {
            // SAT method
            double separation = double.MinValue;

            foreach (Vector vertexA in _vertices)
            {
                Vector normal = PendicularVector(vertexA);
                double minSep = double.MaxValue;

                foreach (Vector vertexB in rect._vertices)
                {
                    minSep = Math.Min(minSep, (vertexB - vertexA) * normal);
                }

                if (minSep > separation)
                {
                    separation = minSep;
                }
            }

            return separation;
        }

        public static Vector PendicularVector(Vector v)
        {
            return new Vector(-v.Y, v.X);
        }

        public bool IsOverlaping(Vector point)
        {
            var _min = _vertices[TopLeftIndex];
            var _max = _vertices[BottomRightIndex];

            return (point.X >= _min.X && point.X <= _max.X)
                && (point.Y >= _min.Y && point.Y <= _max.Y);
        }

        public Rect GetOverlap(Rect rect)
        {
            var _min = _vertices[TopLeftIndex];
            var rect_min = rect._vertices[TopLeftIndex];
            var _max = _vertices[BottomRightIndex];
            var rect_max = rect._vertices[BottomRightIndex];



            Vector minVector = new Vector();
            Vector maxVector = new Vector();

            minVector.X = Math.Max(_min.X, rect_min.X);
            maxVector.X = Math.Min(_max.X, rect_max.X);

            minVector.Y = Math.Max(_min.Y, rect_min.Y);
            maxVector.Y = Math.Min(_max.Y, rect_max.Y);

            return new Rect(minVector, maxVector);
        }

        public override bool Equals(object obj)
        {
            if (obj is Rect rect)
            {
                return rect == this;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int code = 0;

            int x = _vertices.Length;

            for (int i = 0; i < _vertices.Length; i++)
            {
                code += (int)Math.Pow(_vertices[i].GetHashCode(), i + 1);
            }

            code += _center.GetHashCode() * x++;
            code += _angle.GetHashCode() * x;

            return code;
        }

        public static bool operator ==(Rect rect1, Rect rect2)
        {
            bool equal = true;

            for (int i = 0; i < rect1._vertices.Length; i++)
            {
                equal = equal && rect1._vertices[i].Equals(rect2._vertices[i]);
                if (!equal)
                {
                    break;
                }
            }

            return equal;
        }

        public static bool operator !=(Rect rect1, Rect rect2)
        {
            bool equal = true;

            for (int i = 0; i < rect1._vertices.Length; i++)
            {
                equal = equal && rect1._vertices[i].Equals(rect2._vertices[i]);
                if (!equal)
                {
                    break;
                }
            }

            return equal;
        }
    }
}
