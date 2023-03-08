using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace ConsoleApp1 {
    internal class Program {
        void SkeletonFrameReady(object sender,
            SkeletonFrameReadyEventArgs args) {
            using (var frame = args.OpenSkeletonFrame()) {
                if (frame != null) {
                    Skeleton[] skeletons = new Skeleton
                    [frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                    if (skeletons.Length > 0 ) {
                    var user = skeletons.Where
                    (
                    u => u.TrackingState ==
                    SkeletonTrackingState.Tracked
                    )
                    .FirstOrDefault();
                    if (user != null) {
                        }
                    }
                }
            }
        }

        void KinectStart() {
            var kinect = KinectSensor
                    .KinectSensors
                    .FirstOrDefault
                    (s => s.Status == KinectStatus.Connected) ;
            kinect.SkeletonStream.Enable();
            kinect.SkeletonFrameReady +=
            new EventHandler<SkeletonFrameReadyEventArgs>
            (SkeletonFrameReady);
            kinect.Start();
        }

        static void Main(string[] args) {
            
        }
    }
}
