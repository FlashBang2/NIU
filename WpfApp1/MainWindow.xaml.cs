using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Kinect;
using System.Windows.Controls;

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
        private Label label;

        private bool switch01 = false;
        private bool switch02 = false;

        private DispatcherTimer calibrateY = new DispatcherTimer();
        private DispatcherTimer calibrateX = new DispatcherTimer();
        private DispatcherTimer update = new DispatcherTimer();

        private Skeleton user = null;
        private float MaxY = 0;
        private float MinX = 0;
        private float MaxX = 0;

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
                // temp solution for testing
                string text = "Podnieś ręce";
                label = new Label();
                label.Content = text;
                label.FontSize = 100;
                canvas.Children.Add(label);
                label.SizeChanged += (o, e) =>
                {
                    double left = (Width - label.ActualWidth) / 2;
                    double top = (Height - label.ActualHeight) / 2;

                    Thickness margin = label.Margin;
                    margin.Left = left;
                    margin.Top = top;

                    label.Margin = margin;
                };
                calibrateY.Tick += (sender, evt) => CalibrateY();
                calibrateY.Interval = TimeSpan.FromSeconds(10);
                calibrateY.Start();
                KinectStart();
            }
        }

        private void CalibrateX()
        {
            if (switch02)
            {
                MinX = user.Joints[JointType.HandLeft].Position.X;
                MaxX = user.Joints[JointType.HandRight].Position.X;
                calibrateX.Stop();
                update.Tick += (sender, evt) => Render();
                update.Interval = TimeSpan.FromMilliseconds(16.6);
                ShowCenteredText("");
                update.Start();
            }
            else
            {
                switch02 = true;
                ShowCenteredText("Rozłóż Ręce");
            }
        }

        private void CalibrateY()
        {
            if (switch01)
            {
                MaxY = user.Joints[JointType.Head].Position.Y;
                calibrateY.Stop();
                calibrateX.Tick += (sender, evt) => CalibrateX();
                calibrateX.Interval = TimeSpan.FromSeconds(10);
                calibrateX.Start();
            }
            else
            {
                switch01 = true;
            }
        }

        private void ShowCenteredText(string text)
        {
            label.Content = text;
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
                        user = skeletons.Where(u => u.TrackingState == SkeletonTrackingState.Tracked)
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

                var x = (user.Joints[joint.Key].Position.X + MinX)/(MaxX + MinX);
                var y = user.Joints[joint.Key].Position.Y/MaxY;

                DrawEllipseAtLocation(joint.Key, new Vector(x, y), ellipseSize, boneColor);
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
