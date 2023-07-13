using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;

namespace WpfApp1
{
    public struct Connection
    {
        public JointType jointA;
        public JointType jointB;

        public Connection(JointType a, JointType b)
        {
            jointA = a;
            jointB = b;
        }

        public void DrawConnection(Dictionary<JointType, DebugJoint> joints)
        {
            var x = joints[jointA].posX;
            var y = joints[jointA].posY;

            var x2 = joints[jointB].posX;
            var y2 = joints[jointB].posY;

            SDLRendering.DrawLine(new Vector(x, y), new Vector(x2, y2), Color.FromRgb(0, 0, 0));
        }

        public void ClearConnectionLine()
        {
        }

        public override string ToString()
        {
            return jointA.ToString() + " => " + jointB.ToString();
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
