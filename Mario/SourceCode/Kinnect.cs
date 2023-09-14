using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario.SourceCode
{
    public class Kinnect
    {
        public static  bool _isKinnectAvailable;
        private IDictionary<JointType, Tuple<float, float>> _destination = new Dictionary<JointType, Tuple<float, float>>();
        private IDictionary<JointType, Tuple<float, float>> _copy = new Dictionary<JointType, Tuple<float, float>>();
        private bool _useDestination = false;

        private static Kinnect Instance;
        public static Kinnect GetKinnect()
        {
            return Instance;
        }

        public bool IsKinnectAvailable { get => _isKinnectAvailable; }

        public Tuple<float, float> this[JointType type]
        {
            get
            {
                if (_useDestination)
                {
                    return _copy[type];
                }

                return _destination[type];
            }
        }

        public Kinnect()
        {
            KinectSensor kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

            _isKinnectAvailable = kinect != null;

            if (_isKinnectAvailable)
            {
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += OnSkeletonFrameReady;
                kinect.Start();
            }

            foreach (var x in Enum.GetValues(typeof(JointType)).Cast<JointType>())
            {
                _destination.Add(x, new Tuple<float, float>(0, 0));
                _copy.Add(x, new Tuple<float, float>(0, 0));
            }

            Instance = this;
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
                            foreach (var x in user.Joints.Cast<Joint>())
                            {
                                if (_useDestination)
                                {
                                    _destination[x.JointType] = new Tuple<float, float>(x.Position.X, x.Position.Y);
                                }
                                else
                                {
                                    _copy[x.JointType] = new Tuple<float, float>(x.Position.X, x.Position.Y);
                                }
                            }

                            _useDestination = !_useDestination;
                        }
                    }
                }
            }
        }

    }
}
