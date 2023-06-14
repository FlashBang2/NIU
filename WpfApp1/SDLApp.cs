using ConsoleApp1;
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

            Entity skeleton = Entity.CreateEntity("Skeleton");
            skeleton.AddComponent<SkeletonComponent>();
            skeleton.AddComponent<CharacterMovementComponent>();
            skeleton.GetComponent<CharacterMovementComponent>().IsControlledByPlayer = true;
            skeleton.AddComponent<CollisionComponent>();
            skeleton.AddComponent<Sprite>();
            skeleton.GetComponent<Sprite>().spriteId = "mario_big";
            skeleton.GetComponent<CollisionComponent>().IsStatic = false;
            skeleton.Width = 48;
            skeleton.Height = 96;

            AnimationData data = new AnimationData();
            data.EndFrame = 7;
            data.StartFrame = 0;
            data.FrameRatePerSecond = 8;
            data.Width = 48;
            data.Height = 96;

            skeleton.GetComponent<Sprite>().AddAnimation(AnimationType.Idle, data);
            skeleton.GetComponent<Sprite>().PlayAnim(AnimationType.Idle);

            app.Run();
        }

        private static void LoadTextures()
        {
            SDLRendering.LoadTexture("firstPlatform.png", "first_platform");
            SDLRendering.LoadTexture("second_platform.png", "second_platform");
            SDLRendering.LoadTexture("third_platform.png", "third_platform");
            SDLRendering.LoadTexture("fourth_platform.png", "fourth_platform");
            SDLRendering.LoadTexture("mario_big.png", "mario_big");
            SDLRendering.LoadTexture("brick.png", "brick");
            SDLRendering.LoadTexture("questionmark.png", "question_block");
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

        private static void createDownPlatform (SDLApp app)
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

        private static void spawnEnemiesAtStartLocation()
        {

        }

        private static void spawnBackground() 
        {

        }
    }
}
