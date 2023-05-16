﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

                SDLRendering.RenderFrame();
            }

            FreeResources();
        }

        private void FreeResources()
        {
            SDL_DestroyRenderer(_renderer);
            SDL_DestroyWindow(_window);
            IMG_Quit();
            TTF_Quit();
            SDL_Quit();

            _window = IntPtr.Zero;
            _renderer = IntPtr.Zero;
            SDLRendering.Quit();
        }

        private void OnSystemEventOccured(SDL_Event evt)
        {
            if (evt.type == SDL_EventType.SDL_QUIT)
            {
                _isOpen = false;
            }
        }


        class Test : IRenderable
        {
            public bool ShouldDraw => true;

            public string SpriteTextureId => "";

            public int ZIndex { get => 0; set => throw new NotImplementedException(); }
            public Rect Bounds { get => new Rect(new Vector(120, 120), new Vector(240, 240)); set => throw new NotImplementedException(); }
            public double PosX { get => 120; set => throw new NotImplementedException(); }
            public double PosY { get => 120; set => throw new NotImplementedException(); }
            public double RotationAngle { get => 0; set => throw new NotImplementedException(); }
            public double CircleRadius { get => 0; set => throw new NotImplementedException(); }
            public Color ProxyShapeColor { get => Color.FromRgb(125, 123, 255); set => throw new NotImplementedException(); }
            public Rect SourceTextureBounds { get => new Rect(new Vector(), new Vector()); set => throw new NotImplementedException(); }

            public object LinkedEntity => throw new NotImplementedException();

            public RenderMode RenderingMode => RenderMode.Rect;

            public event Action<int> ZIndexChanged;

            public bool IsVisible(Rect cameraBounds)
            {
                return true;
            }
        }


        public static void Main(string[] args)
        {
            SDLApp app = new SDLApp(960, 540, "NIU");
            SDLRendering.AddRenderable(new Test());

            IEntity e;
            app.Run();
        }
    }
}