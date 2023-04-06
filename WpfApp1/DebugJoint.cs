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
        private JointType _jointType;

        public bool IsVisible
        {
            get => IsVisible; set
            {
                if (IsVisible)
                {
                    _joint.Visibility = Visibility.Visible;
                }
                else
                {
                    _joint.Visibility = Visibility.Hidden;
                }

                IsVisible = value;
            }
        }

        public DebugJoint(MainWindow window, JointType type)
        {
            _joint = new Ellipse
            {
                Margin = new Thickness(10)
            };
            
            _window = new WeakReference<MainWindow>(window);

            _jointType = type;  
            IsVisible = true;

            Canvas canvas = window.canvas;
            canvas.Children.Add(_joint);
        }

        public void DrawDebugJoint(Vector normalizedPosition, Vector size, Color color)
        {
            if (!IsVisible)
            {
                _joint.Width = 0;
                _joint.Height = 0;
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
