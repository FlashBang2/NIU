using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<JointType, Ellipse> ellipses = new Dictionary<JointType, Ellipse>();
        static void SkeletonFrameReady(object sender,
    SkeletonFrameReadyEventArgs args)
        {
            using (var frame = args.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    Skeleton[] skeletons = new Skeleton
                    [frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                    if (skeletons.Length > 0)
                    {
                        var user = skeletons.Where
                        (
                        u => u.TrackingState ==
                        SkeletonTrackingState.Tracked
                        )
                        .FirstOrDefault();
                        if (user != null)
                        {
                            foreach (var joint in user.Joints.ToArray())
                            {

                            }
                        }
                    }
                }
            }
        }

        public void DrawEllipseAtLocation(JointType type, Vector v2, Vector size, Color color)
        {
            Ellipse joint = ellipses[type];

            float normalizedX = (float)v2.X;
            float normalizedY = (float)v2.Y;

            float scaledWidth = (float)(size.X * Width);
            float scaledHeight = (float)(size.Y * Height);

            joint.Width = scaledWidth;
            joint.Height = scaledWidth;

            Thickness margin = joint.Margin;
            margin.Left = normalizedX * Width;
            margin.Top = normalizedY * Height;
            joint.Margin = margin;

            joint.Width = scaledWidth;
            joint.Height = scaledHeight;
            joint.Fill = new SolidColorBrush(color);
        }

        static void KinectStart()
        {
            var kinect = KinectSensor
                    .KinectSensors
                    .FirstOrDefault
                    (s => s.Status == KinectStatus.Connected);
            kinect.SkeletonStream.Enable();
            kinect.SkeletonFrameReady +=
            new EventHandler<SkeletonFrameReadyEventArgs>
            (SkeletonFrameReady);
            kinect.Start();
        }


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
        }
    }
}
