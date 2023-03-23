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
        private double MaxY = 0;
        private double MinX = 0;
        private double MaxX = 0;

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
                KinectStart();
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
                calibrateY.Interval = TimeSpan.FromSeconds(2);
                calibrateY.Start();

                foreach (var x in Enum.GetValues(typeof(JointType)).Cast<JointType>())
                {
                    pos[x] = new Vector();
                }
            }
        }

        Dictionary<JointType, Vector> pos = new Dictionary<JointType, Vector>();

        private void CalibrateX()
        {
            if (switch02)
            {
                MinX = -pos[JointType.HandLeft].X;
                MaxX = -pos[JointType.HandRight].X;
                Console.WriteLine("MinX: " + MinX);
                Console.WriteLine("MaxX: " + MaxX);
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
                MaxY = -pos[JointType.HandRight].Y;
                Console.WriteLine("MaxY: " + MaxY);
                calibrateY.Stop();
                calibrateX.Tick += (sender, evt) => CalibrateX();
                calibrateX.Interval = TimeSpan.FromSeconds(2);
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
                            foreach (var x in user.Joints.Cast<Joint>())
                            {
                                pos[x.JointType] = new Vector(x.Position.X, x.Position.Y);
                            }
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

        private void RenderEachJoint()
        {
            Vector ellipseSize = new Vector(0.01f, 0.01f);
            Color boneColor = Color.FromArgb(255, 0, 0, 0);

            foreach (KeyValuePair<JointType, Ellipse> joint in ellipses)
            {

                var x = (-pos[joint.Key].X)/(MaxX + 1.5);
                var y = (-pos[joint.Key].Y)/(MaxY + 1.5);
                Console.WriteLine("x=" + x);

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
            Console.WriteLine(normalizedPosition);

            Ellipse joint = ellipses[type];

            float scaledWidth = (float)(size.X * Width);
            float scaledHeight = (float)(size.Y * Height);

            joint.Width = scaledWidth;
            joint.Height = scaledWidth;

            Thickness margin = joint.Margin;

            margin.Left = (normalizedPosition.X - size.X / 2) * Width;
            margin.Top = (normalizedPosition.Y - size.Y / 2) * Height;
            joint.Margin = margin;

            joint.Width = scaledWidth;
            joint.Height = scaledHeight;
            joint.Fill = new SolidColorBrush(color);
        }
    }
}
