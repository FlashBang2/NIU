﻿using System;
using System.Windows;

namespace WpfApp1
{
    public class WpfPhysicsBody 
    {
        public FrameworkElement UiElement { get => _uiElement; }


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
                _isStatic = value;
            }
        }
        public Vector Velocity { get => _velocity; set => _velocity = value; }
        public bool ShouldApplyGravity { get => _shouldApplyGravity; set => _shouldApplyGravity = value; }
        public double GravityScale { get => _gravityScale; set => _gravityScale = value; }
        public bool IsTrigger { get => _isTrigger; set => _isTrigger = value; }

        private bool _isStatic;
        private bool _isTrigger;
        private FrameworkElement _uiElement;

        private Vector _velocity = new Vector();

        private bool _shouldApplyGravity = true;
        private Vector _lastMove = new Vector();
        private double _gravityScale = 1;

        public WpfPhysicsBody(FrameworkElement element)
        {
            _uiElement = element;
            _isStatic = false;
        }

        public void PhysicsUpdate()
        {
            if (!_isStatic)
            {
                Thickness location = _uiElement.Margin;
                location.Left += _velocity.X;
                location.Top += _velocity.Y;

                _lastMove = _velocity;

                if (ShouldApplyGravity)
                {
                    location = ApplyGravity(location);
                }

                _uiElement.Margin = location;
            }
        }

        private Thickness ApplyGravity(Thickness location)
        {
            return location;
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
            if (_isStatic)
            {
                return;
            }

            Thickness location = _uiElement.Margin;
            location.Left += offset.X;
            location.Top += offset.Y;

            if (!_isStatic)
            {
                _uiElement.Margin = location;
            }

            _uiElement.Margin = location;
            _lastMove = offset;
        }

        public override string ToString()
        {
            return _uiElement.Name;
        }
    }
}
