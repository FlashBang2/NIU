using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;

namespace WpfApp1
{
    public class DebugJoint
    {
        public double PosX { get => _posX; }
        public double PosY { get => _posY; }

        private double _posX = 0;
        private double _posY = 0;

        public Rect Bounds { get => new Rect(new Vector(PosX, PosY), new Vector(PosX + 0, PosY + 0)); }

        private Vector _offset = new Vector();

        public bool IsVisible
        {
            get => _visible; set
            {
                _visible = value;
            }
        }

        private bool _visible;

        public DebugJoint()
        {
            _visible = true;
        }

        public void DrawDebugJoint(Vector normalizedPosition, Vector size, Vector offset, Color color)
        {
            if (!IsVisible)
            {
                return;
            }

            _posX = ((normalizedPosition.X - size.X / 2) * SDLApp.GetInstance().GetAppWidth() + _offset.X) + offset.X;
            _posY = ((normalizedPosition.Y - size.Y / 2) * SDLApp.GetInstance().GetAppHeight() - _offset.Y) + offset.Y;

            SDLRendering.FillCircle((int)_posX, (int)_posY, (int)2, color);
        }

        public void Clear()
        {
        }

        public void AddOffset(Vector offset)
        {
            _offset += offset;
        }
    }
}
