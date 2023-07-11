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
        public static bool isPostCalibrationStage = false;

        public bool shouldDraw => true;
        public SkeletonComponentState state = SkeletonComponentState.CalibrateX;
        public bool shouldDrawDebugBounds = false;

        public Vector velocity = new Vector(0, 0);
        public float maxVelocity = 10;
        public ActionType lastActionType = ActionType.None;

        private bool isKinnectAvailable = false;
        private const int accelerationRate = 20;
        private DebugSkeleton skeleton;

        private float totalOffset = 0;
        private float lastDeltaTime;


        private CharacterMovementComponent movementComponent;
        private Sprite sprite;


        public override void Spawned()
        {
            base.Spawned();

            skeleton = new DebugSkeleton();
            skeleton.Scale = 1;

            StartKinect();

            if (!isKinnectAvailable)
            {
                StartupGameWithoutKinnect();
            }
            else
            {
                StartupGame();
            }

            AdjustOwnerBounds();
        }

        private void AdjustOwnerBounds()
        {
            SDLRendering.LoadFont("arial.ttf", 96, "arial-32");
            SDLRendering.LoadFont("arial.ttf", 48, "arial-16");
            SDLRendering.GetTextTexture("Podnieś ręce", "arial-32", Color.FromRgb(0, 0, 0));
            var TextSize = SDLRendering.GetTextSize("Podnieś ręce", "arial-32");

            Owner.Width = (float)TextSize.X;
            Owner.Height = (float)TextSize.Y;
            isActive = true;
        }

        private void StartupGame()
        {
            state = SkeletonComponentState.CalibrateX;

            Console.WriteLine("Starting calibrating X");

            SDLTimer timer = new SDLTimer(5, false);
            timer.TimeElapsed += () =>
            {
                skeleton.EndOfXCalibration();
                Console.WriteLine("Starting calibrating Y");

                state = SkeletonComponentState.CalibrateY;
                SDLTimer t = new SDLTimer(5, false);
                t.TimeElapsed += () => { skeleton.EndOfYCalibration(); Console.WriteLine("Starting game..."); isPostCalibrationStage = true; state = SkeletonComponentState.GameRunning; };
            };
        }

        private void StartupGameWithoutKinnect()
        {
            state = SkeletonComponentState.GameRunning;
            skeleton.LoadTestSkeleton();
            StartupCalibration();

            skeleton.EndOfXCalibration();
            skeleton.EndOfYCalibration();

            isPostCalibrationStage = true;
        }

        private void StartupCalibration()
        {
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

            isKinnectAvailable = kinect != null;

            if (isKinnectAvailable)
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

            switch (state)
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

            TryGetMovementComponent();

            skeleton.offset = new Vector(totalOffset, Owner.PosY - Owner.Height / 6);

            if (shouldDrawDebugBounds && state == SkeletonComponentState.GameRunning)
            {
                SDLRendering.DrawRect((int)Owner.PosX, (int)Owner.PosY, (int)Owner.Width, (int)Owner.Height, Color.FromRgb(255, 0, 0));
            }
        }
        private void TryGetMovementComponent()
        {
            if (movementComponent == null)
            {
                movementComponent = Owner.GetComponent<CharacterMovementComponent>();
                sprite = Owner.GetComponent<Sprite>();
            }
        }

        private void SlowDown(Sprite sprite)
        {
            if (Math.Abs(velocity.X) < 1)
            {
                sprite.PlayAnim(AnimationType.Idle);
                lastActionType = ActionType.None;
            }

            velocity.X = 0.93 * velocity.X;
        }

        private void MoveRight(Sprite sprite)
        {
            if (velocity.X < 0)
            {
                sprite.PlayAnim(AnimationType.SlowDown);
            }
            else
            {
                sprite.PlayAnim(AnimationType.Walk);
            }

            TryAccelerate(1);

            sprite.FlipMode = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
            skeleton.offset = new Vector(totalOffset, Owner.PosY - Owner.Height / 6);
            lastActionType = ActionType.MoveRight;
        }

        private void MoveLeft(Sprite sprite)
        {
            if (velocity.X > 0)
            {
                sprite.PlayAnim(AnimationType.SlowDown);
            }
            else
            {
                sprite.PlayAnim(AnimationType.Walk);
            }

            TryAccelerate(-1);

            totalOffset += (int)-velocity.X;
            sprite.FlipMode = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
            skeleton.offset = new Vector(totalOffset, Owner.PosY - Owner.Height / 6);
            lastActionType = ActionType.MoveLeft;
        }

        private void TryAccelerate(int direction)
        {
            if (direction > 0)
            {
                velocity.X = velocity.X + lastDeltaTime * accelerationRate > maxVelocity ? maxVelocity : velocity.X + lastDeltaTime * accelerationRate;
            }
            else
            {
                velocity.X = velocity.X - lastDeltaTime * accelerationRate < -maxVelocity ? -maxVelocity : velocity.X - lastDeltaTime * accelerationRate;
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            lastDeltaTime = dt;
            TryGetMovementComponent();

            if (!isKinnectAvailable && state == SkeletonComponentState.GameRunning)
            {
                HandleKeyboardMovement();
            }
            else if (state == SkeletonComponentState.GameRunning)
            {
                HandleKinnectMovement();
            }

            SDLRendering.SetCameraFollow(Owner);
        }

        private void HandleKinnectMovement()
        {
            var actionType = FindActionType();
            bool isJumping = CheckIfJumping();

            switch (actionType)
            {
                case ActionType.MoveLeft:
                    MoveRight(sprite);
                    break;
                case ActionType.MoveRight:
                    MoveLeft(sprite);
                    break;
                default:
                    SlowDown(sprite);
                    break;
            }

            Owner.AddWorldOffset((float)velocity.X, 0);

            if (isJumping && !movementComponent.IsFalling)
            {
                movementComponent.Velocity = new Vector(movementComponent.Velocity.X, -22);
            }
        }

        private void HandleKeyboardMovement()
        {
            if (SDLApp.GetKey(SDL.SDL_Keycode.SDLK_d))
            {
                MoveRight(sprite);
            }
            else if (SDLApp.GetKey(SDL.SDL_Keycode.SDLK_a))
            {
                MoveLeft(sprite);
            }
            else
            {
                SlowDown(sprite);
            }

            Owner.AddWorldOffset((float)velocity.X, 0);

            if (SDLApp.GetKey(SDL.SDL_Keycode.SDLK_SPACE) && !movementComponent.IsFalling)
            {
                movementComponent.Velocity = new Vector(movementComponent.Velocity.X, -30);
            }
        }

        private ActionType FindActionType()
        {
            ActionType actionType = ActionType.None;

            // ankle left nearly equal kneeRight -> accelerate
            // ankle right nearly equal kneeLeft -> accelerate
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

            if (!isKinnectAvailable)
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

        public override bool Destroyed()
        {
            // just resets to start location
            Owner.PosX = 144;
            Owner.PosY = SDLApp.GetInstance().GetAppHeight() - 144;
            return true;
        }
    }

}
