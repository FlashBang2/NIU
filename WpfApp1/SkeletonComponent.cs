using ConsoleApp1;
using Microsoft.Kinect;
using SDL2;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
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
        private SkeletonComponentState _state = SkeletonComponentState.GameRunning;


        public bool ShouldDrawDebugBounds = true;

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
            Owner.Width = skeleton.Bounds.Width;
            Owner.Height = skeleton.Bounds.Height;
#if false
            SDLTimer timer = new SDLTimer(2, false);
            timer.TimeElapsed += () =>
            {
                _state = SkeletonComponentState.CalibrateY;
                SDLTimer t = new SDLTimer(2, false);
                t.TimeElapsed += () => { _state = SkeletonComponentState.GameRunning; };
            };
#endif
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

        private bool _once = false;

        float totalOffset = 0;

        public override void ReceiveRender()
        {
            base.ReceiveRender();

            int x, y;

            SDL.SDL_GetMouseState(out x, out y);

            if (!Owner.GetComponent<CollisionComponent>().IsOverlaping && _state != SkeletonComponentState.GameRunning)
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

                    if (!_once)
                    {
                        Owner.PosX = skeleton.Bounds.Left;
                        Owner.PosY = skeleton.Bounds.Top;
                        _once = true;
                    }
                    Owner.Width = skeleton.Bounds.Width;
                    Owner.Height = skeleton.Bounds.Height;

                    break;
            }

            skeleton.offset = new Vector(totalOffset, Owner.PosY - Owner.Height / 6);

            if (ShouldDrawDebugBounds)
            {
                SDLRendering.DrawRect((int)Owner.PosX, (int)Owner.PosY, (int)Owner.Width, (int)Owner.Height, Color.FromRgb(255, 0, 0));
            }

            if (!IsKinnectAvailable)
            {

                if (SDLApp.GetKey(SDL.SDL_Keycode.SDLK_d))
                {
                    Owner.AddWorldOffset(2, 0);
                    totalOffset += 2;
                    skeleton.offset = new Vector(totalOffset, Owner.PosY - Owner.Height / 6);
                }

                if (SDLApp.GetKey(SDL.SDL_Keycode.SDLK_a))
                {
                    Owner.AddWorldOffset(-2, 0);
                    totalOffset += -2;
                    skeleton.offset = new Vector(totalOffset, Owner.PosY - Owner.Height / 6);
                }

                if (SDLApp.GetKey(SDL.SDL_Keycode.SDLK_SPACE) && !Owner.GetComponent<CharacterMovementComponent>().IsFalling)
                {
                    Owner.GetComponent<CharacterMovementComponent>().Velocity = new Vector(Owner.GetComponent<CharacterMovementComponent>().Velocity.X, -30);
                }
            }
            else
            {
                var actionType = FindActionType();

                switch (actionType)
                {
                    case ActionType.MoveLeft:
                        Owner.AddWorldOffset(-2, 0);
                        totalOffset += -2;
                        skeleton.offset = new Vector(totalOffset, Owner.PosY - Owner.Height / 6);
                        break;
                    case ActionType.MoveRight:
                        Owner.AddWorldOffset(2, 0);
                        totalOffset += 2;
                        skeleton.offset = new Vector(totalOffset, Owner.PosY - Owner.Height / 6);
                        break;
                }
            }

            SDLRendering.SetCameraFollow(Owner);
        }

        private ActionType FindActionType()
        {
            ActionType actionType = ActionType.None;

            Vector ankleLeft = skeleton[JointType.AnkleLeft];
            Vector kneeRight = skeleton[JointType.KneeRight];
            Vector ankleRight = skeleton[JointType.AnkleRight];
            Vector kneeLeft = skeleton[JointType.KneeLeft];

            if (-ankleLeft.Y > -kneeRight.Y)
            {
                actionType = ActionType.MoveRight;
            }
            else if (-ankleRight.Y > -kneeLeft.Y)
            {
                actionType = ActionType.MoveLeft;
            }

            if (!IsKinnectAvailable)
            {
                if (Keyboard.IsKeyDown(Key.D))
                {
                    actionType = ActionType.MoveRight;
                }
                else if (Keyboard.IsKeyDown(Key.A))
                {
                    actionType = ActionType.MoveLeft;
                }
                else
                {
                    actionType = ActionType.None;
                }
            }

            return actionType;
        }
    }
}
