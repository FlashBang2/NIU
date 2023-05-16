using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static SDL2.SDL;

namespace WpfApp1
{
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

        public double Width => (_vertices[TopLeftIndex] + _vertices[TopRightIndex]).Length;
        public double Height => (_vertices[TopLeftIndex] + _vertices[BottomLeftIndex]).Length;

        public double Left => _vertices[TopLeftIndex].X;
        public double Top => _vertices[TopLeftIndex].Y;
        public double Right => _vertices[TopRightIndex].X;
        public double Down => _vertices[BottomLeftIndex].Y;

        public Vector Extend => new Vector(Width, Height) / 2;
        public Vector Center => _center;

        public static readonly Rect Unlimited = new Rect(new Vector(int.MinValue, int.MinValue), new Vector(int.MaxValue, int.MaxValue));
        public double Angle => _angle;

        public SDL_Rect AsSDLRect
        {
            get
            {
                if (this == Unlimited)
                {
                    return default;
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

                separation = Math.Min(separation, minSep);
            }

            return separation <= 0;
        }

        public static Vector PendicularVector(Vector v)
        {
            return new Vector(v.Y, -v.X);
        }

        public bool IsOverlaping(Vector point)
        {
            double separation = double.MinValue;

            foreach (Vector vertexA in _vertices)
            {
                Vector normal = PendicularVector(vertexA);
                double minSep = double.MaxValue;

                minSep = Math.Min(minSep, (point - vertexA) * normal);
                separation = Math.Min(separation, minSep);
            }

            return separation <= 0;
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
