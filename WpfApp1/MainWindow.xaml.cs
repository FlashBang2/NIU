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
using System.Windows.Input;

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

            var body = new WpfPhysicsBody(rectangle);
            body.IsStatic = true;

            body = new WpfPhysicsBody(obstacle);
            body.IsStatic = true;
            
            body = new WpfPhysicsBody(obstacle2);
            body.IsStatic = true;

            if (!IsKinnectAvailable)
            {
                skeleton.LoadTestSkeleton();
                StartupCalibration();
                KeyDown += OnKeyDown;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                skeleton.Jump();
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
            actionType = FindActionType();
            UpdateGame();
        }

        private void UpdateGame()
        {
            switch (actionType)
            {
                case ActionType.MoveLeft:
                    skeleton.Velocity = new Vector(10, skeleton.Velocity.Y);
                    break;
                case ActionType.MoveRight:
                    skeleton.Velocity = new Vector(-10, skeleton.Velocity.Y);
                    break;
            }


            Physics.Update();
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

            if (!IsKinnectAvailable)
            {
                if (Keyboard.IsKeyDown(Key.D))
                {
                    actionType = ActionType.MoveRight;
                }
                else if (Keyboard.IsKeyDown(Key.A))
                {
                    actionType = ActionType.MoveLeft;
                }
                else
                {
                    actionType = ActionType.None;
                }
            }

            return actionType;
        }

        private void ClearCanvas()
        {
            skeleton.Clear();
        }
    }
}
