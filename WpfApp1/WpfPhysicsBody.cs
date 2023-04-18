using System;
using System.Windows;

namespace WpfApp1
{
    public class WpfPhysicsBody : IPhysicsBody
    {
        public FrameworkElement UiElement { get => _uiElement; }

        private FrameworkElement _uiElement;

        public double PosX
        {
            get => _uiElement.Margin.Left;
            set
            {
                Thickness margin = _uiElement.Margin;
                margin.Left = value;
                _uiElement.Margin = margin;
            }
        }

        public double PosY
        {
            get => _uiElement.Margin.Top;
            set
            {
                Thickness margin = _uiElement.Margin;
                margin.Top = value;
                _uiElement.Margin = margin;
            }
        }

        public Rect Bounds { get => new Rect(new Vector(PosX, PosY), new Vector(PosX + _uiElement.Width, PosY + _uiElement.Height)); }

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

        private bool _isStatic;

        public Vector Velocity { get => _velocity; set => _velocity = value; }
        private Vector _velocity = new Vector();

        public bool ShouldApplyGravity { get => _shouldApplyGravity; set => _shouldApplyGravity = value; }

        public bool _shouldApplyGravity = true;
        public double GravityScale { get => _gravityScale; set => _gravityScale = value; }

        public Vector LastMove => _lastMove;
        private Vector _lastMove = new Vector();

        public double _gravityScale = 1;

        public WpfPhysicsBody(FrameworkElement element)
        {
            _uiElement = element;
            _isStatic = false;
            Physics.AddPhysicsBody(this);
        }

        public void PhysicsUpdate()
        {
            if (!_isStatic)
            {
                Thickness location = _uiElement.Margin;
                location.Left += _velocity.X;
                location.Top += _velocity.Y;

                if (Physics.IsCollidingWithAnyObject(this))
                {
                    location = UndoLastMove(location);
                }

                _lastMove = _velocity;

                if (ShouldApplyGravity)
                {
                    _velocity -= new Vector(0, 1) * Physics.Gravity * GravityScale;
                    location.Top -= Physics.Gravity * GravityScale;

                    _lastMove = -new Vector(0, 1) * Physics.Gravity * GravityScale;

                    if (Physics.IsCollidingWithAnyObject(this))
                    {
                        location = UndoLastMove(location);
                        _velocity.Y = 0;
                    }
                }

                _uiElement.Margin = location;
            }
        }

        private Thickness UndoLastMove(Thickness location)
        {
            location.Left -= _lastMove.X;
            location.Top -= _lastMove.Y;
            _lastMove = -_lastMove;

            return location;
        }

        public void AddOffset(Vector offset)
        {
            Thickness location = _uiElement.Margin;
            location.Left += offset.X;
            location.Top += offset.Y;

            if (!_isStatic)
            {
                if (Physics.IsCollidingWithAnyObject(this))
                {
                    location = UndoLastMove(location);
                }

                _uiElement.Margin = location;
            }

            _uiElement.Margin = location;
            _lastMove = offset;
        }

        public bool IsOverlaping(IPhysicsBody other)
        {
            return Bounds.IsOverlaping(other.Bounds);
        }
    }
}
