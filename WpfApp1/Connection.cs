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

        public void DrawConnection(Dictionary<JointType, Vector> jointLocations, FrameworkElement canvas)
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

        public static Connection[] Connections =
        {
            new Connection(JointType.Head, JointType.ShoulderCenter),

            new Connection(JointType.ShoulderCenter, JointType.ShoulderRight),
            new Connection(JointType.ShoulderRight, JointType.ElbowRight),
            new Connection(JointType.ElbowRight, JointType.WristRight),
            new Connection(JointType.WristRight, JointType.HandRight),

            new Connection(JointType.ShoulderCenter, JointType.ShoulderLeft),
            new Connection(JointType.ShoulderLeft, JointType.ElbowLeft),
            new Connection(JointType.ElbowLeft, JointType.WristLeft),
            new Connection(JointType.WristLeft, JointType.HandLeft),

            new Connection(JointType.ShoulderCenter, JointType.Spine),
            new Connection(JointType.Spine, JointType.HipCenter),

            new Connection(JointType.HipCenter, JointType.HipRight),
            new Connection(JointType.HipRight, JointType.KneeRight),
            new Connection(JointType.KneeRight, JointType.AnkleRight),
            new Connection(JointType.AnkleRight, JointType.FootRight),

            new Connection(JointType.HipCenter, JointType.HipLeft),
            new Connection(JointType.HipLeft, JointType.KneeLeft),
            new Connection(JointType.KneeLeft, JointType.AnkleLeft),
            new Connection(JointType.AnkleLeft, JointType.FootLeft),
        };
    }
}
