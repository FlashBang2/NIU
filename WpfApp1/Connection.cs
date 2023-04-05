using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Kinect;
using System.Windows.Controls;

namespace WpfApp1
{
    public struct Connection
    {
        public JointType JointA;
        public JointType JointB;
        public Line DrawLine;

        public Connection(JointType a, JointType b)
        {
            JointA = a;
            JointB = b;
            DrawLine = new Line();
            DrawLine.Stroke = new SolidColorBrush(Color.FromArgb(255, 27, 58, 65));
        }

        public void AttachToCanvas(Canvas canvas)
        {
            canvas.Children.Add(DrawLine);
        }

        public void DrawConnection(Dictionary<JointType, Vector> jointLocations, double maxX, double maxY, FrameworkElement canvas)
        {
            var x = jointLocations[JointA].X;
            var y = jointLocations[JointA].Y;

            DrawLine.X1 = x * canvas.Width;
            DrawLine.Y1 = y * canvas.Height;

            x = jointLocations[JointB].X;
            y = jointLocations[JointB].Y;

            DrawLine.X2 = x * canvas.Width;
            DrawLine.Y2 = y * canvas.Height;
            DrawLine.Stroke = new SolidColorBrush(Color.FromArgb(255, 27, 58, 65));
            DrawLine.StrokeThickness = 2;
        }

        public void ClearConnectionLine()
        {
            DrawLine.X1 = 0;
            DrawLine.Y1 = 0;

            DrawLine.X2 = 0;
            DrawLine.Y2 = 0;
        }

        public override string ToString()
        {
            return JointA.ToString() + " => " + JointB.ToString();
        }
    }
}
