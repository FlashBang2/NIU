﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Kinect;

namespace ConsoleApp1 {
    internal class Program {
        static void SkeletonFrameReady(object sender,
            SkeletonFrameReadyEventArgs args)
        {
            using (var frame = args.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    Skeleton[] skeletons = new Skeleton
                    [frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                    if (skeletons.Length > 0)
                    {
                        var user = skeletons.Where
                        (
                        u => u.TrackingState ==
                        SkeletonTrackingState.Tracked
                        )
                        .FirstOrDefault();
                        if (user != null)
                        {
                            foreach (var joint in user.Joints)
                            {
                                Console.WriteLine(user.Joints[JointType.Head].Position.Z);
                            }
                        }
                    }
                }
            }
        }

        static void Main(string[] args) {
            var kinect = KinectSensor
                   .KinectSensors
                   .FirstOrDefault
                   (s => s.Status == KinectStatus.Connected);
            kinect.SkeletonStream.Enable();
            kinect.SkeletonFrameReady +=
            new EventHandler<SkeletonFrameReadyEventArgs>
            (SkeletonFrameReady);
            kinect.Start();
        }
    }
}