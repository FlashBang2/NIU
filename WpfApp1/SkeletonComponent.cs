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

    public class SkeletonComponent : Component, IRenderable
    {
        private Skeleton user = null;
        private DebugSkeleton skeleton;
        private bool IsKinnectAvailable = false;
        public SkeletonComponentState State = SkeletonComponentState.CalibrateX;

        public static bool IsPostCalibrationStage = false;
        public bool ShouldDrawDebugBounds = false;

        public override void Spawned()
        {
            base.Spawned();

            skeleton = new DebugSkeleton();
            skeleton.Scale = 1;

            StartKinect();

            if (!IsKinnectAvailable)
            {
                State = SkeletonComponentState.GameRunning;
                skeleton.LoadTestSkeleton();
                StartupCalibration();

                skeleton.EndOfXCalibration();
                skeleton.EndOfYCalibration();

                IsPostCalibrationStage = true;
            }
            else
            {
                State = SkeletonComponentState.CalibrateX;

                Console.WriteLine("Starting calibrating X");

                SDLTimer timer = new SDLTimer(5, false);
                timer.TimeElapsed += () =>
                {
                    skeleton.EndOfXCalibration();
                    Console.WriteLine("Starting calibrating Y");

                    State = SkeletonComponentState.CalibrateY;
                    SDLTimer t = new SDLTimer(5, false);
                    t.TimeElapsed += () => { skeleton.EndOfYCalibration(); Console.WriteLine("Starting game..."); IsPostCalibrationStage = true; State = SkeletonComponentState.GameRunning; };
                };
            }

            SDLRendering.LoadFont("arial.ttf", 96, "arial-32");
            SDLRendering.GetTextTexture("Podnieś ręce", "arial-32", Color.FromRgb(0, 0, 0));
            Owner.Width = SDLRendering.GetTextSize("Podnieś ręce", "arial-32").X;
            Owner.Height = SDLRendering.GetTextSize("Podnieś ręce", "arial-32").Y;
        }

        private void StartupCalibration()
        {
            string text = "Podnieś ręce";

            skeleton.EndOfYCalibration();
            skeleton.EndOfXCalibration();
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

        public bool ShouldDraw => true;

        public double RotationAngle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Rect SourceTextureBounds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void ReceiveRender()
        {
            base.ReceiveRender();

            switch (State)
            {
                case SkeletonComponentState.CalibrateY:
                    SDLRendering.DrawTextOnCenterPivot("Podnies rece", "arial-32", SDLApp.GetInstance().GetAppWidth() / 2, SDLApp.GetInstance().GetAppHeight() / 2, Color.FromRgb(0, 0, 0));
                    break;
                case SkeletonComponentState.CalibrateX:
                    SDLRendering.DrawTextOnCenterPivot("Rozloz Rece", "arial-32", SDLApp.GetInstance().GetAppWidth() / 2, SDLApp.GetInstance().GetAppHeight() / 2, Color.FromRgb(0, 0, 0));
                    break;
                case SkeletonComponentState.GameRunning:
                    skeleton.RenderEachJoint();
                    break;
            }

            skeleton.offset = new Vector(totalOffset, Owner.PosY - Owner.Height / 6);

            if (ShouldDrawDebugBounds && State == SkeletonComponentState.GameRunning)
            {
                SDLRendering.DrawRect((int)Owner.PosX, (int)Owner.PosY, (int)Owner.Width, (int)Owner.Height, Color.FromRgb(255, 0, 0));
            }

            if (!IsKinnectAvailable && State == SkeletonComponentState.GameRunning)
            {

                if (SDLApp.GetKey(SDL.SDL_Keycode.SDLK_d))
                {
                    Owner.AddWorldOffset(20, 0);
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
                    Owner.GetComponent<CharacterMovementComponent>().Velocity = new Vector(Owner.GetComponent<CharacterMovementComponent>().Velocity.X, -20);
                }
            }
            else if (State == SkeletonComponentState.GameRunning)
            {
                var actionType = FindActionType();
                bool isJumping = CheckIfJumping();

                switch (actionType)
                {
                    case ActionType.MoveLeft:
                        Owner.AddWorldOffset(2, 0);
                        break;
                    case ActionType.MoveRight:
                        Owner.AddWorldOffset(-2, 0);
                        break;
                }

                if (isJumping && !Owner.GetComponent<CharacterMovementComponent>().IsFalling)
                {
                    Owner.GetComponent<CharacterMovementComponent>().Velocity = new Vector(Owner.GetComponent<CharacterMovementComponent>().Velocity.X, -20);
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

        private bool CheckIfJumping()
        {
            bool isJumping = false;

            Vector head = skeleton[JointType.Head];
            Vector leftHand = skeleton[JointType.HandLeft];
            Vector rightHand = skeleton[JointType.HandRight];

            if (head.Y > leftHand.Y || head.Y > rightHand.Y)
            {
                isJumping = true;
            }
            else
            {
                isJumping = false;
            }

            return isJumping;
        }

    }
}
