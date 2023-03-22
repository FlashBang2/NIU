using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Threading;
using Microsoft.Kinect;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<JointType, Ellipse> ellipses = new Dictionary<JointType, Ellipse>();
        private bool IsKinnectAvailable = false;

        private float t = 0;
        private Vector startVector = new Vector();
        private Vector endVector = new Vector(1.0f, 1.0f);

        public MainWindow()
        {
            InitializeComponent();
            IEnumerable<JointType> joints = Enum.GetValues(typeof(JointType)).Cast<JointType>();

            foreach (JointType joint in joints)
            {
                Ellipse ellipse = new Ellipse
                {
                    Margin = new Thickness(10)
                };

                ellipses.Add(joint, ellipse);
                canvas.Children.Add(ellipse);
            }

            if (IsKinnectAvailable)
            {
                KinectStart();
            }
            else
            {
#if false
                // temp solution for testing
                var dispatcher = new DispatcherTimer();
                dispatcher.Tick += (sender, evt) => Render();
                dispatcher.Interval = TimeSpan.FromMilliseconds(16.6);
                dispatcher.Start();
#else
                string text = "Podnieś ręce";
                Label label = new Label();
                label.Content = text;
                label.FontSize = 100;
                canvas.Children.Add(label);
                Measure(new Size(Width, Height));
                Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));
                
                Loaded += (o, e) =>
                {
                    double left = (Width - label.ActualWidth) / 2;
                    double top = (Height - label.ActualHeight) / 2;

                    Thickness margin = label.Margin;
                    margin.Left = left;
                    margin.Top = top;

                    label.Margin = margin;
                };
#endif 
            }
        }

        private void KinectStart()
        {
            KinectSensor kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

            kinect.SkeletonStream.Enable();
            kinect.SkeletonFrameReady += OnSkeletonFrameReady;
            kinect.Start();
        }

        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs args)
        {
            using (SkeletonFrame frame = args.OpenSkeletonFrame())
            {
                bool isSkeletonDataAvailable = frame != null;

                if (isSkeletonDataAvailable)
                {
                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);

                    if (skeletons.Length > 0)
                    {
                        Skeleton user = skeletons.Where(u => u.TrackingState == SkeletonTrackingState.Tracked)
                            .FirstOrDefault();

                        if (user != null)
                        {
                            RenderEachJoint();
                        }
                    }
                }
            }
        }

        private void Render()
        {
            ClearCanvas();
            RenderEachJoint();
        }

        private Vector Lerp(Vector start, Vector end, float t)
        {
            return (1 - t) * start + t * end;
        }

        private void RenderEachJoint()
        {
            Vector ellipseSize = new Vector(0.1f, 0.1f);
            Color boneColor = Color.FromArgb(255, 0, 0, 0);

            foreach (KeyValuePair<JointType, Ellipse> joint in ellipses)
            {
                Vector normalizedPosition = Lerp(startVector, endVector, t);

                t += 0.0016f / 2;

                if (t >= 1)
                {
                    t = 0;
                    (endVector, startVector) = (startVector, endVector);
                }

                DrawEllipseAtLocation(joint.Key, normalizedPosition, ellipseSize, boneColor);
            }
        }

        private void ClearCanvas()
        {
            Vector ellipseSize = new Vector(0.1f, 0.1f);

            foreach (KeyValuePair<JointType, Ellipse> joint in ellipses)
            {
                DrawEllipseAtLocation(joint.Key, new Vector(), ellipseSize, Color.FromArgb(0, 0, 0, 0));
            }
        }

        private void DrawEllipseAtLocation(JointType type, Vector normalizedPosition, Vector size, Color color)
        {
            Ellipse joint = ellipses[type];

            float scaledWidth = (float)(size.X * Width);
            float scaledHeight = (float)(size.Y * Height);

            joint.Width = scaledWidth;
            joint.Height = scaledWidth;

            Thickness margin = joint.Margin;
            margin.Right = 0;
            margin.Bottom = 0;

            margin.Left = (normalizedPosition.X - size.X / 2) * Width;
            margin.Top = (normalizedPosition.Y - size.Y / 2) * Height;
            joint.Margin = margin;

            joint.Width = scaledWidth;
            joint.Height = scaledHeight;
            joint.Fill = new SolidColorBrush(color);
        }
    }
}
