using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    public class DebugSkeleton
    {
        public enum JumpState
        {
            None = 0,
            DuringJump,
            DuringFall
        }

        public Vector JointSize = new Vector(0.01, 0.01);

        public double Scale = 1;
        public double NormalizedMaxY { get => _maxY; }
        public double NormalizedMaxX { get => _maxX; }
        public double NormalizedMinX { get => _minX; }
        public Rect Bounds { get => _bounds; }
        public double PosX
        {
            get => _bounds.Center.X;
            set
            {
            }
        }

        public double PosY
        {
            get => _bounds.Center.Y;
            set
            {
            }
        }

        public Vector Velocity { get => _velocity; set => _velocity = value; }
        public bool ShouldApplyGravity { get => _shouldApplyGravity; set => _shouldApplyGravity = value; }
        public double GravityScale { get => _gravityScale; set => _gravityScale = value; }
        public bool IsVisible
        {
            get => _joints[JointType.Head].IsVisible;
            set
            {
                foreach (var joint in _joints)
                {
                    joint.Value.IsVisible = value;
                }
            }
        }
        public double MaxJumpHeight = 7;
        public bool IsTrigger { get => _isTrigger; set => _isTrigger = value; }

        private readonly Dictionary<JointType, Vector> _jointLocations = new Dictionary<JointType, Vector>();
        private readonly Dictionary<JointType, Vector> _tempJointLocations = new Dictionary<JointType, Vector>();
        private readonly Dictionary<JointType, DebugJoint> _joints = new Dictionary<JointType, DebugJoint>();
        private readonly Connection[] _connections;

        private double _maxY = 0;
        private double _minX = 0;
        private double _maxX = 0;

        private Rect _bounds;


        private bool _isTrigger;
        private bool _isStatic;

        private Vector _velocity = new Vector();

        public Vector offset = new Vector();
        private bool _shouldApplyGravity = true;
        private double _gravityScale = 1;

        public bool IsFalling { get => _jumpState != JumpState.None; }
        private JumpState _jumpState = JumpState.None;

        public DebugSkeleton()
        {
            _connections = Connection.Connections;
            AddJoints();
            _bounds = new Rect(new Vector(), new Vector());

            foreach (var x in Enum.GetValues(typeof(JointType)).Cast<JointType>())
            {
                _jointLocations[x] = new Vector();
                _tempJointLocations[x] = new Vector();
            }
        }

        private void AddJoints()
        {
            IEnumerable<JointType> joints = Enum.GetValues(typeof(JointType)).Cast<JointType>();
            foreach (JointType joint in joints)
            {
                _joints.Add(joint, new DebugJoint());
            }
        }

        public void LoadTestSkeleton()
        {
            string[] lines = File.ReadAllLines("skeleton.txt");

            int lineNo = 0;
            string jointTag = "";

            int numElements = 0;

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

                    //pos = pos.Select(v => v.Replace(',', '.')).ToArray();

                    double x = double.Parse(pos[0]);
                    double y = -double.Parse(pos[1]);

                    foreach (var joint in _jointLocations.Keys)
                    {
                        if (joint.ToString().Equals(jointTag))
                        {
                            _tempJointLocations[joint] = (new Vector(x, y) + _tempJointLocations[joint]) / 2;
                            _jointLocations[joint] = _tempJointLocations[joint];

                            break;
                        }
                    }
                    numElements++;
                }

                lineNo++;
            }

            this.FindSkeletonBounds();
        }

        public Vector this[JointType type]
        {
            get { return _tempJointLocations[type]; }
        }

        public void UpdateSkeleton(SkeletonFrame frame)
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
                        _jointLocations[x.JointType] = new Vector(x.Position.X, x.Position.Y);
                    }
                }
            }
        }

        public void EndOfYCalibration()
        {
            _maxY = -_jointLocations[JointType.HandRight].Y;
            Console.WriteLine("MaxY: " + _maxY);
        }

        public void EndOfXCalibration()
        {
            _minX = -_jointLocations[JointType.HandLeft].X;
            _maxX = -_jointLocations[JointType.HandRight].X;

            Console.WriteLine("MinX: " + _minX);
            Console.WriteLine("MaxX: " + _maxX);
        }

        static public double Map(double value, double istart, double istop, double ostart, double ostop)
        {
            return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
        }

        public void RenderEachJoint()
        {
            Color boneColor = Color.FromArgb(255, 0, 0, 0);

            foreach (KeyValuePair<JointType, DebugJoint> joint in _joints)
            {
                CalculateScaledBounds(Scale, joint);

                DebugJoint j = joint.Value;
                j.DrawDebugJoint(_tempJointLocations[joint.Key], Scale * JointSize, offset, boneColor);
            }

            FindSkeletonBounds();

            foreach (Connection connection in _connections)
            {
                connection.DrawConnection(_joints);
            }
        }

        private void FindSkeletonBounds()
        {
            double minX = double.PositiveInfinity;
            double minY = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double maxY = double.NegativeInfinity;

            foreach (var i in _joints)
            {
                Rect bounds = i.Value.Bounds;
                minX = Math.Min(minX, bounds.Left);
                minY = Math.Min(minY, bounds.Top);

                maxX = Math.Max(maxX, bounds.Right);
                maxY = Math.Max(maxY, bounds.Down);
            }

            _bounds = new Rect(new Vector(minX, minY), new Vector(maxX, maxY));
        }

        private void CalculateScaledBounds(double scale, KeyValuePair<JointType, DebugJoint> joint)
        {
            double x = Map((_jointLocations[joint.Key].X) + 1.5 / scale * _maxX, -1, 1, 0, 1);
            double y = Map((-_jointLocations[joint.Key].Y) - _maxY, -1, 1, 0, 1);

            _tempJointLocations[joint.Key] = new Vector(x, y);
        }

        public void Clear()
        {
            foreach (KeyValuePair<JointType, DebugJoint> joint in _joints)
            {
                joint.Value.Clear();
            }

            foreach (Connection connection in _connections)
            {
                connection.ClearConnectionLine();
            }
        }

        private bool isInFloor = false;

        public void PhysicsUpdate()
        {

            UpdateJumpState();
        }

        private void UpdateJumpState()
        {
            if (Velocity.Y > 0)
            {
                if (_jumpState == JumpState.None)
                {
                    _jumpState = JumpState.DuringJump;
                }
            }
            else if (Velocity.Y < 0)
            {
                if (_jumpState == JumpState.DuringJump || _jumpState == JumpState.None)
                {
                    _jumpState = JumpState.DuringFall;
                }
            }
            else
            {
                if (_jumpState == JumpState.DuringFall)
                {
                    _jumpState = JumpState.None;
                }
            }
        }

        public void AddOffset(Vector offset)
        {
            MoveByOffsetEachChild(offset);
        }

        void MoveByOffsetEachChild(Vector offset)
        {
            foreach (var joint in _joints)
            {
                joint.Value.AddOffset(new Vector(0, offset.Y));
                joint.Value.DrawDebugJoint(_tempJointLocations[joint.Key], JointSize, this.offset, Color.FromRgb(0, 0, 0));
            }

            FindSkeletonBounds();
        }

        public void Jump()
        {
            Velocity = new Vector(Velocity.X, MaxJumpHeight);
        }
    }
}
