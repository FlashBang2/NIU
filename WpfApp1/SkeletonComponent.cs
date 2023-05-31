using Microsoft.Kinect;
using SDL2;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1
{
    public enum SkeletonComponentState
    {
        CalibrateX,
        CalibrateY,
        GameRunning
    }

    public class SkeletonComponent : Component
    {
        private Skeleton user = null;
        private DebugSkeleton skeleton;
        private bool IsKinnectAvailable = false;
        private SkeletonComponentState _state = SkeletonComponentState.CalibrateX;

        public override void Spawned()
        {
            base.Spawned();

            skeleton = new DebugSkeleton();

            StartKinect();

            skeleton.Scale = 2;

            if (!IsKinnectAvailable)
            {
                skeleton.LoadTestSkeleton();
                StartupCalibration();
            }

            SDLRendering.LoadFont("arial.ttf", 16, "arial-32");
            SDLRendering.GetTextTexture("Podnieś ręce", "arial-32", Color.FromRgb(0, 0, 0));
            Owner.Width = SDLRendering.GetTextSize("Podnieś ręce", "arial-32").X;
            Owner.Height = SDLRendering.GetTextSize("Podnieś ręce", "arial-32").Y;

            SDL.SDL_GetMouseState(out int x, out int y);
            Owner.PosX = x;
            Owner.PosY = y;

            Owner.PosX = x;
            Owner.PosY = y;
        }

        private void StartupCalibration()
        {
            string text = "Podnieś ręce";

            skeleton.EndOfYCalibration();
            skeleton.EndOfXCalibration();
        }

        private void SwitchToCalibrateY()
        {

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                skeleton.Jump();
            }
        }

        private void StartKinect()
        {
            KinectSensor kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

            IsKinnectAvailable = kinect != null;

            if (IsKinnectAvailable)
            {
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += OnSkeletonFrameReady;
                kinect.Start();
            }
        }

        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs args)
        {
            using (SkeletonFrame frame = args.OpenSkeletonFrame())
            {
                bool isSkeletonDataAvailable = frame != null;

                if (isSkeletonDataAvailable)
                {
                    skeleton.UpdateSkeleton(frame);
                }
            }
        }

        public override void ReceiveRender()
        {
            base.ReceiveRender();

            int x, y;

            SDL.SDL_GetMouseState(out x, out y);

            if (!Owner.GetComponent<CollisionComponent>().IsOverlaping)
            {
                Owner.PosX = x;
                Owner.PosY = y;
            }

            switch (_state)
            {
                case SkeletonComponentState.CalibrateX:
                    SDLRendering.DrawTextOnCenterPivot("Podnieś ręce", "arial-32", Owner.PosX, Owner.PosY, Color.FromRgb(0, 0, 0));


                    break;
                case SkeletonComponentState.CalibrateY:
                    SDLRendering.DrawTextOnCenterPivot("Podnieś ręce", "arial-32", 20, 20, Color.FromRgb(0, 0, 0));
                    break;
                case SkeletonComponentState.GameRunning:
                    skeleton.RenderEachJoint();
                    break;
            }


        }
    }
}
