﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public interface IEntity
    {
        IRenderable Renderable { get; set; }
        ICollideable Collideable { get; set; }

        IEntity Parent { get; set; }

        double PosX { get; set; }
        double PosY { get; set; }

        /// <summary>
        /// Approximate bounds width
        /// </summary>
        double Width { get; set; }

        string Name { get; set; }

        /// <summary>
        /// Approximate bounds height
        /// </summary>
        double Height { get; set; }

        Vector Right { get; }
        Vector Left { get; }

        Vector Up { get; }
        Vector Down { get; }

        Rect Bounds { get; }

        void AddLocalOffset(double x, double y);
        void AddWorldOffset(double x, double y);

        void Rotate(double angle);

        void AttachChild(IEntity entity);
        bool RemoveChild(IEntity entity);

        bool IsActive { get; set; }

        void Destroy();
        IEntity FindChild(string name);

        void Tick(double dt);
        void Spawned();
        void Destroyed();
    }
}