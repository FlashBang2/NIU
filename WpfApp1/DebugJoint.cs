using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;
using static SDL2.SDL;

namespace WpfApp1
{
    public class DebugJoint
    {
        public double PosX { get => _posX; }
        public double PosY { get => _posY; }

        private double _posX = 0;
        private double _posY = 0;

        public SDL_Rect Bounds { get { SDL_Rect r = new SDL_Rect(); SdlRectMath.FromXywh((float)_posX, (float)_posY, 0, 0, out r); return r; } }

        private float _offsetX = 0;
        private float _offsetY = 0;

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

            _posX = ((normalizedPosition.X - size.X / 2) * SDLApp.GetInstance().GetAppWidth() + _offsetX) + offset.X;
            _posY = ((normalizedPosition.Y - size.Y / 2) * SDLApp.GetInstance().GetAppHeight() - _offsetY) + offset.Y;

            SDLRendering.FillCircle((int)_posX, (int)_posY, (int)2, color);
        }

        public void Clear()
        {
        }

        public void AddOffset(Vector offset)
        {
            _offsetX += (float)offset.X;
            _offsetY += (float)offset.Y;
        }
    }
}
