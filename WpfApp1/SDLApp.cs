using ConsoleApp1;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;
using static SDL2.SDL_ttf;

namespace WpfApp1
{
    public partial class SDLApp
    {
        private IntPtr _window = IntPtr.Zero;
        private IntPtr _renderer = IntPtr.Zero;
        private bool _isOpen = true;

        private static SDLApp _instance;

        public static SDLApp GetInstance()
        {
            return _instance;
        }

        public SDLApp(int width, int height, string title)
        {
            if (SDL_Init(SDL_INIT_EVERYTHING) != 0)
            {
                throw new ApplicationException("SDL2 library couldn't be initialized");
            }

            IMG_InitFlags flags = IMG_InitFlags.IMG_INIT_PNG | IMG_InitFlags.IMG_INIT_JPG;

            if (IMG_Init(flags) != ((int)flags))
            {
                throw new ApplicationException("Image library couldn't be initialized");
            }

            if (TTF_Init() != 0)
            {
                throw new ApplicationException("TypeFont library couldn't be initialized");
            }

            _window = SDL_CreateWindow(title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height, SDL_WindowFlags.SDL_WINDOW_SHOWN); ;

            if (_window.Equals(IntPtr.Zero))
            {
                FreeResources();
                throw new ApplicationException(SDL_GetError());
            }

            _renderer = SDL_CreateRenderer(_window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (_renderer.Equals(IntPtr.Zero))
            {
                FreeResources();
                throw new ApplicationException(SDL_GetError());
            }

            SDLRendering.Init(_renderer);
            _instance = this;
        }

        public int GetAppWidth()
        {
            SDL_GetWindowSize(_window, out int w, out _);
            return w;
        }

        public int GetAppHeight()
        {
            SDL_GetWindowSize(_window, out _, out int h);
            return h;
        }


        public void Run()
        {
            Debug.Assert(!_window.Equals(IntPtr.Zero));

            uint lastTick = SDL_GetTicks();
            float delta = 0.0f;

            while (_isOpen)
            {
                while (SDL_PollEvent(out SDL_Event systemEvent) != 0)
                {
                    OnSystemEventOccured(systemEvent);
                }

                uint tick_time = SDL_GetTicks();
                delta = tick_time - lastTick;
                lastTick = tick_time;

                Entity.RootEntity.Tick(delta / 1000.0f);

                SDLRendering.ClearFrame();
                Entity.RootEntity.ReceiveRender();
                SDLRendering.RenderFrame();
            }

            FreeResources();
        }

        private void FreeResources()
        {
            SDLRendering.Quit();
            SDL_DestroyRenderer(_renderer);
            SDL_DestroyWindow(_window);
            IMG_Quit();
            TTF_Quit();
            SDL_Quit();

            _window = IntPtr.Zero;
            _renderer = IntPtr.Zero;
        }

        private void OnSystemEventOccured(SDL_Event evt)
        {
            if (evt.type == SDL_EventType.SDL_QUIT)
            {
                _isOpen = false;
            }
        }


        class RectRenderable : Component
        {
            public override void Spawned()
            {
                base.Spawned();
                Owner.Width = GetInstance().GetAppWidth();
                Owner.Height = 40;
                Owner.PosX = 0;
                Owner.PosY = GetInstance().GetAppHeight() - Owner.Height;
            }


            public Color blockColor = Color.FromRgb(120, 40, 30);

            public override void ReceiveRender()
            {
                base.ReceiveRender();


                var e = Entity.RootEntity.FindChild("Skeleton");

                if (e.GetComponent<SkeletonComponent>().State != SkeletonComponentState.GameRunning)
                {
                    return;
                }

                SDLRendering.FillRect((int)Owner.PosX, (int)Owner.PosY, (int)Owner.Width, (int)Owner.Height, blockColor);
            }
        }

        class Temp : Component
        {

            public override void Spawned()
            {
                base.Spawned();
                Owner.Width = 40;
                Owner.Height = 40;
            }

            public override void OnTick(float dt)
            {
                int x, y;

                SDL_GetMouseState(out x, out y);

                Owner.PosX = x;
                Owner.PosY = y;
            }

            public Color blockColor = Color.FromRgb(120, 40, 40);

            public override void ReceiveRender()
            {
                base.ReceiveRender();

                var e = Entity.RootEntity.FindChild("Skeleton");

                if (e.GetComponent<SkeletonComponent>().State != SkeletonComponentState.GameRunning)
                {
                    return;
                }
                SDLRendering.FillCircle((int)Owner.PosX, (int)Owner.PosY, (int)(Owner.Width / 2), blockColor);
            }
        }

        public static bool GetKey(SDL_Keycode _keycode)
        {
            int arraySize;
            bool isKeyPressed = false;
            IntPtr origArray = SDL_GetKeyboardState(out arraySize);
            byte[] keys = new byte[arraySize];
            byte keycode = (byte)SDL_GetScancodeFromKey(_keycode);
            Marshal.Copy(origArray, keys, 0, arraySize);
            isKeyPressed = keys[keycode] == 1;
            return isKeyPressed;
        }

        public static void Main(string[] args)
        {
            SDLApp app = new SDLApp(1920, 1080, "NIU");

            LoadTextures();

            AddPlatforms(app);

            playerCharacter();

            questionMarksBlocks(app);

            brickBlocks(app);

 

            Entity goomba = Entity.CreateEntity("goomba");
            goomba.AddComponent<CharacterMovementComponent>();
            goomba.AddComponent<Gumba>();
            goomba.AddComponent<CollisionComponent>();
            goomba.AddComponent<Sprite>();
            goomba.GetComponent<Sprite>().spriteId = "goomba";
            AnimationData gumbaaData = new AnimationData();
            gumbaaData.StartFrame = 0;
            gumbaaData.EndFrame = 1;
            gumbaaData.FrameRatePerSecond = 1;
            gumbaaData.Width = 48;
            gumbaaData.Height = 48;
            goomba.GetComponent<CollisionComponent>().IsStatic = false;
            goomba.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);

            goomba.PosX += 200;
            goomba.Width = 96 / 2;
            goomba.Height = 48;
            app.Run();
        }

