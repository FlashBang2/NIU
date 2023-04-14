using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Kinect;
using System.Windows.Controls;
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<JointType, DebugJoint> joints = new Dictionary<JointType, DebugJoint>();
        private bool IsKinnectAvailable = false;

        private Label label;
        Dictionary<JointType, Vector> JointLocations = new Dictionary<JointType, Vector>();
        Dictionary<JointType, Vector> TempJointLocations = new Dictionary<JointType, Vector>();
        private Connection[] Connections;

        private bool switch01 = false;
        private bool switch02 = false;

        private DispatcherTimer calibrateY = new DispatcherTimer();
        private DispatcherTimer calibrateX = new DispatcherTimer();
        private DispatcherTimer update = new DispatcherTimer();

        private Skeleton user = null;
        private double MaxY = 0;
        private double MinX = 0;
        private double MaxX = 0;

        private Vector offset = new Vector(-2.46, 0.25);
        private ActionType actionType = ActionType.None;

        public MainWindow()
        {
            InitializeComponent();
            AddJoints();

            Connections = Connection.Connections;

            foreach (Connection connection in Connections)
            {
                connection.AttachToCanvas(canvas);
            }

            foreach (var x in Enum.GetValues(typeof(JointType)).Cast<JointType>())
            {
                JointLocations[x] = new Vector();
                TempJointLocations[x] = new Vector();
            }

            StartKinect();

            if (!IsKinnectAvailable)
            {
                LoadTestSkeleton();
                StartupCalibration();
            }
        }

        private void LoadTestSkeleton()
        {
            string[] lines = File.ReadAllLines("skeleton.txt");

            int lineNo = 0;
            string jointTag = "";

            foreach (string line in lines)
            {
                bool isJointTag = lineNo % 2 == 0;
                if (isJointTag)
                {
                    jointTag = line;
                }
                else
                {
                    string[] pos = line.Split(';');

                    pos = pos.Select(v => v.Replace(',', '.')).ToArray();

                    double x = double.Parse(pos[0]);
                    double y = -double.Parse(pos[1]);

                    foreach (var joint in JointLocations.Keys)
                    {
                        if (joint.ToString().Equals(jointTag))
                        {
                            TempJointLocations[joint] = (new Vector(x, y) + TempJointLocations[joint]) / 2;
                            JointLocations[joint] = TempJointLocations[joint];

                            break;
                        }
                    }
                }

                lineNo++;
            }

            foreach (var i in joints)
            {
                i.Value.IsVisible = true;
            }

            int j = 0;

        }

        private void AddJoints()
        {
            IEnumerable<JointType> joints = Enum.GetValues(typeof(JointType)).Cast<JointType>();
            foreach (JointType joint in joints)
            {
                this.joints.Add(joint, new DebugJoint(this, joint));
            }
        }

        private void StartupCalibration()
        {
            string text = "Podnieś ręce";

            label = new Label();
            label.Content = text;
            label.FontSize = 100;
            canvas.Children.Add(label);
            label.SizeChanged += RecenterText;

            calibrateY.Tick += (sender, evt) => CalibrateY();
            calibrateY.Interval = TimeSpan.FromSeconds(2);
            calibrateY.Start();
        }

        private void RecenterText(object sender, SizeChangedEventArgs e)
        {
            double left = (Width - label.ActualWidth) / 2;
            double top = (Height - label.ActualHeight) / 2;

            Thickness margin = label.Margin;
            margin.Left = left;
            margin.Top = top;

            label.Margin = margin;
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

        private void StartKinect()
        {
            KinectSensor kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

            IsKinnectAvailable = kinect != null;

            if (IsKinnectAvailable)
            {
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += OnSkeletonFrameReady;
                kinect.Start();
            }
        }

        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs args)
        {
            using (SkeletonFrame frame = args.OpenSkeletonFrame())
            {
                bool isSkeletonDataAvailable = frame != null;

                if (isSkeletonDataAvailable)
                {
                    UpdateJointLocations(frame);
                }
            }
        }

        private void UpdateJointLocations(SkeletonFrame frame)
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

        private void Render()
        {
            ClearCanvas();
            RenderEachJoint();
            UpdateGame();
        }

        private void UpdateGame()
        {
            foreach (FrameworkElement child in canvas.Children.Cast<FrameworkElement>())
            {
                if (IsPartOfSkeleton(child))
                {
                    continue;
                }

                Thickness worldLocation = child.Margin;

                switch (actionType)
                {
                    case ActionType.MoveLeft:
                        worldLocation.Left += 10;
                        break;
                    case ActionType.MoveRight:
                        worldLocation.Left -= 10;
                        break;
                }

                child.Margin = worldLocation;
            }
        }

        private static bool IsPartOfSkeleton(FrameworkElement ch)
        {
            bool isBone = ch.GetType().Equals(typeof(Line));
            bool isJoint = ch.GetType().Equals(typeof(Ellipse));

            return isBone || isJoint;
        }

        private void RenderEachJoint()
        {
            Vector jointSize = new Vector(0.01f, 0.01f);
            Color boneColor = Color.FromArgb(255, 0, 0, 0);
            double scale = 0.15f;

            foreach (KeyValuePair<JointType, DebugJoint> joint in joints)
            {
                double x = scale * (JointLocations[joint.Key].X) + 1.5 / (MaxX + 3);
                double y = scale * (-JointLocations[joint.Key].Y) + 2 / (MaxY + 3);

                TempJointLocations[joint.Key] = new Vector(x, y);

                DebugJoint j = joint.Value;
                j.DrawDebugJoint(TempJointLocations[joint.Key], jointSize, boneColor);
            }

            foreach (Connection connection in Connections)
            {
                connection.DrawConnection(TempJointLocations, this);
            }

            actionType = FindActionType();
        }

        private ActionType FindActionType()
        {
            ActionType actionType = ActionType.None;

            Vector ankleLeft = TempJointLocations[JointType.AnkleLeft];
            Vector kneeRight = TempJointLocations[JointType.KneeRight];
            Vector ankleRight = TempJointLocations[JointType.AnkleRight];
            Vector kneeLeft = TempJointLocations[JointType.KneeLeft];

            if (-ankleLeft.Y > -kneeRight.Y)
            {
                actionType = ActionType.MoveRight;
            }
            else if (-ankleRight.Y > -kneeLeft.Y)
            {
                actionType = ActionType.MoveLeft;
            }

            return actionType;
        }

        private void ClearCanvas()
        {
            foreach (KeyValuePair<JointType, DebugJoint> joint in joints)
            {
                joint.Value.Clear();
            }

            foreach (Connection connection in Connections)
            {
                connection.ClearConnectionLine();
            }
        }
    }
}
