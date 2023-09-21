using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario.SourceCode
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public class Kinnect
    {
        public static bool IsKinnectAvailable;
        private IDictionary<JointType, Vector2> _destination = new Dictionary<JointType, Vector2>();
        private IDictionary<JointType, Vector2> _copy = new Dictionary<JointType, Vector2>();
        private bool _isDestinationDictionaryInUsage = false;
        private Skeleton[] _skeletons;

        private static Kinnect Instance;
        public static Kinnect GetKinnect()
        {
            return Instance;
        }

        public Vector2 this[JointType type]
        {
            get
            {
                if (_isDestinationDictionaryInUsage)
                {
                    return _copy[type];
                }

                return _destination[type];
            }
        }

        public Kinnect()
        {
            KinectSensor kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

            IsKinnectAvailable = kinect != null;

            if (IsKinnectAvailable)
            {
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += OnSkeletonFrameReady;
                kinect.Start();
            }

            foreach (var x in Enum.GetValues(typeof(JointType)).Cast<JointType>())
            {
                _destination.Add(x, new Vector2(0, 0));
                _copy.Add(x, new Vector2(0, 0));
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
                    CopySkeletonData(frame);
                }
            }
        }

        private void CopySkeletonData(SkeletonFrame frame)
        {
            if (HasSkeletonDataChanged(frame))
            {
                _skeletons = new Skeleton[frame.SkeletonArrayLength];
            }

            frame.CopySkeletonDataTo(_skeletons);

            if (_skeletons.Length > 0)
            {
                Skeleton user = _skeletons.Where(u => u.TrackingState == SkeletonTrackingState.Tracked)
                    .FirstOrDefault();

                if (user != null)
                {
                    UpdateSkeletonData(user);
                }
            }
        }

        private void UpdateSkeletonData(Skeleton user)
        {
            foreach (var x in user.Joints.Cast<Joint>())
            {
                if (_isDestinationDictionaryInUsage)
                {
                    _destination[x.JointType] = new Vector2(x.Position.X, x.Position.Y);
                }
                else
                {
                    _copy[x.JointType] = new Vector2(x.Position.X, x.Position.Y);
                }
            }

            // switch buffers
            _isDestinationDictionaryInUsage = !_isDestinationDictionaryInUsage;
        }

        private bool HasSkeletonDataChanged(SkeletonFrame frame)
        {
            return _skeletons == null || frame.SkeletonArrayLength != _skeletons.Length;
        }
    }
}
