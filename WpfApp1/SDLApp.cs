using ConsoleApp1;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        public static bool ShouldShowFps = false;

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
            InitSound();

            _instance = this;
        }

        private void InitSound()
        {
            MIX_InitFlags mixerFlags = MIX_InitFlags.MIX_INIT_FLAC | MIX_InitFlags.MIX_INIT_OGG;
            int intMixerFlags = (int)(mixerFlags);

            if (Mix_Init(mixerFlags) != intMixerFlags)
            {
                FreeResources();
                throw new ApplicationException("Sound library initialization failed");
            }

            MixerInitializationParams mixerParams = new MixerInitializationParams();
            mixerParams.SetDefaults();
            mixerParams.PassToOpenAudio();
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

        public static float GlobalDeltaTime = 0.0f;

        public void Run()
        {
            Debug.Assert(!_window.Equals(IntPtr.Zero));

            uint lastTick = SDL_GetTicks();

            while (_isOpen)
            {
                while (SDL_PollEvent(out SDL_Event systemEvent) != 0)
                {
                    OnSystemEventOccured(systemEvent);
                }

                uint tick_time = SDL_GetTicks();
                GlobalDeltaTime = (tick_time - lastTick) / 1000.0f;
                lastTick = tick_time;

                Entity.rootEntity.Tick(GlobalDeltaTime);

                SDLRendering.ClearFrame();
                Entity.rootEntity.ReceiveRender();
                SDLRendering.RenderFrame();

                if (Math.Abs(Entity.GetEntity("goomba", true).posX
                    -
                    Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)

                    Entity.GetEntity("goomba", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba2", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba2", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba2", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba3", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba3", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba3", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba4", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba4", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba4", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba5", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba5", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba5", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba6", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba6", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba6", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba7", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba7", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba7", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba8", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba8", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba8", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba9", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba9", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba9", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba10", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba10", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba10", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba11", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba11", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba11", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba12", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba12", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba12", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba13", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba13", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba13", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba14", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba14", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba14", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba15", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba15", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba15", true).GetComponent<Sprite>().shouldMove = false;
                if (Math.Abs(Entity.GetEntity("goomba16", true).posX - Entity.GetEntity("mario", true).posX) < 500 && canStartGoomba)
                    Entity.GetEntity("goomba16", true).GetComponent<Sprite>().shouldMove = true;
                else
                    Entity.GetEntity("goomba16", true).GetComponent<Sprite>().shouldMove = false;
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

        static byte[] keys = null;
        static IntPtr keyArrayFromNative = IntPtr.Zero;

        public static bool GetKey(SDL_Keycode _keycode)
        {
            if (keyArrayFromNative == IntPtr.Zero)
            {
                keyArrayFromNative = SDL_GetKeyboardState(out int numKeys);
                if (keys == null)
                {
                    keys = new byte[numKeys];
                }
            }

            byte keycode = (byte)SDL_GetScancodeFromKey(_keycode);
            Marshal.Copy(keyArrayFromNative, keys, 0, keys.Length);
            return keys[keycode] == 1;
        }

        public static void Main(string[] args)
        {
            ShouldShowFps = true;

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
            background.width = width;
            background.height = height;
            background.posY = 40;
            SDLRendering.SetWorldBounds(width, height);
        }

        private static void createDownPlatform(SDLApp app)
        {
            Entity firstPlatform = Entity.CreateEntity("firstPlatform");
            firstPlatform.AddComponent<CollisionComponent>();
            firstPlatform.AddComponent<Sprite>();
            firstPlatform.GetComponent<Sprite>().spriteId = "first_platform";
            firstPlatform.width = 3312;
            firstPlatform.height = 96;
            firstPlatform.posX = 0;
            firstPlatform.posY = app.GetAppHeight() - 96;

            Entity secondPlatform = Entity.CreateEntity("secondPlatform");
            secondPlatform.AddComponent<CollisionComponent>();
            secondPlatform.AddComponent<Sprite>();
            secondPlatform.GetComponent<Sprite>().spriteId = "second_platform";
            secondPlatform.width = 720;
            secondPlatform.height = 96;
            secondPlatform.posX = firstPlatform.width + firstPlatform.posX + 96;
            secondPlatform.posY = firstPlatform.posY;

            Entity thirdPlatform = Entity.CreateEntity("thirdPlatform");
            thirdPlatform.AddComponent<CollisionComponent>();
            thirdPlatform.AddComponent<Sprite>();
            thirdPlatform.GetComponent<Sprite>().spriteId = "third_platform";
            thirdPlatform.width = 3072;
            thirdPlatform.height = 96;
            thirdPlatform.posX = secondPlatform.posX + secondPlatform.width + 144;
            thirdPlatform.posY = firstPlatform.posY;

            Entity fourthPlatform = Entity.CreateEntity("fourthPlatform");
            fourthPlatform.AddComponent<CollisionComponent>();
            fourthPlatform.AddComponent<Sprite>();
            fourthPlatform.GetComponent<Sprite>().spriteId = "fourth_platform";
            fourthPlatform.width = 3216;
            fourthPlatform.height = 96;
            fourthPlatform.posX = thirdPlatform.posX + thirdPlatform.width + 96;
            fourthPlatform.posY = firstPlatform.posY;
        }

        private static void playerCharacter(SDLApp app)
        {
            Entity mario = Entity.CreateEntity("mario");
            mario.AddComponent<SkeletonComponent>();
            mario.AddComponent<CharacterMovementComponent>();
            mario.GetComponent<CharacterMovementComponent>().isControlledByPlayer = true;
            mario.AddComponent<CollisionComponent>();
            mario.AddComponent<Sprite>();
            mario.GetComponent<Sprite>().spriteId = "mario_small";
            mario.GetComponent<CollisionComponent>().isStatic = false;
            mario.width = 48;
            mario.height = 48;
            mario.posX = 144;
            mario.posY = app.GetAppHeight() - 144;

            AnimationData idle = new AnimationData();
            idle.endFrame = 0;
            idle.startFrame = 0;
            idle.frameRatePerSecond = 16;
            idle.width = 48;
            idle.height = 48;

            AnimationData walkData = new AnimationData();
            walkData.endFrame = 3;
            walkData.startFrame = 0;
            walkData.frameRatePerSecond = 16;
            walkData.width = 48;
            walkData.height = 48;

            AnimationData jumpData = new AnimationData();
            jumpData.endFrame = 3;
            jumpData.startFrame = 0;
            jumpData.frameRatePerSecond = 16;
            jumpData.width = 48;
            jumpData.height = 48;

            AnimationData slowdownData = new AnimationData();
            slowdownData.endFrame = 5;
            slowdownData.startFrame = 5;
            slowdownData.frameRatePerSecond = 16;
            slowdownData.width = 48;
            slowdownData.height = 48;

            mario.GetComponent<Sprite>().shouldUseSharedAnimationManager = false;
            mario.GetComponent<Sprite>().shouldTick = true;
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
            questionBlock.width = 48;
            questionBlock.height = 48;
            questionBlock.posX = 768;
            questionBlock.posY = app.GetAppHeight() - 288;

            Entity questionBlock2 = Entity.CreateEntity("questionBlock2");
            questionBlock2.AddComponent<CollisionComponent>();
            questionBlock2.AddComponent<Sprite>();
            questionBlock2.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock2.width = 48;
            questionBlock2.height = 48;
            questionBlock2.posX = questionBlock.posX + 240;
            questionBlock2.posY = questionBlock.posY;

            Entity questionBlock3 = Entity.CreateEntity("questionBlock3");
            questionBlock3.AddComponent<CollisionComponent>();
            questionBlock3.AddComponent<Sprite>();
            questionBlock3.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock3.width = 48;
            questionBlock3.height = 48;
            questionBlock3.posX = questionBlock2.posX + 96;
            questionBlock3.posY = questionBlock2.posY;

            Entity questionBlock4 = Entity.CreateEntity("questionBlock4");
            questionBlock4.AddComponent<CollisionComponent>();
            questionBlock4.AddComponent<Sprite>();
            questionBlock4.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock4.width = 48;
            questionBlock4.height = 48;
            questionBlock4.posX = questionBlock2.posX + 48;
            questionBlock4.posY = questionBlock2.posY - 192;

            Entity questionBlock5 = Entity.CreateEntity("questionBlock5");
            questionBlock5.AddComponent<CollisionComponent>();
            questionBlock5.AddComponent<Sprite>();
            questionBlock5.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock5.width = 48;
            questionBlock5.height = 48;
            questionBlock5.posX = questionBlock3.posX + 2640;
            questionBlock5.posY = questionBlock3.posY;

            Entity questionBlock6 = Entity.CreateEntity("questionBlock6");
            questionBlock6.AddComponent<CollisionComponent>();
            questionBlock6.AddComponent<Sprite>();
            questionBlock6.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock6.width = 48;
            questionBlock6.height = 48;
            questionBlock6.posX = questionBlock5.posX + 768;
            questionBlock6.posY = questionBlock5.posY - 192;

            Entity questionBlock7 = Entity.CreateEntity("questionBlock7");
            questionBlock7.AddComponent<CollisionComponent>();
            questionBlock7.AddComponent<Sprite>();
            questionBlock7.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock7.width = 48;
            questionBlock7.height = 48;
            questionBlock7.posX = questionBlock5.posX + 1344;
            questionBlock7.posY = questionBlock5.posY;

            Entity questionBlock8 = Entity.CreateEntity("questionBlock8");
            questionBlock8.AddComponent<CollisionComponent>();
            questionBlock8.AddComponent<Sprite>();
            questionBlock8.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock8.width = 48;
            questionBlock8.height = 48;
            questionBlock8.posX = questionBlock7.posX + 144;
            questionBlock8.posY = questionBlock5.posY;

            Entity questionBlock9 = Entity.CreateEntity("questionBlock9");
            questionBlock9.AddComponent<CollisionComponent>();
            questionBlock9.AddComponent<Sprite>();
            questionBlock9.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock9.width = 48;
            questionBlock9.height = 48;
            questionBlock9.posX = questionBlock8.posX;
            questionBlock9.posY = questionBlock5.posY - 192;

            Entity questionBlock10 = Entity.CreateEntity("questionBlock10");
            questionBlock10.AddComponent<CollisionComponent>();
            questionBlock10.AddComponent<Sprite>();
            questionBlock10.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock10.width = 48;
            questionBlock10.height = 48;
            questionBlock10.posX = questionBlock8.posX + 144;
            questionBlock10.posY = questionBlock5.posY;

            Entity questionBlock11 = Entity.CreateEntity("questionBlock11");
            questionBlock11.AddComponent<CollisionComponent>();
            questionBlock11.AddComponent<Sprite>();
            questionBlock11.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock11.width = 48;
            questionBlock11.height = 48;
            questionBlock11.posX = questionBlock9.posX + 960;
            questionBlock11.posY = questionBlock9.posY;

            Entity questionBlock12 = Entity.CreateEntity("questionBlock12");
            questionBlock12.AddComponent<CollisionComponent>();
            questionBlock12.AddComponent<Sprite>();
            questionBlock12.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock12.width = 48;
            questionBlock12.height = 48;
            questionBlock12.posX = questionBlock11.posX + 48;
            questionBlock12.posY = questionBlock9.posY;

            Entity questionBlock13 = Entity.CreateEntity("questionBlock13");
            questionBlock13.AddComponent<CollisionComponent>();
            questionBlock13.AddComponent<Sprite>();
            questionBlock13.GetComponent<Sprite>().spriteId = "question_block";
            questionBlock13.width = 48;
            questionBlock13.height = 48;
            questionBlock13.posX = questionBlock12.posX + 1920;
            questionBlock13.posY = questionBlock12.posY + 192;

            AnimationData idleQuestion = new AnimationData();
            idleQuestion.endFrame = 2;
            idleQuestion.startFrame = 0;
            idleQuestion.frameRatePerSecond = 4;
            idleQuestion.width = 48;
            idleQuestion.height = 48;

            SharedAnimationManager manager = Entity.rootEntity.GetComponent<SharedAnimationManager>();

            manager.AddAnimation("question_block", AnimationType.Idle, idleQuestion);
            manager.PlayAnim("question_block", AnimationType.Idle);

            questionBlock.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock2.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock3.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock4.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock5.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock6.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock7.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock8.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock9.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock10.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock11.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock12.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock13.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock2.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock3.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock4.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock5.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock6.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock7.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock8.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock9.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock10.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock11.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock12.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            questionBlock13.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
        }
        private static void brickBlocks(SDLApp app)
        {
            Entity brick = Entity.CreateEntity("Brick");
            brick.AddComponent<CollisionComponent>();
            brick.AddComponent<Sprite>();
            brick.GetComponent<Sprite>().spriteId = "brick";
            brick.width = 48;
            brick.height = 48;
            brick.posX = 1056;
            brick.posY = app.GetAppHeight() - 288;

            Entity brick2 = Entity.CreateEntity("Brick2");
            brick2.AddComponent<CollisionComponent>();
            brick2.AddComponent<Sprite>();
            brick2.GetComponent<Sprite>().spriteId = "brick";
            brick2.width = 48;
            brick2.height = 48;
            brick2.posX = brick.posX - 96;
            brick2.posY = brick.posY;

            Entity brick3 = Entity.CreateEntity("Brick3");
            brick3.AddComponent<CollisionComponent>();
            brick3.AddComponent<Sprite>();
            brick3.GetComponent<Sprite>().spriteId = "brick";
            brick3.width = 48;
            brick3.height = 48;
            brick3.posX = brick.posX + 96;
            brick3.posY = brick.posY;

            Entity brick4 = Entity.CreateEntity("Brick4");
            brick4.AddComponent<CollisionComponent>();
            brick4.AddComponent<Sprite>();
            brick4.GetComponent<Sprite>().spriteId = "brick";
            brick4.width = 48;
            brick4.height = 48;
            brick4.posX = brick3.posX + 2544;
            brick4.posY = brick.posY;

            Entity brick5 = Entity.CreateEntity("Brick5");
            brick5.AddComponent<CollisionComponent>();
            brick5.AddComponent<Sprite>();
            brick5.GetComponent<Sprite>().spriteId = "brick";
            brick5.width = 48;
            brick5.height = 48;
            brick5.posX = brick4.posX + 96;
            brick5.posY = brick.posY;

            Entity brick6 = Entity.CreateEntity("Brick6");
            brick6.AddComponent<CollisionComponent>();
            brick6.AddComponent<Sprite>();
            brick6.GetComponent<Sprite>().spriteId = "brick";
            brick6.width = 48;
            brick6.height = 48;
            brick6.posX = brick5.posX + 48;
            brick6.posY = brick5.posY - 192;

            Entity brick7 = Entity.CreateEntity("Brick7");
            brick7.AddComponent<CollisionComponent>();
            brick7.AddComponent<Sprite>();
            brick7.GetComponent<Sprite>().spriteId = "brick";
            brick7.width = 48;
            brick7.height = 48;
            brick7.posX = brick6.posX + 48;
            brick7.posY = brick6.posY;

            Entity brick8 = Entity.CreateEntity("Brick8");
            brick8.AddComponent<CollisionComponent>();
            brick8.AddComponent<Sprite>();
            brick8.GetComponent<Sprite>().spriteId = "brick";
            brick8.width = 48;
            brick8.height = 48;
            brick8.posX = brick7.posX + 48;
            brick8.posY = brick6.posY;

            Entity brick9 = Entity.CreateEntity("Brick9");
            brick9.AddComponent<CollisionComponent>();
            brick9.AddComponent<Sprite>();
            brick9.GetComponent<Sprite>().spriteId = "brick";
            brick9.width = 48;
            brick9.height = 48;
            brick9.posX = brick8.posX + 48;
            brick9.posY = brick6.posY;

            Entity brick10 = Entity.CreateEntity("Brick10");
            brick10.AddComponent<CollisionComponent>();
            brick10.AddComponent<Sprite>();
            brick10.GetComponent<Sprite>().spriteId = "brick";
            brick10.width = 48;
            brick10.height = 48;
            brick10.posX = brick9.posX + 48;
            brick10.posY = brick6.posY;

            Entity brick11 = Entity.CreateEntity("Brick11");
            brick11.AddComponent<CollisionComponent>();
            brick11.AddComponent<Sprite>();
            brick11.GetComponent<Sprite>().spriteId = "brick";
            brick11.width = 48;
            brick11.height = 48;
            brick11.posX = brick10.posX + 48;
            brick11.posY = brick6.posY;

            Entity brick12 = Entity.CreateEntity("Brick12");
            brick12.AddComponent<CollisionComponent>();
            brick12.AddComponent<Sprite>();
            brick12.GetComponent<Sprite>().spriteId = "brick";
            brick12.width = 48;
            brick12.height = 48;
            brick12.posX = brick11.posX + 48;
            brick12.posY = brick6.posY;

            Entity brick13 = Entity.CreateEntity("Brick13");
            brick13.AddComponent<CollisionComponent>();
            brick13.AddComponent<Sprite>();
            brick13.GetComponent<Sprite>().spriteId = "brick";
            brick13.width = 48;
            brick13.height = 48;
            brick13.posX = brick12.posX + 48;
            brick13.posY = brick6.posY;

            Entity brick14 = Entity.CreateEntity("Brick14");
            brick14.AddComponent<CollisionComponent>();
            brick14.AddComponent<Sprite>();
            brick14.GetComponent<Sprite>().spriteId = "brick";
            brick14.width = 48;
            brick14.height = 48;
            brick14.posX = brick13.posX + 192;
            brick14.posY = brick6.posY;

            Entity brick15 = Entity.CreateEntity("Brick15");
            brick15.AddComponent<CollisionComponent>();
            brick15.AddComponent<Sprite>();
            brick15.GetComponent<Sprite>().spriteId = "brick";
            brick15.width = 48;
            brick15.height = 48;
            brick15.posX = brick14.posX + 48;
            brick15.posY = brick6.posY;

            Entity brick16 = Entity.CreateEntity("Brick16");
            brick16.AddComponent<CollisionComponent>();
            brick16.AddComponent<Sprite>();
            brick16.GetComponent<Sprite>().spriteId = "brick";
            brick16.width = 48;
            brick16.height = 48;
            brick16.posX = brick15.posX + 48;
            brick16.posY = brick6.posY;

            Entity brick17 = Entity.CreateEntity("Brick17");
            brick17.AddComponent<CollisionComponent>();
            brick17.AddComponent<Sprite>();
            brick17.GetComponent<Sprite>().spriteId = "brick";
            brick17.width = 48;
            brick17.height = 48;
            brick17.posX = brick16.posX + 48;
            brick17.posY = brick6.posY + 192;

            Entity brick18 = Entity.CreateEntity("Brick18");
            brick18.AddComponent<CollisionComponent>();
            brick18.AddComponent<Sprite>();
            brick18.GetComponent<Sprite>().spriteId = "brick";
            brick18.width = 48;
            brick18.height = 48;
            brick18.posX = brick17.posX + 288;
            brick18.posY = brick17.posY;

            Entity brick19 = Entity.CreateEntity("Brick19");
            brick19.AddComponent<CollisionComponent>();
            brick19.AddComponent<Sprite>();
            brick19.GetComponent<Sprite>().spriteId = "brick";
            brick19.width = 48;
            brick19.height = 48;
            brick19.posX = brick18.posX + 48;
            brick19.posY = brick17.posY;

            Entity brick20 = Entity.CreateEntity("Brick20");
            brick20.AddComponent<CollisionComponent>();
            brick20.AddComponent<Sprite>();
            brick20.GetComponent<Sprite>().spriteId = "brick";
            brick20.width = 48;
            brick20.height = 48;
            brick20.posX = brick19.posX + 816;
            brick20.posY = brick17.posY;

            Entity brick21 = Entity.CreateEntity("Brick21");
            brick21.AddComponent<CollisionComponent>();
            brick21.AddComponent<Sprite>();
            brick21.GetComponent<Sprite>().spriteId = "brick";
            brick21.width = 48;
            brick21.height = 48;
            brick21.posX = brick20.posX + 144;
            brick21.posY = brick20.posY - 192;

            Entity brick22 = Entity.CreateEntity("Brick22");
            brick22.AddComponent<CollisionComponent>();
            brick22.AddComponent<Sprite>();
            brick22.GetComponent<Sprite>().spriteId = "brick";
            brick22.width = 48;
            brick22.height = 48;
            brick22.posX = brick21.posX + 48;
            brick22.posY = brick21.posY;

            Entity brick23 = Entity.CreateEntity("Brick23");
            brick23.AddComponent<CollisionComponent>();
            brick23.AddComponent<Sprite>();
            brick23.GetComponent<Sprite>().spriteId = "brick";
            brick23.width = 48;
            brick23.height = 48;
            brick23.posX = brick22.posX + 48;
            brick23.posY = brick21.posY;

            Entity brick24 = Entity.CreateEntity("Brick24");
            brick24.AddComponent<CollisionComponent>();
            brick24.AddComponent<Sprite>();
            brick24.GetComponent<Sprite>().spriteId = "brick";
            brick24.width = 48;
            brick24.height = 48;
            brick24.posX = brick23.posX + 240;
            brick24.posY = brick21.posY;

            Entity brick25 = Entity.CreateEntity("Brick25");
            brick25.AddComponent<CollisionComponent>();
            brick25.AddComponent<Sprite>();
            brick25.GetComponent<Sprite>().spriteId = "brick";
            brick25.width = 48;
            brick25.height = 48;
            brick25.posX = brick24.posX + 144;
            brick25.posY = brick21.posY;

            Entity brick26 = Entity.CreateEntity("Brick26");
            brick26.AddComponent<CollisionComponent>();
            brick26.AddComponent<Sprite>();
            brick26.GetComponent<Sprite>().spriteId = "brick";
            brick26.width = 48;
            brick26.height = 48;
            brick26.posX = brick24.posX + 48;
            brick26.posY = brick25.posY + 192;

            Entity brick27 = Entity.CreateEntity("Brick27");
            brick27.AddComponent<CollisionComponent>();
            brick27.AddComponent<Sprite>();
            brick27.GetComponent<Sprite>().spriteId = "brick";
            brick27.width = 48;
            brick27.height = 48;
            brick27.posX = brick26.posX + 48;
            brick27.posY = brick26.posY;

            Entity brick28 = Entity.CreateEntity("Brick28");
            brick28.AddComponent<CollisionComponent>();
            brick28.AddComponent<Sprite>();
            brick28.GetComponent<Sprite>().spriteId = "brick";
            brick28.width = 48;
            brick28.height = 48;
            brick28.posX = brick27.posX + 1824;
            brick28.posY = brick27.posY;

            Entity brick29 = Entity.CreateEntity("Brick29");
            brick29.AddComponent<CollisionComponent>();
            brick29.AddComponent<Sprite>();
            brick29.GetComponent<Sprite>().spriteId = "brick";
            brick29.width = 48;
            brick29.height = 48;
            brick29.posX = brick28.posX + 48;
            brick29.posY = brick27.posY;

            Entity brick30 = Entity.CreateEntity("Brick30");
            brick30.AddComponent<CollisionComponent>();
            brick30.AddComponent<Sprite>();
            brick30.GetComponent<Sprite>().spriteId = "brick";
            brick30.width = 48;
            brick30.height = 48;
            brick30.posX = brick29.posX + 96;
            brick30.posY = brick27.posY;
        }

        private static void renderPipes(SDLApp app)
        {
            Entity pipe = Entity.CreateEntity("Pipe");
            pipe.AddComponent<CollisionComponent>();
            pipe.AddComponent<Sprite>();
            pipe.GetComponent<Sprite>().spriteId = "pipe_small";
            pipe.width = 96;
            pipe.height = 96;
            pipe.posX = 1344;
            pipe.posY = app.GetAppHeight() - 192;

            Entity pipe2 = Entity.CreateEntity("Pipe2");
            pipe2.AddComponent<CollisionComponent>();
            pipe2.AddComponent<Sprite>();
            pipe2.GetComponent<Sprite>().spriteId = "pipe_medium";
            pipe2.width = 96;
            pipe2.height = 144;
            pipe2.posX = pipe.posX + 480;
            pipe2.posY = app.GetAppHeight() - 240;

            Entity pipe3 = Entity.CreateEntity("Pipe3");
            pipe3.AddComponent<CollisionComponent>();
            pipe3.AddComponent<Sprite>();
            pipe3.GetComponent<Sprite>().spriteId = "pipe_large";
            pipe3.width = 96;
            pipe3.height = 192;
            pipe3.posX = pipe2.posX + 384;
            pipe3.posY = app.GetAppHeight() - 288;

            Entity pipe4 = Entity.CreateEntity("Pipe4");
            pipe4.AddComponent<CollisionComponent>();
            pipe4.AddComponent<Sprite>();
            pipe4.GetComponent<Sprite>().spriteId = "pipe_large";
            pipe4.width = 96;
            pipe4.height = 192;
            pipe4.posX = pipe3.posX + 528;
            pipe4.posY = app.GetAppHeight() - 288;

            Entity pipe5 = Entity.CreateEntity("Pipe5");
            pipe5.AddComponent<CollisionComponent>();
            pipe5.AddComponent<Sprite>();
            pipe5.GetComponent<Sprite>().spriteId = "pipe_small";
            pipe5.width = 96;
            pipe5.height = 96;
            pipe5.posX = pipe4.posX + 5090;
            pipe5.posY = app.GetAppHeight() - 192;

            Entity pipe6 = Entity.CreateEntity("Pipe6");
            pipe6.AddComponent<CollisionComponent>();
            pipe6.AddComponent<Sprite>();
            pipe6.GetComponent<Sprite>().spriteId = "pipe_small";
            pipe6.width = 96;
            pipe6.height = 96;
            pipe6.posX = pipe5.posX + 768;
            pipe6.posY = app.GetAppHeight() - 192;

        }
        private static void renderStairs(SDLApp app)
        {
            Entity block = Entity.CreateEntity("Block");
            block.AddComponent<CollisionComponent>();
            block.AddComponent<Sprite>();
            block.GetComponent<Sprite>().spriteId = "block";
            block.width = 48;
            block.height = 48;
            block.posX = 6432;
            block.posY = app.GetAppHeight() - 144;

            Entity block2 = Entity.CreateEntity("Block2");
            block2.AddComponent<CollisionComponent>();
            block2.AddComponent<Sprite>();
            block2.GetComponent<Sprite>().spriteId = "block";
            block2.width = 48;
            block2.height = 48;
            block2.posX = block.posX + 48;
            block2.posY = block.posY;

            Entity block3 = Entity.CreateEntity("Block3");
            block3.AddComponent<CollisionComponent>();
            block3.AddComponent<Sprite>();
            block3.GetComponent<Sprite>().spriteId = "block";
            block3.width = 48;
            block3.height = 48;
            block3.posX = block2.posX;
            block3.posY = block.posY - 48;

            Entity block4 = Entity.CreateEntity("Block4");
            block4.AddComponent<CollisionComponent>();
            block4.AddComponent<Sprite>();
            block4.GetComponent<Sprite>().spriteId = "block";
            block4.width = 48;
            block4.height = 48;
            block4.posX = block3.posX + 48;
            block4.posY = block2.posY;

            Entity block5 = Entity.CreateEntity("Block5");
            block5.AddComponent<CollisionComponent>();
            block5.AddComponent<Sprite>();
            block5.GetComponent<Sprite>().spriteId = "block";
            block5.width = 48;
            block5.height = 48;
            block5.posX = block4.posX;
            block5.posY = block4.posY - 48;

            Entity block6 = Entity.CreateEntity("Block6");
            block6.AddComponent<CollisionComponent>();
            block6.AddComponent<Sprite>();
            block6.GetComponent<Sprite>().spriteId = "block";
            block6.width = 48;
            block6.height = 48;
            block6.posX = block4.posX;
            block6.posY = block4.posY - 96;

            Entity block7 = Entity.CreateEntity("Block7");
            block7.AddComponent<CollisionComponent>();
            block7.AddComponent<Sprite>();
            block7.GetComponent<Sprite>().spriteId = "block";
            block7.width = 48;
            block7.height = 48;
            block7.posX = block6.posX + 48;
            block7.posY = block4.posY;

            Entity block8 = Entity.CreateEntity("Block8");
            block8.AddComponent<CollisionComponent>();
            block8.AddComponent<Sprite>();
            block8.GetComponent<Sprite>().spriteId = "block";
            block8.width = 48;
            block8.height = 48;
            block8.posX = block7.posX;
            block8.posY = block7.posY - 48;

            Entity block9 = Entity.CreateEntity("Block9");
            block9.AddComponent<CollisionComponent>();
            block9.AddComponent<Sprite>();
            block9.GetComponent<Sprite>().spriteId = "block";
            block9.width = 48;
            block9.height = 48;
            block9.posX = block7.posX;
            block9.posY = block7.posY - 96;

            Entity block10 = Entity.CreateEntity("Block10");
            block10.AddComponent<CollisionComponent>();
            block10.AddComponent<Sprite>();
            block10.GetComponent<Sprite>().spriteId = "block";
            block10.width = 48;
            block10.height = 48;
            block10.posX = block7.posX;
            block10.posY = block7.posY - 144;

            Entity block11 = Entity.CreateEntity("Block11");
            block11.AddComponent<CollisionComponent>();
            block11.AddComponent<Sprite>();
            block11.GetComponent<Sprite>().spriteId = "block";
            block11.width = 48;
            block11.height = 48;
            block11.posX = block10.posX + 144;
            block11.posY = block10.posY;

            Entity block12 = Entity.CreateEntity("Block12");
            block12.AddComponent<CollisionComponent>();
            block12.AddComponent<Sprite>();
            block12.GetComponent<Sprite>().spriteId = "block";
            block12.width = 48;
            block12.height = 48;
            block12.posX = block11.posX;
            block12.posY = block11.posY + 48;

            Entity block13 = Entity.CreateEntity("Block13");
            block13.AddComponent<CollisionComponent>();
            block13.AddComponent<Sprite>();
            block13.GetComponent<Sprite>().spriteId = "block";
            block13.width = 48;
            block13.height = 48;
            block13.posX = block11.posX;
            block13.posY = block11.posY + 96;

            Entity block14 = Entity.CreateEntity("Block14");
            block14.AddComponent<CollisionComponent>();
            block14.AddComponent<Sprite>();
            block14.GetComponent<Sprite>().spriteId = "block";
            block14.width = 48;
            block14.height = 48;
            block14.posX = block11.posX;
            block14.posY = block11.posY + 144;

            Entity block15 = Entity.CreateEntity("Block15");
            block15.AddComponent<CollisionComponent>();
            block15.AddComponent<Sprite>();
            block15.GetComponent<Sprite>().spriteId = "block";
            block15.width = 48;
            block15.height = 48;
            block15.posX = block11.posX + 48;
            block15.posY = block12.posY;

            Entity block16 = Entity.CreateEntity("Block16");
            block16.AddComponent<CollisionComponent>();
            block16.AddComponent<Sprite>();
            block16.GetComponent<Sprite>().spriteId = "block";
            block16.width = 48;
            block16.height = 48;
            block16.posX = block15.posX;
            block16.posY = block15.posY + 48;

            Entity block17 = Entity.CreateEntity("Block17");
            block17.AddComponent<CollisionComponent>();
            block17.AddComponent<Sprite>();
            block17.GetComponent<Sprite>().spriteId = "block";
            block17.width = 48;
            block17.height = 48;
            block17.posX = block15.posX;
            block17.posY = block15.posY + 96;

            Entity block18 = Entity.CreateEntity("Block18");
            block18.AddComponent<CollisionComponent>();
            block18.AddComponent<Sprite>();
            block18.GetComponent<Sprite>().spriteId = "block";
            block18.width = 48;
            block18.height = 48;
            block18.posX = block16.posX + 48;
            block18.posY = block16.posY;

            Entity block19 = Entity.CreateEntity("Block19");
            block19.AddComponent<CollisionComponent>();
            block19.AddComponent<Sprite>();
            block19.GetComponent<Sprite>().spriteId = "block";
            block19.width = 48;
            block19.height = 48;
            block19.posX = block18.posX;
            block19.posY = block18.posY + 48;

            Entity block20 = Entity.CreateEntity("Block20");
            block20.AddComponent<CollisionComponent>();
            block20.AddComponent<Sprite>();
            block20.GetComponent<Sprite>().spriteId = "block";
            block20.width = 48;
            block20.height = 48;
            block20.posX = block19.posX + 48;
            block20.posY = block19.posY;

            Entity block21 = Entity.CreateEntity("Block21");
            block21.AddComponent<CollisionComponent>();
            block21.AddComponent<Sprite>();
            block21.GetComponent<Sprite>().spriteId = "block";
            block21.width = 48;
            block21.height = 48;
            block21.posX = block20.posX + 240;
            block21.posY = block20.posY;

            Entity block22 = Entity.CreateEntity("Block22");
            block22.AddComponent<CollisionComponent>();
            block22.AddComponent<Sprite>();
            block22.GetComponent<Sprite>().spriteId = "block";
            block22.width = 48;
            block22.height = 48;
            block22.posX = block21.posX + 48;
            block22.posY = block21.posY;


            Entity block23 = Entity.CreateEntity("Block23");
            block23.AddComponent<CollisionComponent>();
            block23.AddComponent<Sprite>();
            block23.GetComponent<Sprite>().spriteId = "block";
            block23.width = 48;
            block23.height = 48;
            block23.posX = block22.posX;
            block23.posY = block22.posY - 48;


            Entity block24 = Entity.CreateEntity("Block24");
            block24.AddComponent<CollisionComponent>();
            block24.AddComponent<Sprite>();
            block24.GetComponent<Sprite>().spriteId = "block";
            block24.width = 48;
            block24.height = 48;
            block24.posX = block22.posX + 48;
            block24.posY = block22.posY;

            Entity block25 = Entity.CreateEntity("Block25");
            block25.AddComponent<CollisionComponent>();
            block25.AddComponent<Sprite>();
            block25.GetComponent<Sprite>().spriteId = "block";
            block25.width = 48;
            block25.height = 48;
            block25.posX = block24.posX;
            block25.posY = block24.posY - 48;

            Entity block26 = Entity.CreateEntity("Block26");
            block26.AddComponent<CollisionComponent>();
            block26.AddComponent<Sprite>();
            block26.GetComponent<Sprite>().spriteId = "block";
            block26.width = 48;
            block26.height = 48;
            block26.posX = block24.posX;
            block26.posY = block24.posY - 96;

            Entity block27 = Entity.CreateEntity("Block27");
            block27.AddComponent<CollisionComponent>();
            block27.AddComponent<Sprite>();
            block27.GetComponent<Sprite>().spriteId = "block";
            block27.width = 48;
            block27.height = 48;
            block27.posX = block24.posX + 48;
            block27.posY = block24.posY;

            Entity block28 = Entity.CreateEntity("Block28");
            block28.AddComponent<CollisionComponent>();
            block28.AddComponent<Sprite>();
            block28.GetComponent<Sprite>().spriteId = "block";
            block28.width = 48;
            block28.height = 48;
            block28.posX = block27.posX;
            block28.posY = block27.posY - 48;

            Entity block29 = Entity.CreateEntity("Block29");
            block29.AddComponent<CollisionComponent>();
            block29.AddComponent<Sprite>();
            block29.GetComponent<Sprite>().spriteId = "block";
            block29.width = 48;
            block29.height = 48;
            block29.posX = block27.posX;
            block29.posY = block27.posY - 96;

            Entity block30 = Entity.CreateEntity("Block30");
            block30.AddComponent<CollisionComponent>();
            block30.AddComponent<Sprite>();
            block30.GetComponent<Sprite>().spriteId = "block";
            block30.width = 48;
            block30.height = 48;
            block30.posX = block27.posX;
            block30.posY = block27.posY - 144;

            Entity block31 = Entity.CreateEntity("Block31");
            block31.AddComponent<CollisionComponent>();
            block31.AddComponent<Sprite>();
            block31.GetComponent<Sprite>().spriteId = "block";
            block31.width = 48;
            block31.height = 48;
            block31.posX = block27.posX + 48;
            block31.posY = block27.posY;

            Entity block32 = Entity.CreateEntity("Block32");
            block32.AddComponent<CollisionComponent>();
            block32.AddComponent<Sprite>();
            block32.GetComponent<Sprite>().spriteId = "block";
            block32.width = 48;
            block32.height = 48;
            block32.posX = block31.posX;
            block32.posY = block31.posY - 48;

            Entity block33 = Entity.CreateEntity("Block33");
            block33.AddComponent<CollisionComponent>();
            block33.AddComponent<Sprite>();
            block33.GetComponent<Sprite>().spriteId = "block";
            block33.width = 48;
            block33.height = 48;
            block33.posX = block31.posX;
            block33.posY = block31.posY - 96;

            Entity block34 = Entity.CreateEntity("Block34");
            block34.AddComponent<CollisionComponent>();
            block34.AddComponent<Sprite>();
            block34.GetComponent<Sprite>().spriteId = "block";
            block34.width = 48;
            block34.height = 48;
            block34.posX = block31.posX;
            block34.posY = block31.posY - 144;

            Entity block35 = Entity.CreateEntity("Block35");
            block35.AddComponent<CollisionComponent>();
            block35.AddComponent<Sprite>();
            block35.GetComponent<Sprite>().spriteId = "block";
            block35.width = 48;
            block35.height = 48;
            block35.posX = block31.posX + 144;
            block35.posY = block31.posY - 144;

            Entity block36 = Entity.CreateEntity("Block36");
            block36.AddComponent<CollisionComponent>();
            block36.AddComponent<Sprite>();
            block36.GetComponent<Sprite>().spriteId = "block";
            block36.width = 48;
            block36.height = 48;
            block36.posX = block35.posX;
            block36.posY = block35.posY + 48;

            Entity block37 = Entity.CreateEntity("Block37");
            block37.AddComponent<CollisionComponent>();
            block37.AddComponent<Sprite>();
            block37.GetComponent<Sprite>().spriteId = "block";
            block37.width = 48;
            block37.height = 48;
            block37.posX = block35.posX;
            block37.posY = block35.posY + 96;

            Entity block38 = Entity.CreateEntity("Block38");
            block38.AddComponent<CollisionComponent>();
            block38.AddComponent<Sprite>();
            block38.GetComponent<Sprite>().spriteId = "block";
            block38.width = 48;
            block38.height = 48;
            block38.posX = block35.posX;
            block38.posY = block35.posY + 144;

            Entity block39 = Entity.CreateEntity("Block39");
            block39.AddComponent<CollisionComponent>();
            block39.AddComponent<Sprite>();
            block39.GetComponent<Sprite>().spriteId = "block";
            block39.width = 48;
            block39.height = 48;
            block39.posX = block38.posX + 48;
            block39.posY = block38.posY;

            Entity block40 = Entity.CreateEntity("Block40");
            block40.AddComponent<CollisionComponent>();
            block40.AddComponent<Sprite>();
            block40.GetComponent<Sprite>().spriteId = "block";
            block40.width = 48;
            block40.height = 48;
            block40.posX = block39.posX;
            block40.posY = block39.posY - 48;

            Entity block41 = Entity.CreateEntity("Block41");
            block41.AddComponent<CollisionComponent>();
            block41.AddComponent<Sprite>();
            block41.GetComponent<Sprite>().spriteId = "block";
            block41.width = 48;
            block41.height = 48;
            block41.posX = block39.posX;
            block41.posY = block39.posY - 96;

            Entity block42 = Entity.CreateEntity("Block42");
            block42.AddComponent<CollisionComponent>();
            block42.AddComponent<Sprite>();
            block42.GetComponent<Sprite>().spriteId = "block";
            block42.width = 48;
            block42.height = 48;
            block42.posX = block39.posX + 48;
            block42.posY = block39.posY;

            Entity block43 = Entity.CreateEntity("Block43");
            block43.AddComponent<CollisionComponent>();
            block43.AddComponent<Sprite>();
            block43.GetComponent<Sprite>().spriteId = "block";
            block43.width = 48;
            block43.height = 48;
            block43.posX = block42.posX;
            block43.posY = block42.posY - 48;

            Entity block44 = Entity.CreateEntity("Block44");
            block44.AddComponent<CollisionComponent>();
            block44.AddComponent<Sprite>();
            block44.GetComponent<Sprite>().spriteId = "block";
            block44.width = 48;
            block44.height = 48;
            block44.posX = block42.posX + 48;
            block44.posY = block42.posY;

            Entity block45 = Entity.CreateEntity("Block45");
            block45.AddComponent<CollisionComponent>();
            block45.AddComponent<Sprite>();
            block45.GetComponent<Sprite>().spriteId = "block";
            block45.width = 48;
            block45.height = 48;
            block45.posX = block44.posX + 1104;
            block45.posY = block44.posY;

            Entity block46 = Entity.CreateEntity("Block46");
            block46.AddComponent<CollisionComponent>();
            block46.AddComponent<Sprite>();
            block46.GetComponent<Sprite>().spriteId = "block";
            block46.width = 48;
            block46.height = 48;
            block46.posX = block45.posX + 48;
            block46.posY = block45.posY;

            Entity block47 = Entity.CreateEntity("Block47");
            block47.AddComponent<CollisionComponent>();
            block47.AddComponent<Sprite>();
            block47.GetComponent<Sprite>().spriteId = "block";
            block47.width = 48;
            block47.height = 48;
            block47.posX = block46.posX;
            block47.posY = block46.posY - 48;

            Entity block48 = Entity.CreateEntity("Block48");
            block48.AddComponent<CollisionComponent>();
            block48.AddComponent<Sprite>();
            block48.GetComponent<Sprite>().spriteId = "block";
            block48.width = 48;
            block48.height = 48;
            block48.posX = block46.posX + 48;
            block48.posY = block46.posY;

            Entity block49 = Entity.CreateEntity("Block49");
            block49.AddComponent<CollisionComponent>();
            block49.AddComponent<Sprite>();
            block49.GetComponent<Sprite>().spriteId = "block";
            block49.width = 48;
            block49.height = 48;
            block49.posX = block48.posX;
            block49.posY = block48.posY - 48;

            Entity block50 = Entity.CreateEntity("Block50");
            block50.AddComponent<CollisionComponent>();
            block50.AddComponent<Sprite>();
            block50.GetComponent<Sprite>().spriteId = "block";
            block50.width = 48;
            block50.height = 48;
            block50.posX = block48.posX;
            block50.posY = block48.posY - 96;

            Entity block51 = Entity.CreateEntity("Block51");
            block51.AddComponent<CollisionComponent>();
            block51.AddComponent<Sprite>();
            block51.GetComponent<Sprite>().spriteId = "block";
            block51.width = 48;
            block51.height = 48;
            block51.posX = block48.posX + 48;
            block51.posY = block48.posY;

            Entity block52 = Entity.CreateEntity("Block52");
            block52.AddComponent<CollisionComponent>();
            block52.AddComponent<Sprite>();
            block52.GetComponent<Sprite>().spriteId = "block";
            block52.width = 48;
            block52.height = 48;
            block52.posX = block51.posX;
            block52.posY = block51.posY - 48;

            Entity block53 = Entity.CreateEntity("Block53");
            block53.AddComponent<CollisionComponent>();
            block53.AddComponent<Sprite>();
            block53.GetComponent<Sprite>().spriteId = "block";
            block53.width = 48;
            block53.height = 48;
            block53.posX = block51.posX;
            block53.posY = block51.posY - 96;

            Entity block54 = Entity.CreateEntity("Block54");
            block54.AddComponent<CollisionComponent>();
            block54.AddComponent<Sprite>();
            block54.GetComponent<Sprite>().spriteId = "block";
            block54.width = 48;
            block54.height = 48;
            block54.posX = block51.posX;
            block54.posY = block51.posY - 144;

            Entity block55 = Entity.CreateEntity("Block55");
            block55.AddComponent<CollisionComponent>();
            block55.AddComponent<Sprite>();
            block55.GetComponent<Sprite>().spriteId = "block";
            block55.width = 48;
            block55.height = 48;
            block55.posX = block51.posX + 48;
            block55.posY = block51.posY;

            Entity block56 = Entity.CreateEntity("Block56");
            block56.AddComponent<CollisionComponent>();
            block56.AddComponent<Sprite>();
            block56.GetComponent<Sprite>().spriteId = "block";
            block56.width = 48;
            block56.height = 48;
            block56.posX = block55.posX;
            block56.posY = block55.posY - 48;

            Entity block57 = Entity.CreateEntity("Block57");
            block57.AddComponent<CollisionComponent>();
            block57.AddComponent<Sprite>();
            block57.GetComponent<Sprite>().spriteId = "block";
            block57.width = 48;
            block57.height = 48;
            block57.posX = block55.posX;
            block57.posY = block55.posY - 96;

            Entity block58 = Entity.CreateEntity("Block58");
            block58.AddComponent<CollisionComponent>();
            block58.AddComponent<Sprite>();
            block58.GetComponent<Sprite>().spriteId = "block";
            block58.width = 48;
            block58.height = 48;
            block58.posX = block55.posX;
            block58.posY = block55.posY - 144;

            Entity block59 = Entity.CreateEntity("Block59");
            block59.AddComponent<CollisionComponent>();
            block59.AddComponent<Sprite>();
            block59.GetComponent<Sprite>().spriteId = "block";
            block59.width = 48;
            block59.height = 48;
            block59.posX = block55.posX;
            block59.posY = block55.posY - 192;

            Entity block60 = Entity.CreateEntity("Block60");
            block60.AddComponent<CollisionComponent>();
            block60.AddComponent<Sprite>();
            block60.GetComponent<Sprite>().spriteId = "block";
            block60.width = 48;
            block60.height = 48;
            block60.posX = block55.posX + 48;
            block60.posY = block55.posY;

            Entity block61 = Entity.CreateEntity("Block61");
            block61.AddComponent<CollisionComponent>();
            block61.AddComponent<Sprite>();
            block61.GetComponent<Sprite>().spriteId = "block";
            block61.width = 48;
            block61.height = 48;
            block61.posX = block60.posX;
            block61.posY = block60.posY - 48;

            Entity block62 = Entity.CreateEntity("Block62");
            block62.AddComponent<CollisionComponent>();
            block62.AddComponent<Sprite>();
            block62.GetComponent<Sprite>().spriteId = "block";
            block62.width = 48;
            block62.height = 48;
            block62.posX = block60.posX;
            block62.posY = block60.posY - 96;

            Entity block63 = Entity.CreateEntity("Block63");
            block63.AddComponent<CollisionComponent>();
            block63.AddComponent<Sprite>();
            block63.GetComponent<Sprite>().spriteId = "block";
            block63.width = 48;
            block63.height = 48;
            block63.posX = block60.posX;
            block63.posY = block60.posY - 144;

            Entity block64 = Entity.CreateEntity("Block64");
            block64.AddComponent<CollisionComponent>();
            block64.AddComponent<Sprite>();
            block64.GetComponent<Sprite>().spriteId = "block";
            block64.width = 48;
            block64.height = 48;
            block64.posX = block60.posX;
            block64.posY = block60.posY - 192;

            Entity block65 = Entity.CreateEntity("Block65");
            block65.AddComponent<CollisionComponent>();
            block65.AddComponent<Sprite>();
            block65.GetComponent<Sprite>().spriteId = "block";
            block65.width = 48;
            block65.height = 48;
            block65.posX = block60.posX;
            block65.posY = block60.posY - 240;

            Entity block66 = Entity.CreateEntity("Block66");
            block66.AddComponent<CollisionComponent>();
            block66.AddComponent<Sprite>();
            block66.GetComponent<Sprite>().spriteId = "block";
            block66.width = 48;
            block66.height = 48;
            block66.posX = block60.posX + 48;
            block66.posY = block60.posY;

            Entity block67 = Entity.CreateEntity("Block67");
            block67.AddComponent<CollisionComponent>();
            block67.AddComponent<Sprite>();
            block67.GetComponent<Sprite>().spriteId = "block";
            block67.width = 48;
            block67.height = 48;
            block67.posX = block66.posX;
            block67.posY = block66.posY - 48;

            Entity block68 = Entity.CreateEntity("Block68");
            block68.AddComponent<CollisionComponent>();
            block68.AddComponent<Sprite>();
            block68.GetComponent<Sprite>().spriteId = "block";
            block68.width = 48;
            block68.height = 48;
            block68.posX = block66.posX;
            block68.posY = block66.posY - 96;

            Entity block69 = Entity.CreateEntity("Block69");
            block69.AddComponent<CollisionComponent>();
            block69.AddComponent<Sprite>();
            block69.GetComponent<Sprite>().spriteId = "block";
            block69.width = 48;
            block69.height = 48;
            block69.posX = block66.posX;
            block69.posY = block66.posY - 144;

            Entity block70 = Entity.CreateEntity("Block70");
            block70.AddComponent<CollisionComponent>();
            block70.AddComponent<Sprite>();
            block70.GetComponent<Sprite>().spriteId = "block";
            block70.width = 48;
            block70.height = 48;
            block70.posX = block66.posX;
            block70.posY = block66.posY - 192;

            Entity block71 = Entity.CreateEntity("Block71");
            block71.AddComponent<CollisionComponent>();
            block71.AddComponent<Sprite>();
            block71.GetComponent<Sprite>().spriteId = "block";
            block71.width = 48;
            block71.height = 48;
            block71.posX = block66.posX;
            block71.posY = block66.posY - 240;

            Entity block72 = Entity.CreateEntity("Block72");
            block72.AddComponent<CollisionComponent>();
            block72.AddComponent<Sprite>();
            block72.GetComponent<Sprite>().spriteId = "block";
            block72.width = 48;
            block72.height = 48;
            block72.posX = block66.posX;
            block72.posY = block66.posY - 288;

            Entity block73 = Entity.CreateEntity("Block73");
            block73.AddComponent<CollisionComponent>();
            block73.AddComponent<Sprite>();
            block73.GetComponent<Sprite>().spriteId = "block";
            block73.width = 48;
            block73.height = 48;
            block73.posX = block66.posX + 48;
            block73.posY = block66.posY;

            Entity block74 = Entity.CreateEntity("Block74");
            block74.AddComponent<CollisionComponent>();
            block74.AddComponent<Sprite>();
            block74.GetComponent<Sprite>().spriteId = "block";
            block74.width = 48;
            block74.height = 48;
            block74.posX = block73.posX;
            block74.posY = block73.posY - 48;

            Entity block75 = Entity.CreateEntity("Block75");
            block75.AddComponent<CollisionComponent>();
            block75.AddComponent<Sprite>();
            block75.GetComponent<Sprite>().spriteId = "block";
            block75.width = 48;
            block75.height = 48;
            block75.posX = block73.posX;
            block75.posY = block73.posY - 96;

            Entity block76 = Entity.CreateEntity("Block76");
            block76.AddComponent<CollisionComponent>();
            block76.AddComponent<Sprite>();
            block76.GetComponent<Sprite>().spriteId = "block";
            block76.width = 48;
            block76.height = 48;
            block76.posX = block73.posX;
            block76.posY = block73.posY - 144;

            Entity block77 = Entity.CreateEntity("Block77");
            block77.AddComponent<CollisionComponent>();
            block77.AddComponent<Sprite>();
            block77.GetComponent<Sprite>().spriteId = "block";
            block77.width = 48;
            block77.height = 48;
            block77.posX = block73.posX;
            block77.posY = block73.posY - 192;

            Entity block78 = Entity.CreateEntity("Block78");
            block78.AddComponent<CollisionComponent>();
            block78.AddComponent<Sprite>();
            block78.GetComponent<Sprite>().spriteId = "block";
            block78.width = 48;
            block78.height = 48;
            block78.posX = block73.posX;
            block78.posY = block73.posY - 240;

            Entity block79 = Entity.CreateEntity("Block79");
            block79.AddComponent<CollisionComponent>();
            block79.AddComponent<Sprite>();
            block79.GetComponent<Sprite>().spriteId = "block";
            block79.width = 48;
            block79.height = 48;
            block79.posX = block73.posX;
            block79.posY = block73.posY - 288;

            Entity block80 = Entity.CreateEntity("Block80");
            block80.AddComponent<CollisionComponent>();
            block80.AddComponent<Sprite>();
            block80.GetComponent<Sprite>().spriteId = "block";
            block80.width = 48;
            block80.height = 48;
            block80.posX = block73.posX;
            block80.posY = block73.posY - 336;

            Entity block81 = Entity.CreateEntity("Block81");
            block81.AddComponent<CollisionComponent>();
            block81.AddComponent<Sprite>();
            block81.GetComponent<Sprite>().spriteId = "block";
            block81.width = 48;
            block81.height = 48;
            block81.posX = block73.posX + 48;
            block81.posY = block73.posY;

            Entity block82 = Entity.CreateEntity("Block82");
            block82.AddComponent<CollisionComponent>();
            block82.AddComponent<Sprite>();
            block82.GetComponent<Sprite>().spriteId = "block";
            block82.width = 48;
            block82.height = 48;
            block82.posX = block81.posX;
            block82.posY = block81.posY - 48;

            Entity block83 = Entity.CreateEntity("Block83");
            block83.AddComponent<CollisionComponent>();
            block83.AddComponent<Sprite>();
            block83.GetComponent<Sprite>().spriteId = "block";
            block83.width = 48;
            block83.height = 48;
            block83.posX = block81.posX;
            block83.posY = block81.posY - 96;

            Entity block84 = Entity.CreateEntity("Block84");
            block84.AddComponent<CollisionComponent>();
            block84.AddComponent<Sprite>();
            block84.GetComponent<Sprite>().spriteId = "block";
            block84.width = 48;
            block84.height = 48;
            block84.posX = block81.posX;
            block84.posY = block81.posY - 144;

            Entity block85 = Entity.CreateEntity("Block85");
            block85.AddComponent<CollisionComponent>();
            block85.AddComponent<Sprite>();
            block85.GetComponent<Sprite>().spriteId = "block";
            block85.width = 48;
            block85.height = 48;
            block85.posX = block81.posX;
            block85.posY = block81.posY - 192;

            Entity block86 = Entity.CreateEntity("Block86");
            block86.AddComponent<CollisionComponent>();
            block86.AddComponent<Sprite>();
            block86.GetComponent<Sprite>().spriteId = "block";
            block86.width = 48;
            block86.height = 48;
            block86.posX = block81.posX;
            block86.posY = block81.posY - 240;

            Entity block87 = Entity.CreateEntity("Block87");
            block87.AddComponent<CollisionComponent>();
            block87.AddComponent<Sprite>();
            block87.GetComponent<Sprite>().spriteId = "block";
            block87.width = 48;
            block87.height = 48;
            block87.posX = block81.posX;
            block87.posY = block81.posY - 288;

            Entity block88 = Entity.CreateEntity("Block88");
            block88.AddComponent<CollisionComponent>();
            block88.AddComponent<Sprite>();
            block88.GetComponent<Sprite>().spriteId = "block";
            block88.width = 48;
            block88.height = 48;
            block88.posX = block81.posX;
            block88.posY = block81.posY - 336;

            Entity block89 = Entity.CreateEntity("Block89");
            block89.AddComponent<CollisionComponent>();
            block89.AddComponent<Sprite>();
            block89.GetComponent<Sprite>().spriteId = "block";
            block89.width = 48;
            block89.height = 48;
            block89.posX = block81.posX + 432;
            block89.posY = block81.posY;
        }
        private static void spawnEnemiesAtStartLocation(SDLApp app)
        {
            Entity flag = Entity.CreateEntity("flag");
            flag.AddComponent<CollisionComponent>();
            flag.AddComponent<Sprite>();
            flag.GetComponent<Sprite>().spriteId = "full-flag";
            flag.width = 96;
            flag.height = 480;
            flag.posX = 9456;
            flag.posY = app.GetAppHeight() - 624;
            CollisionComponent flagCollision = flag.GetComponent<CollisionComponent>();
            flagCollision.MarkItAsTrigger();

            flagCollision.onOverlaped += e =>
            {
                if (e.lastContact.name.Equals("mario"))
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
            goomba.GetComponent<CollisionComponent>().isStatic = false;
            goomba.width = 48;
            goomba.height = 48;
            goomba.posX = 1056;
            goomba.posY = app.GetAppHeight() - 144;

            Entity goomba2 = Entity.CreateEntity("goomba2");
            goomba2.AddComponent<CharacterMovementComponent>();
            goomba2.AddComponent<Gumba>();
            goomba2.AddComponent<CollisionComponent>();
            goomba2.AddComponent<Sprite>();
            goomba2.GetComponent<Sprite>().spriteId = "goomba";
            goomba2.GetComponent<CollisionComponent>().isStatic = false;
            goomba2.width = 48;
            goomba2.height = 48;
            goomba2.posX = goomba.posX + 864;
            goomba2.posY = app.GetAppHeight() - 144;

            Entity goomba3 = Entity.CreateEntity("goomba3");
            goomba3.AddComponent<CharacterMovementComponent>();
            goomba3.AddComponent<Gumba>();
            goomba3.AddComponent<CollisionComponent>();
            goomba3.AddComponent<Sprite>();
            goomba3.GetComponent<Sprite>().spriteId = "goomba";
            goomba3.GetComponent<CollisionComponent>().isStatic = false;
            goomba3.width = 48;
            goomba3.height = 48;
            goomba3.posX = goomba2.posX + 528;
            goomba3.posY = app.GetAppHeight() - 144;

            Entity goomba4 = Entity.CreateEntity("goomba4");
            goomba4.AddComponent<CharacterMovementComponent>();
            goomba4.AddComponent<Gumba>();
            goomba4.AddComponent<CollisionComponent>();
            goomba4.AddComponent<Sprite>();
            goomba4.GetComponent<Sprite>().spriteId = "goomba";
            goomba4.GetComponent<CollisionComponent>().isStatic = false;
            goomba4.width = 48;
            goomba4.height = 48;
            goomba4.posX = goomba3.posX + 72;
            goomba4.posY = app.GetAppHeight() - 144;

            Entity goomba5 = Entity.CreateEntity("goomba5");
            goomba5.AddComponent<CharacterMovementComponent>();
            goomba5.AddComponent<Gumba>();
            goomba5.AddComponent<CollisionComponent>();
            goomba5.AddComponent<Sprite>();
            goomba5.GetComponent<Sprite>().spriteId = "goomba";
            goomba5.GetComponent<CollisionComponent>().isStatic = false;
            goomba5.width = 48;
            goomba5.height = 48;
            goomba5.posX = goomba3.posX + 1392;
            goomba5.posY = goomba3.posY - 384;

            Entity goomba6 = Entity.CreateEntity("goomba6");
            goomba6.AddComponent<CharacterMovementComponent>();
            goomba6.AddComponent<Gumba>();
            goomba6.AddComponent<CollisionComponent>();
            goomba6.AddComponent<Sprite>();
            goomba6.GetComponent<Sprite>().spriteId = "goomba";
            goomba6.GetComponent<CollisionComponent>().isStatic = false;
            goomba6.width = 48;
            goomba6.height = 48;
            goomba6.posX = goomba5.posX + 96;
            goomba6.posY = goomba5.posY;

            Entity goomba7 = Entity.CreateEntity("goomba7");
            goomba7.AddComponent<CharacterMovementComponent>();
            goomba7.AddComponent<Gumba>();
            goomba7.AddComponent<CollisionComponent>();
            goomba7.AddComponent<Sprite>();
            goomba7.GetComponent<Sprite>().spriteId = "goomba";
            goomba7.GetComponent<CollisionComponent>().isStatic = false;
            goomba7.width = 48;
            goomba7.height = 48;
            goomba7.posX = goomba3.posX + 2208;
            goomba7.posY = goomba3.posY;

            Entity goomba8 = Entity.CreateEntity("goomba8");
            goomba8.AddComponent<CharacterMovementComponent>();
            goomba8.AddComponent<Gumba>();
            goomba8.AddComponent<CollisionComponent>();
            goomba8.AddComponent<Sprite>();
            goomba8.GetComponent<Sprite>().spriteId = "goomba";
            goomba8.GetComponent<CollisionComponent>().isStatic = false;
            goomba8.width = 48;
            goomba8.height = 48;
            goomba8.posX = goomba7.posX + 72;
            goomba8.posY = goomba7.posY;

            Entity koopa = Entity.CreateEntity("Koopa");
            koopa.AddComponent<CharacterMovementComponent>();
            koopa.AddComponent<CollisionComponent>();
            koopa.AddComponent<Sprite>();
            koopa.GetComponent<Sprite>().spriteId = "koopa";
            //koopa.GetComponent<CollisionComponent>().IsStatic = false;
            koopa.width = 48;
            koopa.height = 36;
            koopa.posX = goomba7.posX + 480;
            koopa.posY = goomba7.posY - 48;

            Entity goomba9 = Entity.CreateEntity("goomba9");
            goomba9.AddComponent<CharacterMovementComponent>();
            goomba9.AddComponent<Gumba>();
            goomba9.AddComponent<CollisionComponent>();
            goomba9.AddComponent<Sprite>();
            goomba9.GetComponent<Sprite>().spriteId = "goomba";
            goomba9.GetComponent<CollisionComponent>().isStatic = false;
            goomba9.width = 48;
            goomba9.height = 48;
            goomba9.posX = goomba7.posX + 816;
            goomba9.posY = goomba7.posY;

            Entity goomba10 = Entity.CreateEntity("goomba10");
            goomba10.AddComponent<CharacterMovementComponent>();
            goomba10.AddComponent<Gumba>();
            goomba10.AddComponent<CollisionComponent>();
            goomba10.AddComponent<Sprite>();
            goomba10.GetComponent<Sprite>().spriteId = "goomba";
            goomba10.GetComponent<CollisionComponent>().isStatic = false;
            goomba10.width = 48;
            goomba10.height = 48;
            goomba10.posX = goomba9.posX + 72;
            goomba10.posY = goomba9.posY;

            Entity goomba11 = Entity.CreateEntity("goomba11");
            goomba11.AddComponent<CharacterMovementComponent>();
            goomba11.AddComponent<Gumba>();
            goomba11.AddComponent<CollisionComponent>();
            goomba11.AddComponent<Sprite>();
            goomba11.GetComponent<Sprite>().spriteId = "goomba";
            goomba11.GetComponent<CollisionComponent>().isStatic = false;
            goomba11.width = 48;
            goomba11.height = 48;
            goomba11.posX = goomba9.posX + 480;
            goomba11.posY = goomba9.posY;

            Entity goomba12 = Entity.CreateEntity("goomba12");
            goomba12.AddComponent<CharacterMovementComponent>();
            goomba12.AddComponent<Gumba>();
            goomba12.AddComponent<CollisionComponent>();
            goomba12.AddComponent<Sprite>();
            goomba12.GetComponent<Sprite>().spriteId = "goomba";
            goomba12.GetComponent<CollisionComponent>().isStatic = false;
            goomba12.width = 48;
            goomba12.height = 48;
            goomba12.posX = goomba11.posX + 72;
            goomba12.posY = goomba11.posY;

            Entity goomba13 = Entity.CreateEntity("goomba13");
            goomba13.AddComponent<CharacterMovementComponent>();
            goomba13.AddComponent<Gumba>();
            goomba13.AddComponent<CollisionComponent>();
            goomba13.AddComponent<Sprite>();
            goomba13.GetComponent<Sprite>().spriteId = "goomba";
            goomba13.GetComponent<CollisionComponent>().isStatic = false;
            goomba13.width = 48;
            goomba13.height = 48;
            goomba13.posX = goomba11.posX + 192;
            goomba13.posY = goomba11.posY;

            Entity goomba14 = Entity.CreateEntity("goomba14");
            goomba14.AddComponent<CharacterMovementComponent>();
            goomba14.AddComponent<Gumba>();
            goomba14.AddComponent<CollisionComponent>();
            goomba14.AddComponent<Sprite>();
            goomba14.GetComponent<Sprite>().spriteId = "goomba";
            goomba14.GetComponent<CollisionComponent>().isStatic = false;
            goomba14.width = 48;
            goomba14.height = 48;
            goomba14.posX = goomba13.posX + 72;
            goomba14.posY = goomba13.posY;

            Entity goomba15 = Entity.CreateEntity("goomba15");
            goomba15.AddComponent<CharacterMovementComponent>();
            goomba15.AddComponent<Gumba>();
            goomba15.AddComponent<CollisionComponent>();
            goomba15.AddComponent<Sprite>();
            goomba15.GetComponent<Sprite>().spriteId = "goomba";
            goomba15.GetComponent<CollisionComponent>().isStatic = false;
            goomba15.width = 48;
            goomba15.height = 48;
            goomba15.posX = goomba13.posX + 2208;
            goomba15.posY = goomba13.posY;

            Entity goomba16 = Entity.CreateEntity("goomba16");
            goomba16.AddComponent<CharacterMovementComponent>();
            goomba16.AddComponent<Gumba>();
            goomba16.AddComponent<CollisionComponent>();
            goomba16.AddComponent<Sprite>();
            goomba16.GetComponent<Sprite>().spriteId = "goomba";
            goomba16.GetComponent<CollisionComponent>().isStatic = false;
            goomba16.width = 48;
            goomba16.height = 48;
            goomba16.posX = goomba15.posX + 72;
            goomba16.posY = goomba15.posY;

            AnimationData koopaData = new AnimationData();
            koopaData.startFrame = 0;
            koopaData.endFrame = 1;
            koopaData.frameRatePerSecond = 4;
            koopaData.width = 48;
            koopaData.height = 36;

            AnimationData gumbaaData = new AnimationData();
            gumbaaData.startFrame = 0;
            gumbaaData.endFrame = 1;
            gumbaaData.frameRatePerSecond = 4;
            gumbaaData.width = 48;
            gumbaaData.height = 48;

            SharedAnimationManager manager = Entity.rootEntity.GetComponent<SharedAnimationManager>();

            manager.AddAnimation("goomba", AnimationType.Idle, gumbaaData);
            manager.PlayAnim("goomba", AnimationType.Idle);

            manager.AddAnimation("koopa", AnimationType.Idle, koopaData);
            manager.PlayAnim("koopa", AnimationType.Idle);

            koopa.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba2.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba3.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba4.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba5.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba6.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba7.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba8.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba9.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba10.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba11.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba12.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba13.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba14.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba15.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
            goomba16.GetComponent<Sprite>().shouldUseSharedAnimationManager = true;
        }
    }
}
