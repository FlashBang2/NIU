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
        private Ellipse _joint;
        private WeakReference<MainWindow> _window;

        public double PosX { get => _joint.Margin.Left; }
        public double PosY { get => _joint.Margin.Top; }

        public Rect Bounds { get => new Rect(new Vector(PosX, PosY), new Vector(PosX + _joint.Width, PosY + _joint.Height)); }

        public bool IsVisible
        {
            get => _visible; set
            {
                if (_visible)
                {
                    _joint.Visibility = Visibility.Visible;
                }
                else
                {
                    _joint.Width = 0;
                    _joint.Height = 0;
                    _joint.Visibility = Visibility.Hidden;
                }

                _visible = value;
            }
        }
        private bool _visible;

        public DebugJoint(MainWindow window)
        {
            _joint = new Ellipse
            {
                Margin = new Thickness(0)
            };

            _window = new WeakReference<MainWindow>(window);

            _visible = true;

            Canvas canvas = window.canvas;
            canvas.Children.Add(_joint);
        }

        public void DrawDebugJoint(Vector normalizedPosition, Vector size, Color color)
        {
            if (!IsVisible)
            {
                return;
            }

            MainWindow w;

            if (!_window.TryGetTarget(out w))
            {
                throw new NullReferenceException();
            }

            double scaledWidth = size.X * w.Width;
            double scaledHeight = size.Y * w.Height;

            _joint.Width = scaledWidth;
            _joint.Height = scaledWidth;

            Thickness margin = _joint.Margin;

            margin.Left = (normalizedPosition.X - size.X / 2) * w.Width;
            margin.Top = (normalizedPosition.Y - size.Y / 2) * w.Height;
            _joint.Margin = margin;

            _joint.Width = scaledWidth;
            _joint.Height = scaledHeight;
            _joint.Fill = new SolidColorBrush(color);
        }

        public void Clear()
        {
            DrawDebugJoint(new Vector(), new Vector(), Color.FromArgb(0, 0, 0, 0));
        }
    }
}
