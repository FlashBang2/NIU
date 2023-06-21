using ConsoleApp1;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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

        public bool canStartGoomba = false;

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

                if (Math.Abs(Entity.GetEntity("goomba", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba2", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba2", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba3", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba3", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba4", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba4", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba5", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba5", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba6", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba6", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba7", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba7", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba8", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba8", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba9", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba9", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba10", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba10", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba11", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba11", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba12", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba12", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba13", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba13", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba14", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba14", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba15", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba15", true).GetComponent<Sprite>().shouldMove = true;
                if (Math.Abs(Entity.GetEntity("goomba16", true).PosX - Entity.GetEntity("mario", true).PosX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba16", true).GetComponent<Sprite>().shouldMove = true;

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

            playerCharacter(app);

            questionMarksBlocks(app);

            brickBlocks(app);

            renderStairs(app);

            renderPipes(app);

            spawnEnemiesAtStartLocation(app);

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
            SDLRendering.LoadTexture("koopa.png", "koopa");
            SDLRendering.LoadTexture("brick.png", "brick");
            SDLRendering.LoadTexture("block.png", "block");
            SDLRendering.LoadTexture("pipe_small.png", "pipe_small");
            SDLRendering.LoadTexture("pipe_medium.png", "pipe_medium");
            SDLRendering.LoadTexture("pipe_large.png", "pipe_large");
            SDLRendering.LoadTexture("FullFlag.png", "full-flag");
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

        private static void playerCharacter(SDLApp app)
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
            mario.PosX = 144;
            mario.PosY = app.GetAppHeight() - 144;

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
            questionBlock2.PosX = questionBlock.PosX + 240;
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

            Entity questionBlock5 = Entity.CreateEntity("questionBlock5");
            questionBlock5.AddComponent<CollisionComponent>();
            questionBlock5.AddComponent<Sprite>();
            questionBlock5.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock5.Width = 48;
            questionBlock5.Height = 48;
            questionBlock5.PosX = questionBlock3.PosX + 2640;
            questionBlock5.PosY = questionBlock3.PosY;

            Entity questionBlock6 = Entity.CreateEntity("questionBlock6");
            questionBlock6.AddComponent<CollisionComponent>();
            questionBlock6.AddComponent<Sprite>();
            questionBlock6.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock6.Width = 48;
            questionBlock6.Height = 48;
            questionBlock6.PosX = questionBlock5.PosX + 768;
            questionBlock6.PosY = questionBlock5.PosY - 192;

            Entity questionBlock7 = Entity.CreateEntity("questionBlock7");
            questionBlock7.AddComponent<CollisionComponent>();
            questionBlock7.AddComponent<Sprite>();
            questionBlock7.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock7.Width = 48;
            questionBlock7.Height = 48;
            questionBlock7.PosX = questionBlock5.PosX + 1344;
            questionBlock7.PosY = questionBlock5.PosY;

            Entity questionBlock8 = Entity.CreateEntity("questionBlock8");
            questionBlock8.AddComponent<CollisionComponent>();
            questionBlock8.AddComponent<Sprite>();
            questionBlock8.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock8.Width = 48;
            questionBlock8.Height = 48;
            questionBlock8.PosX = questionBlock7.PosX + 144;
            questionBlock8.PosY = questionBlock5.PosY;

            Entity questionBlock9 = Entity.CreateEntity("questionBlock9");
            questionBlock9.AddComponent<CollisionComponent>();
            questionBlock9.AddComponent<Sprite>();
            questionBlock9.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock9.Width = 48;
            questionBlock9.Height = 48;
            questionBlock9.PosX = questionBlock8.PosX;
            questionBlock9.PosY = questionBlock5.PosY - 192;

            Entity questionBlock10 = Entity.CreateEntity("questionBlock10");
            questionBlock10.AddComponent<CollisionComponent>();
            questionBlock10.AddComponent<Sprite>();
            questionBlock10.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock10.Width = 48;
            questionBlock10.Height = 48;
            questionBlock10.PosX = questionBlock8.PosX + 144;
            questionBlock10.PosY = questionBlock5.PosY;

            Entity questionBlock11 = Entity.CreateEntity("questionBlock11");
            questionBlock11.AddComponent<CollisionComponent>();
            questionBlock11.AddComponent<Sprite>();
            questionBlock11.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock11.Width = 48;
            questionBlock11.Height = 48;
            questionBlock11.PosX = questionBlock9.PosX + 960;
            questionBlock11.PosY = questionBlock9.PosY;

            Entity questionBlock12 = Entity.CreateEntity("questionBlock12");
            questionBlock12.AddComponent<CollisionComponent>();
            questionBlock12.AddComponent<Sprite>();
            questionBlock12.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock12.Width = 48;
            questionBlock12.Height = 48;
            questionBlock12.PosX = questionBlock11.PosX + 48;
            questionBlock12.PosY = questionBlock9.PosY;

            Entity questionBlock13 = Entity.CreateEntity("questionBlock13");
            questionBlock13.AddComponent<CollisionComponent>();
            questionBlock13.AddComponent<Sprite>();
            questionBlock13.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock13.Width = 48;
            questionBlock13.Height = 48;
            questionBlock13.PosX = questionBlock12.PosX + 1920;
            questionBlock13.PosY = questionBlock12.PosY + 192;

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
            questionBlock5.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock6.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock7.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock8.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock9.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock10.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock11.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock12.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock13.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, idleQuestion);
            questionBlock.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock2.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock3.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock4.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock5.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock6.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock7.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock8.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock9.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock10.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock11.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock12.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            questionBlock13.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
        }
        private static void brickBlocks(SDLApp app)
        {
            Entity brick = Entity.CreateEntity("Brick");
            brick.AddComponent<CollisionComponent>();
            brick.AddComponent<Sprite>();
            brick.GetComponent<Sprite>().spriteId = "brick";
            brick.Width = 48;
            brick.Height = 48;
            brick.PosX = 1056;
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

            Entity brick4 = Entity.CreateEntity("Brick4");
            brick4.AddComponent<CollisionComponent>();
            brick4.AddComponent<Sprite>();
            brick4.GetComponent<Sprite>().spriteId = "brick";
            brick4.Width = 48;
            brick4.Height = 48;
            brick4.PosX = brick3.PosX + 2544;
            brick4.PosY = brick.PosY;

            Entity brick5 = Entity.CreateEntity("Brick5");
            brick5.AddComponent<CollisionComponent>();
            brick5.AddComponent<Sprite>();
            brick5.GetComponent<Sprite>().spriteId = "brick";
            brick5.Width = 48;
            brick5.Height = 48;
            brick5.PosX = brick4.PosX + 96;
            brick5.PosY = brick.PosY;

            Entity brick6 = Entity.CreateEntity("Brick6");
            brick6.AddComponent<CollisionComponent>();
            brick6.AddComponent<Sprite>();
            brick6.GetComponent<Sprite>().spriteId = "brick";
            brick6.Width = 48;
            brick6.Height = 48;
            brick6.PosX = brick5.PosX + 48;
            brick6.PosY = brick5.PosY - 192;

            Entity brick7 = Entity.CreateEntity("Brick7");
            brick7.AddComponent<CollisionComponent>();
            brick7.AddComponent<Sprite>();
            brick7.GetComponent<Sprite>().spriteId = "brick";
            brick7.Width = 48;
            brick7.Height = 48;
            brick7.PosX = brick6.PosX + 48;
            brick7.PosY = brick6.PosY;

            Entity brick8 = Entity.CreateEntity("Brick8");
            brick8.AddComponent<CollisionComponent>();
            brick8.AddComponent<Sprite>();
            brick8.GetComponent<Sprite>().spriteId = "brick";
            brick8.Width = 48;
            brick8.Height = 48;
            brick8.PosX = brick7.PosX + 48;
            brick8.PosY = brick6.PosY;

            Entity brick9 = Entity.CreateEntity("Brick9");
            brick9.AddComponent<CollisionComponent>();
            brick9.AddComponent<Sprite>();
            brick9.GetComponent<Sprite>().spriteId = "brick";
            brick9.Width = 48;
            brick9.Height = 48;
            brick9.PosX = brick8.PosX + 48;
            brick9.PosY = brick6.PosY;

            Entity brick10 = Entity.CreateEntity("Brick10");
            brick10.AddComponent<CollisionComponent>();
            brick10.AddComponent<Sprite>();
            brick10.GetComponent<Sprite>().spriteId = "brick";
            brick10.Width = 48;
            brick10.Height = 48;
            brick10.PosX = brick9.PosX + 48;
            brick10.PosY = brick6.PosY;

            Entity brick11 = Entity.CreateEntity("Brick11");
            brick11.AddComponent<CollisionComponent>();
            brick11.AddComponent<Sprite>();
            brick11.GetComponent<Sprite>().spriteId = "brick";
            brick11.Width = 48;
            brick11.Height = 48;
            brick11.PosX = brick10.PosX + 48;
            brick11.PosY = brick6.PosY;

            Entity brick12 = Entity.CreateEntity("Brick12");
            brick12.AddComponent<CollisionComponent>();
            brick12.AddComponent<Sprite>();
            brick12.GetComponent<Sprite>().spriteId = "brick";
            brick12.Width = 48;
            brick12.Height = 48;
            brick12.PosX = brick11.PosX + 48;
            brick12.PosY = brick6.PosY;

            Entity brick13 = Entity.CreateEntity("Brick13");
            brick13.AddComponent<CollisionComponent>();
            brick13.AddComponent<Sprite>();
            brick13.GetComponent<Sprite>().spriteId = "brick";
            brick13.Width = 48;
            brick13.Height = 48;
            brick13.PosX = brick12.PosX + 48;
            brick13.PosY = brick6.PosY;

            Entity brick14 = Entity.CreateEntity("Brick14");
            brick14.AddComponent<CollisionComponent>();
            brick14.AddComponent<Sprite>();
            brick14.GetComponent<Sprite>().spriteId = "brick";
            brick14.Width = 48;
            brick14.Height = 48;
            brick14.PosX = brick13.PosX + 192;
            brick14.PosY = brick6.PosY;

            Entity brick15 = Entity.CreateEntity("Brick15");
            brick15.AddComponent<CollisionComponent>();
            brick15.AddComponent<Sprite>();
            brick15.GetComponent<Sprite>().spriteId = "brick";
            brick15.Width = 48;
            brick15.Height = 48;
            brick15.PosX = brick14.PosX + 48;
            brick15.PosY = brick6.PosY;

            Entity brick16 = Entity.CreateEntity("Brick16");
            brick16.AddComponent<CollisionComponent>();
            brick16.AddComponent<Sprite>();
            brick16.GetComponent<Sprite>().spriteId = "brick";
            brick16.Width = 48;
            brick16.Height = 48;
            brick16.PosX = brick15.PosX + 48;
            brick16.PosY = brick6.PosY;

            Entity brick17 = Entity.CreateEntity("Brick17");
            brick17.AddComponent<CollisionComponent>();
            brick17.AddComponent<Sprite>();
            brick17.GetComponent<Sprite>().spriteId = "brick";
            brick17.Width = 48;
            brick17.Height = 48;
            brick17.PosX = brick16.PosX + 48;
            brick17.PosY = brick6.PosY + 192;

            Entity brick18 = Entity.CreateEntity("Brick18");
            brick18.AddComponent<CollisionComponent>();
            brick18.AddComponent<Sprite>();
            brick18.GetComponent<Sprite>().spriteId = "brick";
            brick18.Width = 48;
            brick18.Height = 48;
            brick18.PosX = brick17.PosX + 288;
            brick18.PosY = brick17.PosY;

            Entity brick19 = Entity.CreateEntity("Brick19");
            brick19.AddComponent<CollisionComponent>();
            brick19.AddComponent<Sprite>();
            brick19.GetComponent<Sprite>().spriteId = "brick";
            brick19.Width = 48;
            brick19.Height = 48;
            brick19.PosX = brick18.PosX + 48;
            brick19.PosY = brick17.PosY;

            Entity brick20 = Entity.CreateEntity("Brick20");
            brick20.AddComponent<CollisionComponent>();
            brick20.AddComponent<Sprite>();
            brick20.GetComponent<Sprite>().spriteId = "brick";
            brick20.Width = 48;
            brick20.Height = 48;
            brick20.PosX = brick19.PosX + 816;
            brick20.PosY = brick17.PosY;

            Entity brick21 = Entity.CreateEntity("Brick21");
            brick21.AddComponent<CollisionComponent>();
            brick21.AddComponent<Sprite>();
            brick21.GetComponent<Sprite>().spriteId = "brick";
            brick21.Width = 48;
            brick21.Height = 48;
            brick21.PosX = brick20.PosX + 144;
            brick21.PosY = brick20.PosY - 192;

            Entity brick22 = Entity.CreateEntity("Brick22");
            brick22.AddComponent<CollisionComponent>();
            brick22.AddComponent<Sprite>();
            brick22.GetComponent<Sprite>().spriteId = "brick";
            brick22.Width = 48;
            brick22.Height = 48;
            brick22.PosX = brick21.PosX + 48;
            brick22.PosY = brick21.PosY;

            Entity brick23 = Entity.CreateEntity("Brick23");
            brick23.AddComponent<CollisionComponent>();
            brick23.AddComponent<Sprite>();
            brick23.GetComponent<Sprite>().spriteId = "brick";
            brick23.Width = 48;
            brick23.Height = 48;
            brick23.PosX = brick22.PosX + 48;
            brick23.PosY = brick21.PosY;

            Entity brick24 = Entity.CreateEntity("Brick24");
            brick24.AddComponent<CollisionComponent>();
            brick24.AddComponent<Sprite>();
            brick24.GetComponent<Sprite>().spriteId = "brick";
            brick24.Width = 48;
            brick24.Height = 48;
            brick24.PosX = brick23.PosX + 240;
            brick24.PosY = brick21.PosY;

            Entity brick25 = Entity.CreateEntity("Brick25");
            brick25.AddComponent<CollisionComponent>();
            brick25.AddComponent<Sprite>();
            brick25.GetComponent<Sprite>().spriteId = "brick";
            brick25.Width = 48;
            brick25.Height = 48;
            brick25.PosX = brick24.PosX + 144;
            brick25.PosY = brick21.PosY;

            Entity brick26 = Entity.CreateEntity("Brick26");
            brick26.AddComponent<CollisionComponent>();
            brick26.AddComponent<Sprite>();
            brick26.GetComponent<Sprite>().spriteId = "brick";
            brick26.Width = 48;
            brick26.Height = 48;
            brick26.PosX = brick24.PosX + 48;
            brick26.PosY = brick25.PosY + 192;

            Entity brick27 = Entity.CreateEntity("Brick27");
            brick27.AddComponent<CollisionComponent>();
            brick27.AddComponent<Sprite>();
            brick27.GetComponent<Sprite>().spriteId = "brick";
            brick27.Width = 48;
            brick27.Height = 48;
            brick27.PosX = brick26.PosX + 48;
            brick27.PosY = brick26.PosY;

            Entity brick28 = Entity.CreateEntity("Brick28");
            brick28.AddComponent<CollisionComponent>();
            brick28.AddComponent<Sprite>();
            brick28.GetComponent<Sprite>().spriteId = "brick";
            brick28.Width = 48;
            brick28.Height = 48;
            brick28.PosX = brick27.PosX + 1824;
            brick28.PosY = brick27.PosY;

            Entity brick29 = Entity.CreateEntity("Brick29");
            brick29.AddComponent<CollisionComponent>();
            brick29.AddComponent<Sprite>();
            brick29.GetComponent<Sprite>().spriteId = "brick";
            brick29.Width = 48;
            brick29.Height = 48;
            brick29.PosX = brick28.PosX + 48;
            brick29.PosY = brick27.PosY;

            Entity brick30 = Entity.CreateEntity("Brick30");
            brick30.AddComponent<CollisionComponent>();
            brick30.AddComponent<Sprite>();
            brick30.GetComponent<Sprite>().spriteId = "brick";
            brick30.Width = 48;
            brick30.Height = 48;
            brick30.PosX = brick29.PosX + 96;
            brick30.PosY = brick27.PosY;
        }
        
        private static void renderPipes(SDLApp app)
        {
            Entity pipe = Entity.CreateEntity("Pipe");
            pipe.AddComponent<CollisionComponent>();
            pipe.AddComponent<Sprite>();
            pipe.GetComponent<Sprite>().spriteId = "pipe_small";
            pipe.Width = 96;
            pipe.Height = 96;
            pipe.PosX = 1344;
            pipe.PosY = app.GetAppHeight() - 192;

            Entity pipe2 = Entity.CreateEntity("Pipe2");
            pipe2.AddComponent<CollisionComponent>();
            pipe2.AddComponent<Sprite>();
            pipe2.GetComponent<Sprite>().spriteId = "pipe_medium";
            pipe2.Width = 96;
            pipe2.Height = 144;
            pipe2.PosX = pipe.PosX + 480;
            pipe2.PosY = app.GetAppHeight() - 240;

            Entity pipe3 = Entity.CreateEntity("Pipe3");
            pipe3.AddComponent<CollisionComponent>();
            pipe3.AddComponent<Sprite>();
            pipe3.GetComponent<Sprite>().spriteId = "pipe_large";
            pipe3.Width = 96;
            pipe3.Height = 192;
            pipe3.PosX = pipe2.PosX + 384;
            pipe3.PosY = app.GetAppHeight() - 288;

            Entity pipe4 = Entity.CreateEntity("Pipe4");
            pipe4.AddComponent<CollisionComponent>();
            pipe4.AddComponent<Sprite>();
            pipe4.GetComponent<Sprite>().spriteId = "pipe_large";
            pipe4.Width = 96;
            pipe4.Height = 192;
            pipe4.PosX = pipe3.PosX + 528;
            pipe4.PosY = app.GetAppHeight() - 288;

            Entity pipe5 = Entity.CreateEntity("Pipe5");
            pipe5.AddComponent<CollisionComponent>();
            pipe5.AddComponent<Sprite>();
            pipe5.GetComponent<Sprite>().spriteId = "pipe_small";
            pipe5.Width = 96;
            pipe5.Height = 96;
            pipe5.PosX = pipe4.PosX + 5090;
            pipe5.PosY = app.GetAppHeight() - 192;

            Entity pipe6 = Entity.CreateEntity("Pipe6");
            pipe6.AddComponent<CollisionComponent>();
            pipe6.AddComponent<Sprite>();
            pipe6.GetComponent<Sprite>().spriteId = "pipe_small";
            pipe6.Width = 96;
            pipe6.Height = 96;
            pipe6.PosX = pipe5.PosX + 768;
            pipe6.PosY = app.GetAppHeight() - 192;

        }
        private static void renderStairs(SDLApp app)
        {
            Entity block = Entity.CreateEntity("Block");
            block.AddComponent<CollisionComponent>();
            block.AddComponent<Sprite>();
            block.GetComponent<Sprite>().spriteId = "block";
            block.Width = 48;
            block.Height = 48;
            block.PosX = 6432;
            block.PosY = app.GetAppHeight() - 144;

            Entity block2 = Entity.CreateEntity("Block2");
            block2.AddComponent<CollisionComponent>();
            block2.AddComponent<Sprite>();
            block2.GetComponent<Sprite>().spriteId = "block";
            block2.Width = 48;
            block2.Height = 48;
            block2.PosX = block.PosX + 48;
            block2.PosY = block.PosY;

            Entity block3 = Entity.CreateEntity("Block3");
            block3.AddComponent<CollisionComponent>();
            block3.AddComponent<Sprite>();
            block3.GetComponent<Sprite>().spriteId = "block";
            block3.Width = 48;
            block3.Height = 48;
            block3.PosX = block2.PosX;
            block3.PosY = block.PosY - 48;

            Entity block4 = Entity.CreateEntity("Block4");
            block4.AddComponent<CollisionComponent>();
            block4.AddComponent<Sprite>();
            block4.GetComponent<Sprite>().spriteId = "block";
            block4.Width = 48;
            block4.Height = 48;
            block4.PosX = block3.PosX + 48;
            block4.PosY = block2.PosY;

            Entity block5 = Entity.CreateEntity("Block5");
            block5.AddComponent<CollisionComponent>();
            block5.AddComponent<Sprite>();
            block5.GetComponent<Sprite>().spriteId = "block";
            block5.Width = 48;
            block5.Height = 48;
            block5.PosX = block4.PosX;
            block5.PosY = block4.PosY - 48;

            Entity block6 = Entity.CreateEntity("Block6");
            block6.AddComponent<CollisionComponent>();
            block6.AddComponent<Sprite>();
            block6.GetComponent<Sprite>().spriteId = "block";
            block6.Width = 48;
            block6.Height = 48;
            block6.PosX = block4.PosX;
            block6.PosY = block4.PosY - 96;

            Entity block7 = Entity.CreateEntity("Block7");
            block7.AddComponent<CollisionComponent>();
            block7.AddComponent<Sprite>();
            block7.GetComponent<Sprite>().spriteId = "block";
            block7.Width = 48;
            block7.Height = 48;
            block7.PosX = block6.PosX + 48;
            block7.PosY = block4.PosY;

            Entity block8 = Entity.CreateEntity("Block8");
            block8.AddComponent<CollisionComponent>();
            block8.AddComponent<Sprite>();
            block8.GetComponent<Sprite>().spriteId = "block";
            block8.Width = 48;
            block8.Height = 48;
            block8.PosX = block7.PosX;
            block8.PosY = block7.PosY - 48;

            Entity block9 = Entity.CreateEntity("Block9");
            block9.AddComponent<CollisionComponent>();
            block9.AddComponent<Sprite>();
            block9.GetComponent<Sprite>().spriteId = "block";
            block9.Width = 48;
            block9.Height = 48;
            block9.PosX = block7.PosX;
            block9.PosY = block7.PosY - 96;

            Entity block10 = Entity.CreateEntity("Block10");
            block10.AddComponent<CollisionComponent>();
            block10.AddComponent<Sprite>();
            block10.GetComponent<Sprite>().spriteId = "block";
            block10.Width = 48;
            block10.Height = 48;
            block10.PosX = block7.PosX;
            block10.PosY = block7.PosY - 144;

            Entity block11 = Entity.CreateEntity("Block11");
            block11.AddComponent<CollisionComponent>();
            block11.AddComponent<Sprite>();
            block11.GetComponent<Sprite>().spriteId = "block";
            block11.Width = 48;
            block11.Height = 48;
            block11.PosX = block10.PosX + 144;
            block11.PosY = block10.PosY;

            Entity block12 = Entity.CreateEntity("Block12");
            block12.AddComponent<CollisionComponent>();
            block12.AddComponent<Sprite>();
            block12.GetComponent<Sprite>().spriteId = "block";
            block12.Width = 48;
            block12.Height = 48;
            block12.PosX = block11.PosX;
            block12.PosY = block11.PosY + 48;

            Entity block13 = Entity.CreateEntity("Block13");
            block13.AddComponent<CollisionComponent>();
            block13.AddComponent<Sprite>();
            block13.GetComponent<Sprite>().spriteId = "block";
            block13.Width = 48;
            block13.Height = 48;
            block13.PosX = block11.PosX;
            block13.PosY = block11.PosY + 96;

            Entity block14 = Entity.CreateEntity("Block14");
            block14.AddComponent<CollisionComponent>();
            block14.AddComponent<Sprite>();
            block14.GetComponent<Sprite>().spriteId = "block";
            block14.Width = 48;
            block14.Height = 48;
            block14.PosX = block11.PosX;
            block14.PosY = block11.PosY + 144;

            Entity block15 = Entity.CreateEntity("Block15");
            block15.AddComponent<CollisionComponent>();
            block15.AddComponent<Sprite>();
            block15.GetComponent<Sprite>().spriteId = "block";
            block15.Width = 48;
            block15.Height = 48;
            block15.PosX = block11.PosX + 48;
            block15.PosY = block12.PosY;

            Entity block16 = Entity.CreateEntity("Block16");
            block16.AddComponent<CollisionComponent>();
            block16.AddComponent<Sprite>();
            block16.GetComponent<Sprite>().spriteId = "block";
            block16.Width = 48;
            block16.Height = 48;
            block16.PosX = block15.PosX;
            block16.PosY = block15.PosY + 48;

            Entity block17 = Entity.CreateEntity("Block17");
            block17.AddComponent<CollisionComponent>();
            block17.AddComponent<Sprite>();
            block17.GetComponent<Sprite>().spriteId = "block";
            block17.Width = 48;
            block17.Height = 48;
            block17.PosX = block15.PosX;
            block17.PosY = block15.PosY + 96;

            Entity block18 = Entity.CreateEntity("Block18");
            block18.AddComponent<CollisionComponent>();
            block18.AddComponent<Sprite>();
            block18.GetComponent<Sprite>().spriteId = "block";
            block18.Width = 48;
            block18.Height = 48;
            block18.PosX = block16.PosX + 48;
            block18.PosY = block16.PosY;

            Entity block19 = Entity.CreateEntity("Block19");
            block19.AddComponent<CollisionComponent>();
            block19.AddComponent<Sprite>();
            block19.GetComponent<Sprite>().spriteId = "block";
            block19.Width = 48;
            block19.Height = 48;
            block19.PosX = block18.PosX;
            block19.PosY = block18.PosY + 48;

            Entity block20 = Entity.CreateEntity("Block20");
            block20.AddComponent<CollisionComponent>();
            block20.AddComponent<Sprite>();
            block20.GetComponent<Sprite>().spriteId = "block";
            block20.Width = 48;
            block20.Height = 48;
            block20.PosX = block19.PosX + 48;
            block20.PosY = block19.PosY;

            Entity block21 = Entity.CreateEntity("Block21");
            block21.AddComponent<CollisionComponent>();
            block21.AddComponent<Sprite>();
            block21.GetComponent<Sprite>().spriteId = "block";
            block21.Width = 48;
            block21.Height = 48;
            block21.PosX = block20.PosX + 240;
            block21.PosY = block20.PosY;

            Entity block22 = Entity.CreateEntity("Block22");
            block22.AddComponent<CollisionComponent>();
            block22.AddComponent<Sprite>();
            block22.GetComponent<Sprite>().spriteId = "block";
            block22.Width = 48;
            block22.Height = 48;
            block22.PosX = block21.PosX + 48;
            block22.PosY = block21.PosY;

            
            Entity block23 = Entity.CreateEntity("Block23");
            block23.AddComponent<CollisionComponent>();
            block23.AddComponent<Sprite>();
            block23.GetComponent<Sprite>().spriteId = "block";
            block23.Width = 48;
            block23.Height = 48;
            block23.PosX = block22.PosX;
            block23.PosY = block22.PosY - 48;

            
            Entity block24 = Entity.CreateEntity("Block24");
            block24.AddComponent<CollisionComponent>();
            block24.AddComponent<Sprite>();
            block24.GetComponent<Sprite>().spriteId = "block";
            block24.Width = 48;
            block24.Height = 48;
            block24.PosX = block22.PosX + 48;
            block24.PosY = block22.PosY;

            Entity block25 = Entity.CreateEntity("Block25");
            block25.AddComponent<CollisionComponent>();
            block25.AddComponent<Sprite>();
            block25.GetComponent<Sprite>().spriteId = "block";
            block25.Width = 48;
            block25.Height = 48;
            block25.PosX = block24.PosX;
            block25.PosY = block24.PosY - 48;

            Entity block26 = Entity.CreateEntity("Block26");
            block26.AddComponent<CollisionComponent>();
            block26.AddComponent<Sprite>();
            block26.GetComponent<Sprite>().spriteId = "block";
            block26.Width = 48;
            block26.Height = 48;
            block26.PosX = block24.PosX;
            block26.PosY = block24.PosY - 96;

            Entity block27 = Entity.CreateEntity("Block27");
            block27.AddComponent<CollisionComponent>();
            block27.AddComponent<Sprite>();
            block27.GetComponent<Sprite>().spriteId = "block";
            block27.Width = 48;
            block27.Height = 48;
            block27.PosX = block24.PosX + 48;
            block27.PosY = block24.PosY;

            Entity block28 = Entity.CreateEntity("Block28");
            block28.AddComponent<CollisionComponent>();
            block28.AddComponent<Sprite>();
            block28.GetComponent<Sprite>().spriteId = "block";
            block28.Width = 48;
            block28.Height = 48;
            block28.PosX = block27.PosX;
            block28.PosY = block27.PosY - 48;

            Entity block29 = Entity.CreateEntity("Block29");
            block29.AddComponent<CollisionComponent>();
            block29.AddComponent<Sprite>();
            block29.GetComponent<Sprite>().spriteId = "block";
            block29.Width = 48;
            block29.Height = 48;
            block29.PosX = block27.PosX;
            block29.PosY = block27.PosY - 96;

            Entity block30 = Entity.CreateEntity("Block30");
            block30.AddComponent<CollisionComponent>();
            block30.AddComponent<Sprite>();
            block30.GetComponent<Sprite>().spriteId = "block";
            block30.Width = 48;
            block30.Height = 48;
            block30.PosX = block27.PosX;
            block30.PosY = block27.PosY - 144;

            Entity block31 = Entity.CreateEntity("Block31");
            block31.AddComponent<CollisionComponent>();
            block31.AddComponent<Sprite>();
            block31.GetComponent<Sprite>().spriteId = "block";
            block31.Width = 48;
            block31.Height = 48;
            block31.PosX = block27.PosX + 48;
            block31.PosY = block27.PosY;

            Entity block32 = Entity.CreateEntity("Block32");
            block32.AddComponent<CollisionComponent>();
            block32.AddComponent<Sprite>();
            block32.GetComponent<Sprite>().spriteId = "block";
            block32.Width = 48;
            block32.Height = 48;
            block32.PosX = block31.PosX;
            block32.PosY = block31.PosY - 48;

            Entity block33 = Entity.CreateEntity("Block33");
            block33.AddComponent<CollisionComponent>();
            block33.AddComponent<Sprite>();
            block33.GetComponent<Sprite>().spriteId = "block";
            block33.Width = 48;
            block33.Height = 48;
            block33.PosX = block31.PosX;
            block33.PosY = block31.PosY - 96;

            Entity block34 = Entity.CreateEntity("Block34");
            block34.AddComponent<CollisionComponent>();
            block34.AddComponent<Sprite>();
            block34.GetComponent<Sprite>().spriteId = "block";
            block34.Width = 48;
            block34.Height = 48;
            block34.PosX = block31.PosX;
            block34.PosY = block31.PosY - 144;

            Entity block35 = Entity.CreateEntity("Block35");
            block35.AddComponent<CollisionComponent>();
            block35.AddComponent<Sprite>();
            block35.GetComponent<Sprite>().spriteId = "block";
            block35.Width = 48;
            block35.Height = 48;
            block35.PosX = block31.PosX + 144;
            block35.PosY = block31.PosY - 144;

            Entity block36 = Entity.CreateEntity("Block36");
            block36.AddComponent<CollisionComponent>();
            block36.AddComponent<Sprite>();
            block36.GetComponent<Sprite>().spriteId = "block";
            block36.Width = 48;
            block36.Height = 48;
            block36.PosX = block35.PosX;
            block36.PosY = block35.PosY + 48;

            Entity block37 = Entity.CreateEntity("Block37");
            block37.AddComponent<CollisionComponent>();
            block37.AddComponent<Sprite>();
            block37.GetComponent<Sprite>().spriteId = "block";
            block37.Width = 48;
            block37.Height = 48;
            block37.PosX = block35.PosX;
            block37.PosY = block35.PosY + 96;

            Entity block38 = Entity.CreateEntity("Block38");
            block38.AddComponent<CollisionComponent>();
            block38.AddComponent<Sprite>();
            block38.GetComponent<Sprite>().spriteId = "block";
            block38.Width = 48;
            block38.Height = 48;
            block38.PosX = block35.PosX;
            block38.PosY = block35.PosY + 144;

            Entity block39 = Entity.CreateEntity("Block39");
            block39.AddComponent<CollisionComponent>();
            block39.AddComponent<Sprite>();
            block39.GetComponent<Sprite>().spriteId = "block";
            block39.Width = 48;
            block39.Height = 48;
            block39.PosX = block38.PosX + 48;
            block39.PosY = block38.PosY;

            Entity block40 = Entity.CreateEntity("Block40");
            block40.AddComponent<CollisionComponent>();
            block40.AddComponent<Sprite>();
            block40.GetComponent<Sprite>().spriteId = "block";
            block40.Width = 48;
            block40.Height = 48;
            block40.PosX = block39.PosX;
            block40.PosY = block39.PosY - 48;

            Entity block41 = Entity.CreateEntity("Block41");
            block41.AddComponent<CollisionComponent>();
            block41.AddComponent<Sprite>();
            block41.GetComponent<Sprite>().spriteId = "block";
            block41.Width = 48;
            block41.Height = 48;
            block41.PosX = block39.PosX;
            block41.PosY = block39.PosY - 96;

            Entity block42 = Entity.CreateEntity("Block42");
            block42.AddComponent<CollisionComponent>();
            block42.AddComponent<Sprite>();
            block42.GetComponent<Sprite>().spriteId = "block";
            block42.Width = 48;
            block42.Height = 48;
            block42.PosX = block39.PosX + 48;
            block42.PosY = block39.PosY;

            Entity block43 = Entity.CreateEntity("Block43");
            block43.AddComponent<CollisionComponent>();
            block43.AddComponent<Sprite>();
            block43.GetComponent<Sprite>().spriteId = "block";
            block43.Width = 48;
            block43.Height = 48;
            block43.PosX = block42.PosX;
            block43.PosY = block42.PosY - 48;

            Entity block44 = Entity.CreateEntity("Block44");
            block44.AddComponent<CollisionComponent>();
            block44.AddComponent<Sprite>();
            block44.GetComponent<Sprite>().spriteId = "block";
            block44.Width = 48;
            block44.Height = 48;
            block44.PosX = block42.PosX + 48;
            block44.PosY = block42.PosY;

            Entity block45 = Entity.CreateEntity("Block45");
            block45.AddComponent<CollisionComponent>();
            block45.AddComponent<Sprite>();
            block45.GetComponent<Sprite>().spriteId = "block";
            block45.Width = 48;
            block45.Height = 48;
            block45.PosX = block44.PosX + 1104;
            block45.PosY = block44.PosY;

            Entity block46 = Entity.CreateEntity("Block46");
            block46.AddComponent<CollisionComponent>();
            block46.AddComponent<Sprite>();
            block46.GetComponent<Sprite>().spriteId = "block";
            block46.Width = 48;
            block46.Height = 48;
            block46.PosX = block45.PosX + 48;
            block46.PosY = block45.PosY;

            Entity block47 = Entity.CreateEntity("Block47");
            block47.AddComponent<CollisionComponent>();
            block47.AddComponent<Sprite>();
            block47.GetComponent<Sprite>().spriteId = "block";
            block47.Width = 48;
            block47.Height = 48;
            block47.PosX = block46.PosX;
            block47.PosY = block46.PosY - 48;

            Entity block48 = Entity.CreateEntity("Block48");
            block48.AddComponent<CollisionComponent>();
            block48.AddComponent<Sprite>();
            block48.GetComponent<Sprite>().spriteId = "block";
            block48.Width = 48;
            block48.Height = 48;
            block48.PosX = block46.PosX + 48;
            block48.PosY = block46.PosY;

            Entity block49 = Entity.CreateEntity("Block49");
            block49.AddComponent<CollisionComponent>();
            block49.AddComponent<Sprite>();
            block49.GetComponent<Sprite>().spriteId = "block";
            block49.Width = 48;
            block49.Height = 48;
            block49.PosX = block48.PosX;
            block49.PosY = block48.PosY - 48;

            Entity block50 = Entity.CreateEntity("Block50");
            block50.AddComponent<CollisionComponent>();
            block50.AddComponent<Sprite>();
            block50.GetComponent<Sprite>().spriteId = "block";
            block50.Width = 48;
            block50.Height = 48;
            block50.PosX = block48.PosX;
            block50.PosY = block48.PosY - 96;

            Entity block51 = Entity.CreateEntity("Block51");
            block51.AddComponent<CollisionComponent>();
            block51.AddComponent<Sprite>();
            block51.GetComponent<Sprite>().spriteId = "block";
            block51.Width = 48;
            block51.Height = 48;
            block51.PosX = block48.PosX + 48;
            block51.PosY = block48.PosY;

            Entity block52 = Entity.CreateEntity("Block52");
            block52.AddComponent<CollisionComponent>();
            block52.AddComponent<Sprite>();
            block52.GetComponent<Sprite>().spriteId = "block";
            block52.Width = 48;
            block52.Height = 48;
            block52.PosX = block51.PosX;
            block52.PosY = block51.PosY - 48;

            Entity block53 = Entity.CreateEntity("Block53");
            block53.AddComponent<CollisionComponent>();
            block53.AddComponent<Sprite>();
            block53.GetComponent<Sprite>().spriteId = "block";
            block53.Width = 48;
            block53.Height = 48;
            block53.PosX = block51.PosX;
            block53.PosY = block51.PosY - 96;

            Entity block54 = Entity.CreateEntity("Block54");
            block54.AddComponent<CollisionComponent>();
            block54.AddComponent<Sprite>();
            block54.GetComponent<Sprite>().spriteId = "block";
            block54.Width = 48;
            block54.Height = 48;
            block54.PosX = block51.PosX;
            block54.PosY = block51.PosY - 144;

            Entity block55 = Entity.CreateEntity("Block55");
            block55.AddComponent<CollisionComponent>();
            block55.AddComponent<Sprite>();
            block55.GetComponent<Sprite>().spriteId = "block";
            block55.Width = 48;
            block55.Height = 48;
            block55.PosX = block51.PosX + 48;
            block55.PosY = block51.PosY;

            Entity block56 = Entity.CreateEntity("Block56");
            block56.AddComponent<CollisionComponent>();
            block56.AddComponent<Sprite>();
            block56.GetComponent<Sprite>().spriteId = "block";
            block56.Width = 48;
            block56.Height = 48;
            block56.PosX = block55.PosX;
            block56.PosY = block55.PosY - 48;

            Entity block57 = Entity.CreateEntity("Block57");
            block57.AddComponent<CollisionComponent>();
            block57.AddComponent<Sprite>();
            block57.GetComponent<Sprite>().spriteId = "block";
            block57.Width = 48;
            block57.Height = 48;
            block57.PosX = block55.PosX;
            block57.PosY = block55.PosY - 96;

            Entity block58 = Entity.CreateEntity("Block58");
            block58.AddComponent<CollisionComponent>();
            block58.AddComponent<Sprite>();
            block58.GetComponent<Sprite>().spriteId = "block";
            block58.Width = 48;
            block58.Height = 48;
            block58.PosX = block55.PosX;
            block58.PosY = block55.PosY - 144;

            Entity block59 = Entity.CreateEntity("Block59");
            block59.AddComponent<CollisionComponent>();
            block59.AddComponent<Sprite>();
            block59.GetComponent<Sprite>().spriteId = "block";
            block59.Width = 48;
            block59.Height = 48;
            block59.PosX = block55.PosX;
            block59.PosY = block55.PosY - 192;

            Entity block60 = Entity.CreateEntity("Block60");
            block60.AddComponent<CollisionComponent>();
            block60.AddComponent<Sprite>();
            block60.GetComponent<Sprite>().spriteId = "block";
            block60.Width = 48;
            block60.Height = 48;
            block60.PosX = block55.PosX + 48;
            block60.PosY = block55.PosY;

            Entity block61 = Entity.CreateEntity("Block61");
            block61.AddComponent<CollisionComponent>();
            block61.AddComponent<Sprite>();
            block61.GetComponent<Sprite>().spriteId = "block";
            block61.Width = 48;
            block61.Height = 48;
            block61.PosX = block60.PosX;
            block61.PosY = block60.PosY - 48;

            Entity block62 = Entity.CreateEntity("Block62");
            block62.AddComponent<CollisionComponent>();
            block62.AddComponent<Sprite>();
            block62.GetComponent<Sprite>().spriteId = "block";
            block62.Width = 48;
            block62.Height = 48;
            block62.PosX = block60.PosX;
            block62.PosY = block60.PosY - 96;

            Entity block63 = Entity.CreateEntity("Block63");
            block63.AddComponent<CollisionComponent>();
            block63.AddComponent<Sprite>();
            block63.GetComponent<Sprite>().spriteId = "block";
            block63.Width = 48;
            block63.Height = 48;
            block63.PosX = block60.PosX;
            block63.PosY = block60.PosY - 144;

            Entity block64 = Entity.CreateEntity("Block64");
            block64.AddComponent<CollisionComponent>();
            block64.AddComponent<Sprite>();
            block64.GetComponent<Sprite>().spriteId = "block";
            block64.Width = 48;
            block64.Height = 48;
            block64.PosX = block60.PosX;
            block64.PosY = block60.PosY - 192;

            Entity block65 = Entity.CreateEntity("Block65");
            block65.AddComponent<CollisionComponent>();
            block65.AddComponent<Sprite>();
            block65.GetComponent<Sprite>().spriteId = "block";
            block65.Width = 48;
            block65.Height = 48;
            block65.PosX = block60.PosX;
            block65.PosY = block60.PosY - 240;

            Entity block66 = Entity.CreateEntity("Block66");
            block66.AddComponent<CollisionComponent>();
            block66.AddComponent<Sprite>();
            block66.GetComponent<Sprite>().spriteId = "block";
            block66.Width = 48;
            block66.Height = 48;
            block66.PosX = block60.PosX + 48;
            block66.PosY = block60.PosY;

            Entity block67 = Entity.CreateEntity("Block67");
            block67.AddComponent<CollisionComponent>();
            block67.AddComponent<Sprite>();
            block67.GetComponent<Sprite>().spriteId = "block";
            block67.Width = 48;
            block67.Height = 48;
            block67.PosX = block66.PosX;
            block67.PosY = block66.PosY - 48;

            Entity block68 = Entity.CreateEntity("Block68");
            block68.AddComponent<CollisionComponent>();
            block68.AddComponent<Sprite>();
            block68.GetComponent<Sprite>().spriteId = "block";
            block68.Width = 48;
            block68.Height = 48;
            block68.PosX = block66.PosX;
            block68.PosY = block66.PosY - 96;

            Entity block69 = Entity.CreateEntity("Block69");
            block69.AddComponent<CollisionComponent>();
            block69.AddComponent<Sprite>();
            block69.GetComponent<Sprite>().spriteId = "block";
            block69.Width = 48;
            block69.Height = 48;
            block69.PosX = block66.PosX;
            block69.PosY = block66.PosY - 144;

            Entity block70 = Entity.CreateEntity("Block70");
            block70.AddComponent<CollisionComponent>();
            block70.AddComponent<Sprite>();
            block70.GetComponent<Sprite>().spriteId = "block";
            block70.Width = 48;
            block70.Height = 48;
            block70.PosX = block66.PosX;
            block70.PosY = block66.PosY - 192;

            Entity block71 = Entity.CreateEntity("Block71");
            block71.AddComponent<CollisionComponent>();
            block71.AddComponent<Sprite>();
            block71.GetComponent<Sprite>().spriteId = "block";
            block71.Width = 48;
            block71.Height = 48;
            block71.PosX = block66.PosX;
            block71.PosY = block66.PosY - 240;

            Entity block72 = Entity.CreateEntity("Block72");
            block72.AddComponent<CollisionComponent>();
            block72.AddComponent<Sprite>();
            block72.GetComponent<Sprite>().spriteId = "block";
            block72.Width = 48;
            block72.Height = 48;
            block72.PosX = block66.PosX;
            block72.PosY = block66.PosY - 288;

            Entity block73 = Entity.CreateEntity("Block73");
            block73.AddComponent<CollisionComponent>();
            block73.AddComponent<Sprite>();
            block73.GetComponent<Sprite>().spriteId = "block";
            block73.Width = 48;
            block73.Height = 48;
            block73.PosX = block66.PosX + 48;
            block73.PosY = block66.PosY;

            Entity block74 = Entity.CreateEntity("Block74");
            block74.AddComponent<CollisionComponent>();
            block74.AddComponent<Sprite>();
            block74.GetComponent<Sprite>().spriteId = "block";
            block74.Width = 48;
            block74.Height = 48;
            block74.PosX = block73.PosX;
            block74.PosY = block73.PosY - 48;

            Entity block75 = Entity.CreateEntity("Block75");
            block75.AddComponent<CollisionComponent>();
            block75.AddComponent<Sprite>();
            block75.GetComponent<Sprite>().spriteId = "block";
            block75.Width = 48;
            block75.Height = 48;
            block75.PosX = block73.PosX;
            block75.PosY = block73.PosY - 96;

            Entity block76 = Entity.CreateEntity("Block76");
            block76.AddComponent<CollisionComponent>();
            block76.AddComponent<Sprite>();
            block76.GetComponent<Sprite>().spriteId = "block";
            block76.Width = 48;
            block76.Height = 48;
            block76.PosX = block73.PosX;
            block76.PosY = block73.PosY - 144;

            Entity block77 = Entity.CreateEntity("Block77");
            block77.AddComponent<CollisionComponent>();
            block77.AddComponent<Sprite>();
            block77.GetComponent<Sprite>().spriteId = "block";
            block77.Width = 48;
            block77.Height = 48;
            block77.PosX = block73.PosX;
            block77.PosY = block73.PosY - 192;

            Entity block78 = Entity.CreateEntity("Block78");
            block78.AddComponent<CollisionComponent>();
            block78.AddComponent<Sprite>();
            block78.GetComponent<Sprite>().spriteId = "block";
            block78.Width = 48;
            block78.Height = 48;
            block78.PosX = block73.PosX;
            block78.PosY = block73.PosY - 240;

            Entity block79 = Entity.CreateEntity("Block79");
            block79.AddComponent<CollisionComponent>();
            block79.AddComponent<Sprite>();
            block79.GetComponent<Sprite>().spriteId = "block";
            block79.Width = 48;
            block79.Height = 48;
            block79.PosX = block73.PosX;
            block79.PosY = block73.PosY - 288;

            Entity block80 = Entity.CreateEntity("Block80");
            block80.AddComponent<CollisionComponent>();
            block80.AddComponent<Sprite>();
            block80.GetComponent<Sprite>().spriteId = "block";
            block80.Width = 48;
            block80.Height = 48;
            block80.PosX = block73.PosX;
            block80.PosY = block73.PosY - 336;

            Entity block81 = Entity.CreateEntity("Block81");
            block81.AddComponent<CollisionComponent>();
            block81.AddComponent<Sprite>();
            block81.GetComponent<Sprite>().spriteId = "block";
            block81.Width = 48;
            block81.Height = 48;
            block81.PosX = block73.PosX + 48;
            block81.PosY = block73.PosY;

            Entity block82 = Entity.CreateEntity("Block82");
            block82.AddComponent<CollisionComponent>();
            block82.AddComponent<Sprite>();
            block82.GetComponent<Sprite>().spriteId = "block";
            block82.Width = 48;
            block82.Height = 48;
            block82.PosX = block81.PosX;
            block82.PosY = block81.PosY - 48;

            Entity block83 = Entity.CreateEntity("Block83");
            block83.AddComponent<CollisionComponent>();
            block83.AddComponent<Sprite>();
            block83.GetComponent<Sprite>().spriteId = "block";
            block83.Width = 48;
            block83.Height = 48;
            block83.PosX = block81.PosX;
            block83.PosY = block81.PosY - 96;

            Entity block84 = Entity.CreateEntity("Block84");
            block84.AddComponent<CollisionComponent>();
            block84.AddComponent<Sprite>();
            block84.GetComponent<Sprite>().spriteId = "block";
            block84.Width = 48;
            block84.Height = 48;
            block84.PosX = block81.PosX;
            block84.PosY = block81.PosY - 144;

            Entity block85 = Entity.CreateEntity("Block85");
            block85.AddComponent<CollisionComponent>();
            block85.AddComponent<Sprite>();
            block85.GetComponent<Sprite>().spriteId = "block";
            block85.Width = 48;
            block85.Height = 48;
            block85.PosX = block81.PosX;
            block85.PosY = block81.PosY - 192;

            Entity block86 = Entity.CreateEntity("Block86");
            block86.AddComponent<CollisionComponent>();
            block86.AddComponent<Sprite>();
            block86.GetComponent<Sprite>().spriteId = "block";
            block86.Width = 48;
            block86.Height = 48;
            block86.PosX = block81.PosX;
            block86.PosY = block81.PosY - 240;

            Entity block87 = Entity.CreateEntity("Block87");
            block87.AddComponent<CollisionComponent>();
            block87.AddComponent<Sprite>();
            block87.GetComponent<Sprite>().spriteId = "block";
            block87.Width = 48;
            block87.Height = 48;
            block87.PosX = block81.PosX;
            block87.PosY = block81.PosY - 288;

            Entity block88 = Entity.CreateEntity("Block88");
            block88.AddComponent<CollisionComponent>();
            block88.AddComponent<Sprite>();
            block88.GetComponent<Sprite>().spriteId = "block";
            block88.Width = 48;
            block88.Height = 48;
            block88.PosX = block81.PosX;
            block88.PosY = block81.PosY - 336;

            Entity block89 = Entity.CreateEntity("Block89");
            block89.AddComponent<CollisionComponent>();
            block89.AddComponent<Sprite>();
            block89.GetComponent<Sprite>().spriteId = "block";
            block89.Width = 48;
            block89.Height = 48;
            block89.PosX = block81.PosX + 432;
            block89.PosY = block81.PosY;
        } 
        private static void spawnEnemiesAtStartLocation(SDLApp app)
        {
            Entity flag = Entity.CreateEntity("flag");
            flag.AddComponent<CollisionComponent>();
            flag.AddComponent<Sprite>();
            flag.GetComponent<Sprite>().spriteId = "full-flag";
            flag.Width = 96;
            flag.Height = 480;
            flag.PosX = 9456;
            flag.PosY = app.GetAppHeight() - 624;
            CollisionComponent flagCollision = flag.GetComponent<CollisionComponent>();
            flagCollision.IsTrigger = true;
            flagCollision.Overlaped += e =>
            {
                if (e.LastContact.Name.Equals("mario"))
                {
                    Entity.GetEntity("mario", true).Destroy();
                }
            };

            Entity goomba = Entity.CreateEntity("goomba");
            goomba.AddComponent<CharacterMovementComponent>();
            goomba.AddComponent<Gumba>();
            goomba.AddComponent<CollisionComponent>();
            goomba.AddComponent<Sprite>();
            goomba.GetComponent<Sprite>().spriteId = "goomba";
            goomba.GetComponent<CollisionComponent>().IsStatic = false;
            goomba.Width = 48;
            goomba.Height = 48;
            goomba.PosX = 1056;
            goomba.PosY = app.GetAppHeight() - 144;

            Entity goomba2 = Entity.CreateEntity("goomba2");
            goomba2.AddComponent<CharacterMovementComponent>();
            goomba2.AddComponent<Gumba>();
            goomba2.AddComponent<CollisionComponent>();
            goomba2.AddComponent<Sprite>();
            goomba2.GetComponent<Sprite>().spriteId = "goomba";
            goomba2.GetComponent<CollisionComponent>().IsStatic = false;
            goomba2.Width = 48;
            goomba2.Height = 48;
            goomba2.PosX = goomba.PosX + 864;
            goomba2.PosY = app.GetAppHeight() - 144;

            Entity goomba3 = Entity.CreateEntity("goomba3");
            goomba3.AddComponent<CharacterMovementComponent>();
            goomba3.AddComponent<Gumba>();
            goomba3.AddComponent<CollisionComponent>();
            goomba3.AddComponent<Sprite>();
            goomba3.GetComponent<Sprite>().spriteId = "goomba";
            goomba3.GetComponent<CollisionComponent>().IsStatic = false;
            goomba3.Width = 48;
            goomba3.Height = 48;
            goomba3.PosX = goomba2.PosX + 528;
            goomba3.PosY = app.GetAppHeight() - 144;

            Entity goomba4 = Entity.CreateEntity("goomba4");
            goomba4.AddComponent<CharacterMovementComponent>();
            goomba4.AddComponent<Gumba>();
            goomba4.AddComponent<CollisionComponent>();
            goomba4.AddComponent<Sprite>();
            goomba4.GetComponent<Sprite>().spriteId = "goomba";
            goomba4.GetComponent<CollisionComponent>().IsStatic = false;
            goomba4.Width = 48;
            goomba4.Height = 48;
            goomba4.PosX = goomba3.PosX + 72;
            goomba4.PosY = app.GetAppHeight() - 144;

            Entity goomba5 = Entity.CreateEntity("goomba5");
            goomba5.AddComponent<CharacterMovementComponent>();
            goomba5.AddComponent<Gumba>();
            goomba5.AddComponent<CollisionComponent>();
            goomba5.AddComponent<Sprite>();
            goomba5.GetComponent<Sprite>().spriteId = "goomba";
            goomba5.GetComponent<CollisionComponent>().IsStatic = false;
            goomba5.Width = 48;
            goomba5.Height = 48;
            goomba5.PosX = goomba3.PosX + 1392;
            goomba5.PosY = goomba3.PosY - 384;

            Entity goomba6 = Entity.CreateEntity("goomba6");
            goomba6.AddComponent<CharacterMovementComponent>();
            goomba6.AddComponent<Gumba>();
            goomba6.AddComponent<CollisionComponent>();
            goomba6.AddComponent<Sprite>();
            goomba6.GetComponent<Sprite>().spriteId = "goomba";
            goomba6.GetComponent<CollisionComponent>().IsStatic = false;
            goomba6.Width = 48;
            goomba6.Height = 48;
            goomba6.PosX = goomba5.PosX + 96;
            goomba6.PosY = goomba5.PosY;

            Entity goomba7 = Entity.CreateEntity("goomba7");
            goomba7.AddComponent<CharacterMovementComponent>();
            goomba7.AddComponent<Gumba>();
            goomba7.AddComponent<CollisionComponent>();
            goomba7.AddComponent<Sprite>();
            goomba7.GetComponent<Sprite>().spriteId = "goomba";
            goomba7.GetComponent<CollisionComponent>().IsStatic = false;
            goomba7.Width = 48;
            goomba7.Height = 48;
            goomba7.PosX = goomba3.PosX + 2208;
            goomba7.PosY = goomba3.PosY;

            Entity goomba8 = Entity.CreateEntity("goomba8");
            goomba8.AddComponent<CharacterMovementComponent>();
            goomba8.AddComponent<Gumba>();
            goomba8.AddComponent<CollisionComponent>();
            goomba8.AddComponent<Sprite>();
            goomba8.GetComponent<Sprite>().spriteId = "goomba";
            goomba8.GetComponent<CollisionComponent>().IsStatic = false;
            goomba8.Width = 48;
            goomba8.Height = 48;
            goomba8.PosX = goomba7.PosX + 72;
            goomba8.PosY = goomba7.PosY;

            Entity koopa = Entity.CreateEntity("Koopa");
            koopa.AddComponent<CharacterMovementComponent>();
            koopa.AddComponent<CollisionComponent>();
            koopa.AddComponent<Sprite>();
            koopa.GetComponent<Sprite>().spriteId = "koopa";
            //koopa.GetComponent<CollisionComponent>().IsStatic = false;
            koopa.Width = 48;
            koopa.Height = 36;
            koopa.PosX = goomba7.PosX + 480;
            koopa.PosY = goomba7.PosY - 48;

            Entity goomba9 = Entity.CreateEntity("goomba9");
            goomba9.AddComponent<CharacterMovementComponent>();
            goomba9.AddComponent<Gumba>();
            goomba9.AddComponent<CollisionComponent>();
            goomba9.AddComponent<Sprite>();
            goomba9.GetComponent<Sprite>().spriteId = "goomba";
            goomba9.GetComponent<CollisionComponent>().IsStatic = false;
            goomba9.Width = 48;
            goomba9.Height = 48;
            goomba9.PosX = goomba7.PosX + 816;
            goomba9.PosY = goomba7.PosY;

            Entity goomba10 = Entity.CreateEntity("goomba10");
            goomba10.AddComponent<CharacterMovementComponent>();
            goomba10.AddComponent<Gumba>();
            goomba10.AddComponent<CollisionComponent>();
            goomba10.AddComponent<Sprite>();
            goomba10.GetComponent<Sprite>().spriteId = "goomba";
            goomba10.GetComponent<CollisionComponent>().IsStatic = false;
            goomba10.Width = 48;
            goomba10.Height = 48;
            goomba10.PosX = goomba9.PosX + 72;
            goomba10.PosY = goomba9.PosY;

            Entity goomba11 = Entity.CreateEntity("goomba11");
            goomba11.AddComponent<CharacterMovementComponent>();
            goomba11.AddComponent<Gumba>();
            goomba11.AddComponent<CollisionComponent>();
            goomba11.AddComponent<Sprite>();
            goomba11.GetComponent<Sprite>().spriteId = "goomba";
            goomba11.GetComponent<CollisionComponent>().IsStatic = false;
            goomba11.Width = 48;
            goomba11.Height = 48;
            goomba11.PosX = goomba9.PosX + 480;
            goomba11.PosY = goomba9.PosY;

            Entity goomba12 = Entity.CreateEntity("goomba12");
            goomba12.AddComponent<CharacterMovementComponent>();
            goomba12.AddComponent<Gumba>();
            goomba12.AddComponent<CollisionComponent>();
            goomba12.AddComponent<Sprite>();
            goomba12.GetComponent<Sprite>().spriteId = "goomba";
            goomba12.GetComponent<CollisionComponent>().IsStatic = false;
            goomba12.Width = 48;
            goomba12.Height = 48;
            goomba12.PosX = goomba11.PosX + 72;
            goomba12.PosY = goomba11.PosY;

            Entity goomba13 = Entity.CreateEntity("goomba13");
            goomba13.AddComponent<CharacterMovementComponent>();
            goomba13.AddComponent<Gumba>();
            goomba13.AddComponent<CollisionComponent>();
            goomba13.AddComponent<Sprite>();
            goomba13.GetComponent<Sprite>().spriteId = "goomba";
            goomba13.GetComponent<CollisionComponent>().IsStatic = false;
            goomba13.Width = 48;
            goomba13.Height = 48;
            goomba13.PosX = goomba11.PosX + 192;
            goomba13.PosY = goomba11.PosY;

            Entity goomba14 = Entity.CreateEntity("goomba14");
            goomba14.AddComponent<CharacterMovementComponent>();
            goomba14.AddComponent<Gumba>();
            goomba14.AddComponent<CollisionComponent>();
            goomba14.AddComponent<Sprite>();
            goomba14.GetComponent<Sprite>().spriteId = "goomba";
            goomba14.GetComponent<CollisionComponent>().IsStatic = false;
            goomba14.Width = 48;
            goomba14.Height = 48;
            goomba14.PosX = goomba13.PosX + 72;
            goomba14.PosY = goomba13.PosY;

            Entity goomba15 = Entity.CreateEntity("goomba15");
            goomba15.AddComponent<CharacterMovementComponent>();
            goomba15.AddComponent<Gumba>();
            goomba15.AddComponent<CollisionComponent>();
            goomba15.AddComponent<Sprite>();
            goomba15.GetComponent<Sprite>().spriteId = "goomba";
            goomba15.GetComponent<CollisionComponent>().IsStatic = false;
            goomba15.Width = 48;
            goomba15.Height = 48;
            goomba15.PosX = goomba13.PosX + 2208;
            goomba15.PosY = goomba13.PosY;

            Entity goomba16 = Entity.CreateEntity("goomba16");
            goomba16.AddComponent<CharacterMovementComponent>();
            goomba16.AddComponent<Gumba>();
            goomba16.AddComponent<CollisionComponent>();
            goomba16.AddComponent<Sprite>();
            goomba16.GetComponent<Sprite>().spriteId = "goomba";
            goomba16.GetComponent<CollisionComponent>().IsStatic = false;
            goomba16.Width = 48;
            goomba16.Height = 48;
            goomba16.PosX = goomba15.PosX + 72;
            goomba16.PosY = goomba15.PosY;

            AnimationData koopaData = new AnimationData();
            koopaData.StartFrame = 0;
            koopaData.EndFrame = 1;
            koopaData.FrameRatePerSecond = 4;
            koopaData.Width = 48;
            koopaData.Height = 36;

            AnimationData gumbaaData = new AnimationData();
            gumbaaData.StartFrame = 0;
            gumbaaData.EndFrame = 1;
            gumbaaData.FrameRatePerSecond = 4;
            gumbaaData.Width = 48;
            gumbaaData.Height = 48;

            koopa.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, koopaData);
            goomba.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba2.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba3.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba4.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba5.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba6.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba7.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba8.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba9.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba10.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba11.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba12.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba13.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba14.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba15.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            goomba16.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, gumbaaData);
            koopa.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba2.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba3.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba4.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba5.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba6.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba7.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba8.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba9.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba10.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba11.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba12.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba13.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba14.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba15.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
            goomba16.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);
        }
    }
}
