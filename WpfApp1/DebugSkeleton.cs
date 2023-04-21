﻿using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class DebugSkeleton : IPhysicsBody
    {
        public enum JumpState
        {
            None = 0,
            DuringJump,
            DuringFall
        }

        public Vector JointSize = new Vector(0.01, 0.01);

        public double Scale = 0.15;
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

        public bool IsStatic
        {
            get => _isStatic;
            set
            {
                Physics.RemovePhysicsBody(this);
                _isStatic = value;
                Physics.AddPhysicsBody(this);
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

        private readonly Dictionary<JointType, Vector> _jointLocations = new Dictionary<JointType, Vector>();
        private readonly Dictionary<JointType, Vector> _tempJointLocations = new Dictionary<JointType, Vector>();
        private readonly Dictionary<JointType, DebugJoint> _joints = new Dictionary<JointType, DebugJoint>();
        private readonly Connection[] _connections;

        private double _maxY = 0;
        private double _minX = 0;
        private double _maxX = 0;

        private WeakReference<MainWindow> _window;
        private Rect _bounds;
        private readonly Rectangle _characterDebugBounds = new Rectangle();

        private bool _isStatic;

        private Vector _velocity = new Vector();

        private bool _shouldApplyGravity = true;
        private double _gravityScale = 1;

        public bool IsFalling { get => _jumpState != JumpState.None; }
        private JumpState _jumpState = JumpState.None;

        public DebugSkeleton(Canvas canvas, MainWindow window)
        {
            _connections = Connection.Connections;
            _window = new WeakReference<MainWindow>(window);
            AddJoints(window);
            _bounds = new Rect(new Vector(), new Vector());
            canvas.Children.Add(_characterDebugBounds);

            foreach (Connection connection in _connections)
            {
                connection.AttachToCanvas(canvas);
            }

            foreach (var x in Enum.GetValues(typeof(JointType)).Cast<JointType>())
            {
                _jointLocations[x] = new Vector();
                _tempJointLocations[x] = new Vector();
            }

            Physics.AddPhysicsBody(this);
        }

        private void AddJoints(MainWindow window)
        {
            IEnumerable<JointType> joints = Enum.GetValues(typeof(JointType)).Cast<JointType>();
            foreach (JointType joint in joints)
            {
                _joints.Add(joint, new DebugJoint(window));
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

                    pos = pos.Select(v => v.Replace(',', '.')).ToArray();

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

        public void RenderEachJoint()
        {
            Color boneColor = Color.FromArgb(255, 0, 0, 0);

            foreach (KeyValuePair<JointType, DebugJoint> joint in _joints)
            {
                CalculateScaledBounds(Scale, joint);

                DebugJoint j = joint.Value;
                j.DrawDebugJoint(_tempJointLocations[joint.Key], JointSize, boneColor);
            }

            FindSkeletonBounds();

            foreach (Connection connection in _connections)
            {
                _window.TryGetTarget(out MainWindow w);

                connection.DrawConnection(_joints, w);
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
            _characterDebugBounds.Width = _bounds.Width;
            _characterDebugBounds.Height = _bounds.Height;

            Thickness margin = _characterDebugBounds.Margin;
            margin.Left = _bounds.Left;
            margin.Top = _bounds.Top;
            _characterDebugBounds.Margin = margin;

            _characterDebugBounds.Stroke = new SolidColorBrush(Color.FromRgb(125, 125, 255));
            _characterDebugBounds.StrokeThickness = 1;
        }

        private void CalculateScaledBounds(double scale, KeyValuePair<JointType, DebugJoint> joint)
        {
            double x = scale * (_jointLocations[joint.Key].X) + 1.5 / (_maxX + 3);
            double y = scale * (-_jointLocations[joint.Key].Y) + 2 / (_maxY + 3);

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

        public bool IsOverlaping(IPhysicsBody other)
        {
            return _bounds.IsOverlaping(other.Bounds);
        }

        private bool isInFloor = false;

        public void PhysicsUpdate()
        {
            MainWindow w;

            _window.TryGetTarget(out w);

            // first move objects
            MoveByOffsetEachChild(_velocity);

            IPhysicsBody hit;


            // in a last position skeleton doesn't collide with anything
            if (Physics.IsCollidingWithAnyObject(this, out hit))
            {
                var rect = hit.Bounds.GetOverlap(Bounds);
                bool hasHitAGround = rect.Height >= 5;
                if (hasHitAGround)
                {
                    var o = Bounds.Down - hit.Bounds.Top + 0.01;
                    if (o > Bounds.Width || o < 0)
                    {
                        o = -(hit.Bounds.Down - Bounds.Top + 0.01);
                        _velocity.Y = 0;
                    }

                    MoveByOffsetEachChild(new Vector(0, o));    
                }

                // test, are we colliding in x axis
                if (Physics.IsCollidingWithAnyObject(this, out hit))
                {
                    rect = hit.Bounds.GetOverlap(Bounds);

                    if (rect.Width >= 5)
                    {
                        MoveByOffsetEachChild(new Vector(-_velocity.X, 0));
                    }
                }
            }

            var offset = -new Vector(0, 1) * _gravityScale * Physics.Gravity;
            _velocity += offset;
            _velocity.X -= 1;
            if (_velocity.X <= 0)
            {
                _velocity.X = 0;
            }

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

        private void UndoLastMove(MainWindow w)
        {
            MoveByOffsetEachChild(-_velocity);
        }

        public void AddOffset(Vector offset)
        {
            MoveByOffsetEachChild(offset);

            IPhysicsBody hit;
            if (Physics.IsCollidingWithAnyObject(this, out hit))
            {
                MoveByOffsetEachChild(-offset);
            }
        }

        void MoveByOffsetEachChild(Vector offset)
        {
            _window.TryGetTarget(out MainWindow w);

            foreach (var child in w.canvas.Children.Cast<FrameworkElement>())
            {
                if (IsPartOfSkeleton(child))
                {
                    continue;
                }

                Thickness thickness = child.Margin;

                thickness.Left += offset.X;

                child.Margin = thickness;
            }

            foreach (var joint in _joints)
            {
                joint.Value.AddOffset(new Vector(0, offset.Y));
                joint.Value.DrawDebugJoint(_tempJointLocations[joint.Key], JointSize, Color.FromRgb(0, 0, 0));
            }

            FindSkeletonBounds();
        }

        private bool IsPartOfSkeleton(FrameworkElement ch)
        {
            bool isBone = ch.GetType().Equals(typeof(Line));
            bool isJoint = ch.GetType().Equals(typeof(Ellipse));

            return isBone || isJoint || ch == _characterDebugBounds;
        }

        public void Jump()
        {
            Velocity = new Vector(Velocity.X, MaxJumpHeight);
        }
    }
}