        private static void LoadTextures()
        {
            SDLRendering.LoadTexture("firstPlatform.png", "first_platform");
            SDLRendering.LoadTexture("second_platform.png", "second_platform");
            SDLRendering.LoadTexture("third_platform.png", "third_platform");
            SDLRendering.LoadTexture("fourth_platform.png", "fourth_platform");
            SDLRendering.LoadTexture("mario_big.png", "mario_big");
            SDLRendering.LoadTexture("mario_small.png", "mario_small");
            SDLRendering.LoadTexture("questionmark.png", "question_block");
            SDLRendering.LoadTexture("goomba.png", "goomba");
            SDLRendering.LoadTexture("brick.png", "brick");
            backgroundTexture = SDLRendering.LoadTexture("background_objects.png", "background_objects");

        }

        static IntPtr backgroundTexture = IntPtr.Zero;

        private static void AddPlatforms(SDLApp app)
        {
            createDownPlatform(app);

            uint format;
            int access, width, height;

            Entity background = Entity.CreateEntity("background");
            background.AddComponent<Sprite>();
            background.GetComponent<Sprite>().spriteId = "background_objects";
            SDL_QueryTexture(backgroundTexture, out format, out access, out width, out height);
            background.Width = width;
            background.Height = height;
            background.PosY = 40;
            SDLRendering.SetWorldBounds(width, height);
        }

        private static void createDownPlatform(SDLApp app)
        {
            Entity firstPlatform = Entity.CreateEntity("firstPlatform");
            firstPlatform.AddComponent<CollisionComponent>();
            firstPlatform.AddComponent<Sprite>();
            firstPlatform.GetComponent<Sprite>().spriteId = "first_platform";
            firstPlatform.Width = 3312;
            firstPlatform.Height = 96;
            firstPlatform.PosX = 0;
            firstPlatform.PosY = app.GetAppHeight() - 96;

            Entity secondPlatform = Entity.CreateEntity("secondPlatform");
            secondPlatform.AddComponent<CollisionComponent>();
            secondPlatform.AddComponent<Sprite>();
            secondPlatform.GetComponent<Sprite>().spriteId = "second_platform";
            secondPlatform.Width = 720;
            secondPlatform.Height = 96;
            secondPlatform.PosX = firstPlatform.Width + firstPlatform.PosX + 96;
            secondPlatform.PosY = firstPlatform.PosY;

            Entity thirdPlatform = Entity.CreateEntity("thirdPlatform");
            thirdPlatform.AddComponent<CollisionComponent>();
            thirdPlatform.AddComponent<Sprite>();
            thirdPlatform.GetComponent<Sprite>().spriteId = "third_platform";
            thirdPlatform.Width = 3072;
            thirdPlatform.Height = 96;
            thirdPlatform.PosX = secondPlatform.PosX + secondPlatform.Width + 144;
            thirdPlatform.PosY = firstPlatform.PosY;

            Entity fourthPlatform = Entity.CreateEntity("fourthPlatform");
            fourthPlatform.AddComponent<CollisionComponent>();
            fourthPlatform.AddComponent<Sprite>();
            fourthPlatform.GetComponent<Sprite>().spriteId = "fourth_platform";
            fourthPlatform.Width = 3216;
            fourthPlatform.Height = 96;
            fourthPlatform.PosX = thirdPlatform.PosX + thirdPlatform.Width + 96;
            fourthPlatform.PosY = firstPlatform.PosY;
        }

