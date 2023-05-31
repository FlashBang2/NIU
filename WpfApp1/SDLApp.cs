﻿using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public class SDLApp
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
            while (_isOpen)
            {
                while (SDL_PollEvent(out SDL_Event systemEvent) != 0)
                {
                    OnSystemEventOccured(systemEvent);
                }
                Entity.RootEntity.Tick(0);

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

            public override void ReceiveRender()
            {
                base.ReceiveRender();

                SDLRendering.FillRect((int)Owner.PosX, (int)Owner.PosY, (int)Owner.Width, (int)Owner.Height, Color.FromRgb(40, 128, 30));
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

            public override void ReceiveRender()
            {
                base.ReceiveRender();

                SDLRendering.FillCircle((int)Owner.PosX, (int)Owner.PosY, (int)(Owner.Width / 2), Color.FromRgb(120, 40, 40));
            }
        }

        public static void Main(string[] args)
        {
            SDLApp app = new SDLApp(960, 540, "NIU");

            Entity e = Entity.CreateEntity("Skeleton");
            Entity e2 = Entity.CreateEntity("FF");
            e.AddComponent<CollisionComponent>();
            e.AddComponent<SkeletonComponent>();
            e.GetComponent<CollisionComponent>().IsStatic = false;

            e2.AddComponent<RectRenderable>();
            e2.AddComponent<CollisionComponent>();

            SDLTimer t = new SDLTimer(2.0f, false);
            t.Tick += evt => Console.WriteLine(evt.TotalSeconds);

            app.Run();
        }
    }
}
