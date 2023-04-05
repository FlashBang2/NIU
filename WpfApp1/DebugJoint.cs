using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class DebugJoint
    {
        private Ellipse _joint;
        private MainWindow _window;
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
            
            _window = window;

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

            double scaledWidth = size.X * _window.Width;
            double scaledHeight = size.Y * _window.Height;

            _joint.Width = scaledWidth;
            _joint.Height = scaledWidth;

            Thickness margin = _joint.Margin;

            margin.Left = (normalizedPosition.X - size.X / 2) * _window.Width;
            margin.Top = (normalizedPosition.Y - size.Y / 2) * _window.Height;
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
