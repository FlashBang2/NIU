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
        private bool IsKinnectAvailable = false;
        private Label label;

        private bool switch01 = false;
        private bool switch02 = false;

        private DispatcherTimer calibrateY = new DispatcherTimer();
        private DispatcherTimer calibrateX = new DispatcherTimer();
        private DispatcherTimer update = new DispatcherTimer();

        private Skeleton user = null;
        DebugSkeleton skeleton;
        private ActionType actionType = ActionType.None;

        public MainWindow()
        {
            InitializeComponent();
            skeleton = new DebugSkeleton(canvas, this);

            StartKinect();

            if (!IsKinnectAvailable)
            {
                skeleton.LoadTestSkeleton();
                StartupCalibration();
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
                skeleton.EndOfXCalibration();

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
                skeleton.EndOfYCalibration();
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
                    skeleton.UpdateSkeleton(frame);
                }
            }
        }

        private void Render()
        {
            ClearCanvas();
            skeleton.RenderEachJoint();
            FindActionType();
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

        private ActionType FindActionType()
        {
            ActionType actionType = ActionType.None;

            Vector ankleLeft = skeleton[JointType.AnkleLeft];
            Vector kneeRight = skeleton[JointType.KneeRight];
            Vector ankleRight = skeleton[JointType.AnkleRight];
            Vector kneeLeft = skeleton[JointType.KneeLeft];

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
            skeleton.Clear();
        }
    }
}
