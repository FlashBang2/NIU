using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using static SDL2.SDL;

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

        public Vector jointSize = new Vector(0.01, 0.01);

        public double scale = 1;
        public double normalizedMaxY { get => _maxY; }
        public double normalizedMaxX { get => _maxX; }
        public double normalizedMinX { get => _minX; }
        public SDL_Rect bounds { get => _bounds; }
        public double posX
        {
            get => _bounds.x;
        }

        public double posY
        {
            get => _bounds.y;
        }

        public Vector velocity { get => _velocity; set => _velocity = value; }
        public bool isVisible
        {
            get => _joints[JointType.Head].isVisible;
            set
            {
                foreach (var joint in _joints)
                {
                    joint.Value.isVisible = value;
                }
            }
        }
        public double MaxJumpHeight = 7;
        public bool isTrigger { get => _isTrigger; set => _isTrigger = value; }
        public Vector offset = new Vector();
        public bool isFalling { get => _jumpState != JumpState.None; }

        private readonly Dictionary<JointType, Vector> _jointLocations = new Dictionary<JointType, Vector>();
        private readonly Dictionary<JointType, Vector> _tempJointLocations = new Dictionary<JointType, Vector>();
        private readonly Dictionary<JointType, DebugJoint> _joints = new Dictionary<JointType, DebugJoint>();

        private readonly Connection[] _connections;
        private double _maxY = 0;
        private double _minX = 0;

        private double _maxX = 0;
        private SDL_Rect _bounds;
        private bool _isTrigger;

        private Vector _velocity = new Vector();
        private bool _shouldApplyGravity = true;
        private double _gravityScale = 1;

        private JumpState _jumpState = JumpState.None;

        public DebugSkeleton()
        {
            _connections = Connection.Connections;
            AddJoints();
            _bounds = new SDL_Rect();

            foreach (var x in Enum.GetValues(typeof(JointType)).Cast<JointType>())
            {
                _jointLocations[x] = new Vector();
                _tempJointLocations[x] = new Vector();
            }

            isVisible = false;
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
                    NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;

                    pos = pos.Select(v => v.Replace(",", nfi.NumberDecimalSeparator)).ToArray();
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

            SDLApp.GetInstance().canStartGoomba = true;

            //Entity.GetEntity("goomba", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba2", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba3", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba4", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba5", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba6", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba7", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba8", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba9", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba10", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba11", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba12", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba13", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba14", true).GetComponent<Sprite>().shouldMove = true;
            //Entity.GetEntity("goomba15", true).GetComponent<Sprite>().shouldMove = true;
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
                CalculateScaledBounds(scale, joint);

                DebugJoint j = joint.Value;
                j.DrawDebugJoint(_tempJointLocations[joint.Key], scale * jointSize, offset, boneColor);
            }

            FindSkeletonBounds();

            foreach (Connection connection in _connections)
            {
                connection.DrawConnection(_joints);
            }
        }

        private void FindSkeletonBounds()
        {
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            foreach (var i in _joints)
            {
                SDL_Rect bounds = i.Value.bounds;
                minX = Math.Min(minX, bounds.x);
                minY = Math.Min(minY, bounds.y);

                maxX = Math.Max(maxX, bounds.x + bounds.w);
                maxY = Math.Max(maxY, bounds.y + bounds.h);
            }

            SdlRectMath.FromMinAndMax(minX, minY, maxX, maxY, out _bounds);
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

        public void AddOffset(Vector offset)
        {
            MoveByOffsetEachChild(offset);
        }

        void MoveByOffsetEachChild(Vector offset)
        {
            foreach (var joint in _joints)
            {
                joint.Value.AddOffset(new Vector(0, offset.Y));
                joint.Value.DrawDebugJoint(_tempJointLocations[joint.Key], jointSize, this.offset, Color.FromRgb(0, 0, 0));
            }

            FindSkeletonBounds();
        }
    }
}