        private static void playerCharacter()
        {
            Entity mario = Entity.CreateEntity("mario");
            mario.AddComponent<SkeletonComponent>();
            mario.AddComponent<CharacterMovementComponent>();
            mario.GetComponent<CharacterMovementComponent>().IsControlledByPlayer = true;
            mario.AddComponent<CollisionComponent>();
            mario.AddComponent<Sprite>();
            mario.GetComponent<Sprite>().spriteId = "mario_small";
            mario.GetComponent<CollisionComponent>().IsStatic = false;
            mario.Width = 48;
            mario.Height = 48;

            AnimationData idle = new AnimationData();
            idle.EndFrame = 0;
            idle.StartFrame = 0;
            idle.FrameRatePerSecond = 16;
            idle.Width = 48;
            idle.Height = 48;

            AnimationData walkData = new AnimationData();
            walkData.EndFrame = 3;
            walkData.StartFrame = 0;
            walkData.FrameRatePerSecond = 16;
            walkData.Width = 48;
            walkData.Height = 48;

            AnimationData jumpData = new AnimationData();
            jumpData.EndFrame = 3;
            jumpData.StartFrame = 0;
            jumpData.FrameRatePerSecond = 16;
            jumpData.Width = 48;
            jumpData.Height = 48;

            AnimationData slowdownData = new AnimationData();
            slowdownData.EndFrame = 5;
            slowdownData.StartFrame = 5;
            slowdownData.FrameRatePerSecond = 16;
            slowdownData.Width = 48;
            slowdownData.Height = 48;

            mario.GetComponent<Sprite>().AddAnimation(AnimationType.Walk, walkData);
            mario.GetComponent<Sprite>().AddAnimation(AnimationType.SlowDown, slowdownData);
            mario.GetComponent<Sprite>().AddAnimation(AnimationType.Jump, jumpData);
            mario.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idle);
            mario.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
        }
        private static void questionMarksBlocks(SDLApp app)
        {
            Entity questionBlock = Entity.CreateEntity("questionBlock");
            questionBlock.AddComponent<CollisionComponent>();
            questionBlock.AddComponent<Sprite>();
            questionBlock.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock.Width = 48;
            questionBlock.Height = 48;
            questionBlock.PosX = 768;
            questionBlock.PosY = app.GetAppHeight() - 288;

            Entity questionBlock2 = Entity.CreateEntity("questionBlock2");
            questionBlock2.AddComponent<CollisionComponent>();
            questionBlock2.AddComponent<Sprite>();
            questionBlock2.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock2.Width = 48;
            questionBlock2.Height = 48;
            questionBlock2.PosX = questionBlock.PosX + 192;
            questionBlock2.PosY = questionBlock.PosY;

            Entity questionBlock3 = Entity.CreateEntity("questionBlock3");
            questionBlock3.AddComponent<CollisionComponent>();
            questionBlock3.AddComponent<Sprite>();
            questionBlock3.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock3.Width = 48;
            questionBlock3.Height = 48;
            questionBlock3.PosX = questionBlock2.PosX + 96;
            questionBlock3.PosY = questionBlock2.PosY;

            Entity questionBlock4 = Entity.CreateEntity("questionBlock4");
            questionBlock4.AddComponent<CollisionComponent>();
            questionBlock4.AddComponent<Sprite>();
            questionBlock4.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock4.Width = 48;
            questionBlock4.Height = 48;
            questionBlock4.PosX = questionBlock2.PosX + 48;
            questionBlock4.PosY = questionBlock2.PosY - 192;

            AnimationData idleQuestion = new AnimationData();
            idleQuestion.EndFrame = 2;
            idleQuestion.StartFrame = 0;
            idleQuestion.FrameRatePerSecond = 4;
            idleQuestion.Width = 48;
            idleQuestion.Height = 48;

            questionBlock.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock2.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock3.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock4.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock2.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock3.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock4.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
        }
        private static void brickBlocks(SDLApp app)
        {
            Entity brick = Entity.CreateEntity("Brick");
            brick.AddComponent<CollisionComponent>();
            brick.AddComponent<Sprite>();
            brick.GetComponent<Sprite>().spriteId = "brick";
            brick.Width = 48;
            brick.Height = 48;
            brick.PosX = 1008;
            brick.PosY = app.GetAppHeight() - 288;

            Entity brick2 = Entity.CreateEntity("Brick2");
            brick2.AddComponent<CollisionComponent>();
            brick2.AddComponent<Sprite>();
            brick2.GetComponent<Sprite>().spriteId = "brick";
            brick2.Width = 48;
            brick2.Height = 48;
            brick2.PosX = brick.PosX - 96;
            brick2.PosY = brick.PosY;

            Entity brick3 = Entity.CreateEntity("Brick3");
            brick3.AddComponent<CollisionComponent>();
            brick3.AddComponent<Sprite>();
            brick3.GetComponent<Sprite>().spriteId = "brick";
            brick3.Width = 48;
            brick3.Height = 48;
            brick3.PosX = brick.PosX + 96;
            brick3.PosY = brick.PosY;

        }

        private static void spawnBackground()
        {

        }
        
        private static void spawnEnemiesAtStartLocation()
        {

        }
    }
}
