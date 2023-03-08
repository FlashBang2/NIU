using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
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
        private Timer timer;

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

            DrawEllipseAtLocation(JointType.Head, new Vector(0.3f, 0.3f), new Vector(0.1f, 0.21f), Color.FromArgb(255, 0, 0, 0));
            DrawEllipseAtLocation(JointType.HandLeft, new Vector(0.3f, 0.2f), new Vector(0.1f, 0.1f), Color.FromArgb(255, 0, 0, 0));

            if (IsKinnectAvailable)
            {
                KinectStart();
            }
            else
            {

                // temp solution for testing
                var dispatcher = new DispatcherTimer();
                dispatcher.Tick += (sender, evt) => OnRender();
                dispatcher.Interval = TimeSpan.FromMilliseconds(16.6);
                dispatcher.Start();
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
                if (frame != null)
                {
                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);

                    if (skeletons.Length > 0)
                    {
                        Skeleton user = skeletons.Where(u => u.TrackingState == SkeletonTrackingState.Tracked)
                            .FirstOrDefault();
                        
                        if (user != null)
                        {
                            OnRender();
                        }
                    }
                }
            }
        }

        private void OnRender()
        {
            ClearScreen();
            RenderEachJoint();
        }

        private void RenderEachJoint()
        {
            Vector ellipseSize = new Vector(0.2f, 0.2f);

            foreach (KeyValuePair<JointType, Ellipse> joint in ellipses)
            {
                Vector normalizedPosition = new Vector(0.5f, 0.5f);
                DrawEllipseAtLocation(joint.Key, normalizedPosition, ellipseSize, Color.FromArgb(255, 0, 0, 0));
            }
        }

        private void ClearScreen()
        {
            Vector ellipseSize = new Vector(0.2f, 0.2f);
            foreach (KeyValuePair<JointType, Ellipse> joint in ellipses)
            {
                Vector normalizedPosition = new Vector(0.5f, 0.5f);
                DrawEllipseAtLocation(joint.Key, normalizedPosition, ellipseSize, Color.FromArgb(0, 0, 0, 0));
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
