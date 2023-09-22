using System.Linq;

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
    private System.Collections.Generic.IDictionary<Microsoft.Kinect.JointType, Vector2> _destination = new System.Collections.Generic.Dictionary<Microsoft.Kinect.JointType, Vector2>();
    private System.Collections.Generic.IDictionary<Microsoft.Kinect.JointType, Vector2> _copy = new System.Collections.Generic.Dictionary<Microsoft.Kinect.JointType, Vector2>();
    private bool _isDestinationDictionaryInUsage = false;
    private Microsoft.Kinect.Skeleton[] _skeletons;

    private static Kinnect Instance;
    public static Kinnect GetKinnect()
    {
        return Instance;
    }

    public Vector2 this[Microsoft.Kinect.JointType type]
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
        Microsoft.Kinect.KinectSensor kinect = Microsoft.Kinect.KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == Microsoft.Kinect.KinectStatus.Connected);

        IsKinnectAvailable = kinect != null;

        if (IsKinnectAvailable)
        {
            kinect.SkeletonStream.Enable();
            kinect.SkeletonFrameReady += OnSkeletonFrameReady;
            kinect.Start();
        }

        foreach (var x in System.Enum.GetValues(typeof(Microsoft.Kinect.JointType)).Cast<Microsoft.Kinect.JointType>())
        {
            _destination.Add(x, new Vector2(0, 0));
            _copy.Add(x, new Vector2(0, 0));
        }

        Instance = this;
    }

    private void OnSkeletonFrameReady(object sender, Microsoft.Kinect.SkeletonFrameReadyEventArgs args)
    {
        using (Microsoft.Kinect.SkeletonFrame frame = args.OpenSkeletonFrame())
        {
            bool isSkeletonDataAvailable = frame != null;

            if (isSkeletonDataAvailable)
            {
                CopySkeletonData(frame);
            }
        }
    }

    private void CopySkeletonData(Microsoft.Kinect.SkeletonFrame frame)
    {
        if (HasSkeletonDataChanged(frame))
        {
            _skeletons = new Microsoft.Kinect.Skeleton[frame.SkeletonArrayLength];
        }

        frame.CopySkeletonDataTo(_skeletons);

        if (_skeletons.Length > 0)
        {
            Microsoft.Kinect.Skeleton user = _skeletons.Where(u => u.TrackingState == Microsoft.Kinect.SkeletonTrackingState.Tracked)
                .FirstOrDefault();

            if (user != null)
            {
                UpdateSkeletonData(user);
            }
        }
    }

    private void UpdateSkeletonData(Microsoft.Kinect.Skeleton user)
    {
        foreach (var x in user.Joints.Cast<Microsoft.Kinect.Joint>())
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

    private bool HasSkeletonDataChanged(Microsoft.Kinect.SkeletonFrame frame)
    {
        return _skeletons == null || frame.SkeletonArrayLength != _skeletons.Length;
    }
}