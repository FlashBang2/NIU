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
        private bool IsKinnectAvailable = true;

        private Label label;
        Dictionary<JointType, Vector> JointLocations = new Dictionary<JointType, Vector>();
        Dictionary<JointType, Vector> TempJointLocations = new Dictionary<JointType, Vector>();

        private Connection[] Connections =
        {
            new Connection(JointType.Head, JointType.ShoulderCenter),

            new Connection(JointType.ShoulderCenter, JointType.ShoulderRight),
            new Connection(JointType.ShoulderRight, JointType.ElbowRight),
            new Connection(JointType.ElbowRight, JointType.WristRight),
            new Connection(JointType.WristRight, JointType.HandRight),

            new Connection(JointType.ShoulderCenter, JointType.ShoulderLeft),
            new Connection(JointType.ShoulderLeft, JointType.ElbowLeft),
            new Connection(JointType.ElbowLeft, JointType.WristLeft),
            new Connection(JointType.WristLeft, JointType.HandLeft),

            new Connection(JointType.ShoulderCenter, JointType.Spine),
            new Connection(JointType.Spine, JointType.HipCenter),

            new Connection(JointType.HipCenter, JointType.HipRight),
            new Connection(JointType.HipRight, JointType.KneeRight),
            new Connection(JointType.KneeRight, JointType.AnkleRight),
            new Connection(JointType.AnkleRight, JointType.FootRight),

            new Connection(JointType.HipCenter, JointType.HipLeft),
            new Connection(JointType.HipLeft, JointType.KneeLeft),
            new Connection(JointType.KneeLeft, JointType.AnkleLeft),
            new Connection(JointType.AnkleLeft, JointType.FootLeft),
        };

        private bool switch01 = false;
        private bool switch02 = false;

        private DispatcherTimer calibrateY = new DispatcherTimer();
        private DispatcherTimer calibrateX = new DispatcherTimer();
        private DispatcherTimer update = new DispatcherTimer();

        private Skeleton user = null;
        private double MaxY = 0;
        private double MinX = 0;
        private double MaxX = 0;

        private Vector offset = new Vector(0.0, 0.25);

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

            foreach (Connection connection in Connections)
            {
                connection.AttachToCanvas(canvas);
            }

            if (IsKinnectAvailable)
            {

                foreach (var x in Enum.GetValues(typeof(JointType)).Cast<JointType>())
                {
                    JointLocations[x] = new Vector();
                }
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
            }
            else
            {
                foreach (var x in Enum.GetValues(typeof(JointType)).Cast<JointType>())
                {
                    JointLocations[x] = new Vector();
                    TempJointLocations[x] = new Vector();
                }

                foreach (Connection connection in Connections)
                {
                    Console.WriteLine(connection);
                }
            }
        }

        private void CalibrateX()
        {
            if (switch02)
            {
                MinX = -JointLocations[JointType.HandLeft].X;
                MaxX = -JointLocations[JointType.HandRight].X;
                Console.WriteLine("MinX: " + MinX);
                Console.WriteLine("MaxX: " + MaxX);
                calibrateX.Stop();
                update.Tick += (sender, evt) => Render();
                update.Interval = TimeSpan.FromMilliseconds(16.6);
                ShowCenteredText("");
                update.Start();
                rectangle.Visibility = Visibility.Visible;
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
                MaxY = -JointLocations[JointType.HandRight].Y;
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
                                JointLocations[x.JointType] = new Vector(x.Position.X, x.Position.Y);
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
            foreach (Connection connection in Connections)
            {
                connection.DrawConnection(TempJointLocations, MaxX, MaxY, this);
            }
        }

        private void RenderEachJoint()
        {
            Vector ellipseSize = new Vector(0.01f, 0.01f);
            Color boneColor = Color.FromArgb(255, 0, 0, 0);
            double scale = 0.25f;

            foreach (KeyValuePair<JointType, Ellipse> joint in ellipses)
            {

                var x = scale * (JointLocations[joint.Key].X + offset.X) + 1.5 / (MaxX + 3);
                var y = scale * (-JointLocations[joint.Key].Y + offset.Y) + 2 / (MaxY + 3);
                Console.WriteLine("x=" + x);

                TempJointLocations[joint.Key] = new Vector(x, y);
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

            foreach (Connection connection in Connections)
            {
                connection.ClearConnectionLine();
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
