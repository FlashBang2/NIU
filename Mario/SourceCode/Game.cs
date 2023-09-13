using SDL2;

namespace Mario
{
    internal class Game
    {
        public static System.IntPtr _Renderer = System.IntPtr.Zero, gameMusic;
        public static int _ScrollSpeed = 0, inGameTime = 400;
        public static int[] immpasableBlocks = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 23, 26 };
        public static Map _CurrentLevel;
        public static string score = "000000";
        public bool _IsRunning = true, inMainMenu = true;
        public static GameObject _Player;
        private System.IntPtr _Window, fontTexture, fontTexture2, fontTexture3,
                fontTexture4, fontTexture5, fontTexture6, fontTexture7, fontTexture8,
                fontTexture9, fontTexture10;
        private GameObject[] _Enemies = new GameObject[17];
        private int[] _EnemiesPositionsX = new int[] {
            48 * 22,
            48 * 40,
            48 * 51,
            48 * 52 + 24,
            48 * 80,
            48 * 82,
            48 * 98,
            48 * 99 + 24,
            48 * 109,
            48 * 116,
            48 * 117 + 24,
            48 * 126,
            48 * 127 + 24,
            48 * 130,
            48 * 131 + 24,
            48 * 174,
            48 * 175 + 24
        };
        private int[] _EnemiesPositionsY = new int[] {
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - 48 * 11,
            1008 - 48 * 11,
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - (48 * 3 + 24),
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - 48 * 3,
            1008 - 48 * 3
        };
        public static class Controls
        {
            public static bool isPressingW = false;
            public static bool isPressingA = false;
            public static bool isPressingD = false;
            public static bool isPressingS = false;
            public static bool isPressingShift = false;
        }
        private SDL.SDL_Rect fontRectangle, fontRectangle2, fontRectangle3,
                fontRectangle4, fontRectangle5, fontRectangle6, fontRectangle7,
                fontRectangle8, fontRectangle9, fontRectangle10;
        private TextureManager.TextureInfo titleGraphic, coinIcon;
        public Game()
        {
            System.IO.Directory.SetCurrentDirectory("DLLs");

            if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
            {
                System.Console.WriteLine("Unable to initialize SDL_mixer. Error {0}", SDL.SDL_GetError());
            }
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO) < 0)
            {
                System.Console.WriteLine("Unable to initialize SDL. Error {0}", SDL.SDL_GetError());
            }
            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) != (int)(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG))
            {
                System.Console.WriteLine("Unable to initialize SDL_image. Error {0}", SDL.SDL_GetError());
            }
            if (SDL_ttf.TTF_Init() < 0)
            {
                System.Console.WriteLine("Unable to initialize SDL_ttf. Error {0}", SDL.SDL_GetError());
            }
            
            System.IO.Directory.SetCurrentDirectory("../");
        }

        public void Init(string title, int x, int y, int w, int h, SDL.SDL_WindowFlags flags)
        {
            _Window = SDL.SDL_CreateWindow(title, x, y, w, h, flags);
            if (_Window == null)
            {
                System.Console.WriteLine("Unable to create SDL window. Error {0}", SDL.SDL_GetError());
            }
            _Renderer = SDL.SDL_CreateRenderer(_Window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (_Renderer == null)
            {
                System.Console.WriteLine("Unable to create SDL renderer. Error{0}", SDL.SDL_GetError());
            }

            SDL.SDL_SetRenderDrawColor(_Renderer, 142, 140, 237, 255);
            _CurrentLevel = new Map("Data/Levels/1-1.xml");
            _Player = new Player("Assets/Characters/Player/marioSmall.png", 96, 864, 10);
            for (int i = 0; i < _Enemies.Length; i++)
            {
                if (i == 8)
                {
                    _Enemies[i] = new Enemy("Assets/Characters/Enemies/koopa.png", _EnemiesPositionsX[i], _EnemiesPositionsY[i], 2);
                    continue;
                }
                _Enemies[i] = new Enemy("Assets/Characters/Enemies/goomba.png", _EnemiesPositionsX[i], _EnemiesPositionsY[i], 2);
            }

            System.IntPtr fontPointer = SDL_ttf.TTF_OpenFont("Assets/Fonts/super-mario-bros-nes.ttf", 8);
            SDL.SDL_Color fontColor;
            fontColor.r = 255;
            fontColor.g = 255;
            fontColor.b = 255;
            fontColor.a = 255;
            System.IntPtr fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, "MARIO", fontColor);
            fontTexture = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
            SDL.SDL_FreeSurface(fontSurface);
            fontRectangle.x = 96;
            fontRectangle.y = 10;
            fontRectangle.w = 120;
            fontRectangle.h = 24;
            coinIcon = TextureManager.LoadTexture("Assets/HUD/coinIcon.png");
            titleGraphic = TextureManager.LoadTexture("Assets/HUD/Title.png");
            fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, " x 00", fontColor);
            fontTexture3 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
            SDL.SDL_FreeSurface(fontSurface);
            fontRectangle3.x = 596;
            fontRectangle3.y = 34;
            fontRectangle3.w = 120;
            fontRectangle3.h = 24;
            fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, "WORLD", fontColor);
            fontTexture4 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
            SDL.SDL_FreeSurface(fontSurface);
            fontRectangle4.x = 996;
            fontRectangle4.y = 10;
            fontRectangle4.w = 120;
            fontRectangle4.h = 24;
            fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, " 1-1 ", fontColor);
            fontTexture5 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
            SDL.SDL_FreeSurface(fontSurface);
            fontRectangle5.x = 996;
            fontRectangle5.y = 34;
            fontRectangle5.w = 120;
            fontRectangle5.h = 24;
            fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, " TIME", fontColor);
            fontTexture6 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
            SDL.SDL_FreeSurface(fontSurface);
            fontRectangle6.x = App.screenWidth - 246;
            fontRectangle6.y = 10;
            fontRectangle6.w = 120;
            fontRectangle6.h = 24;
            fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, "PRESS LMB TO", fontColor);
            fontTexture8 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
            SDL.SDL_FreeSurface(fontSurface);
            fontRectangle8.x = App.screenWidth / 2 - 200;
            fontRectangle8.y = App.screenHeight / 2 + 50;
            fontRectangle8.w = 288;
            fontRectangle8.h = 24;
            fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, "START NEW GAME", fontColor);
            fontTexture9 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
            SDL.SDL_FreeSurface(fontSurface);
            fontRectangle9.x = App.screenWidth / 2 - 200;
            fontRectangle9.y = App.screenHeight / 2 + 98;
            fontRectangle9.w = 336;
            fontRectangle9.h = 24;
            fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, "TOP- 000000", fontColor);
            fontTexture10 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
            SDL.SDL_FreeSurface(fontSurface);
            fontRectangle10.x = App.screenWidth / 2 - 200;
            fontRectangle10.y = App.screenHeight / 2 + 146;
            fontRectangle10.w = 264;
            fontRectangle10.h = 24;
        }

        public void HandleEvents()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        _IsRunning = false;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        if (inMainMenu) break;
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_w:
                                if (_Player.onGround)
                                {
                                    Controls.isPressingW = true;
                                }
                                break;
                            case SDL.SDL_Keycode.SDLK_d:
                                Controls.isPressingD = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_a:
                                Controls.isPressingA = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_s:
                                Controls.isPressingS = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_LSHIFT:
                                Controls.isPressingShift = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_RSHIFT:
                                Controls.isPressingShift = true;
                                break;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        if (inMainMenu) break;
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_w:
                                Controls.isPressingW = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_a:
                                Controls.isPressingA = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_d:
                                Controls.isPressingD = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_s:
                                Controls.isPressingS = false;
                                break;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        switch (e.button.button)
                        {
                            case (byte)SDL.SDL_BUTTON_LEFT:
                                if (inMainMenu)
                                {
                                    inMainMenu = false;
                                    SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
                                    gameMusic = SDL_mixer.Mix_LoadWAV("Assets/Music/OverworldTheme.wav");
                                    SDL_mixer.Mix_Volume(-1, 20);
                                    SDL_mixer.Mix_PlayChannel(-1, gameMusic, -1);
                                }
                                break;
                        }
                        break;
                }
            }
        }

        public void Update()
        {
            if (!_Player.isReseting || inGameTime > 0) 
            {
                _CurrentLevel.UpdateCameraOffset();
                _Player.Update();
                _Player.UpdateAnimation();
                for (int i = 0; i < _Enemies.Length; i++)
                {
                    _Enemies[i].Update();
                }
            }
        }

        public void Render()
        {
            if (!_Player.isReseting || inGameTime > 0)
            {
                SDL.SDL_RenderClear(_Renderer);
                _CurrentLevel.DrawMap();
                _Player.Render();
                for (int i = 0; i < _Enemies.Length; i++)
                {
                    _Enemies[i].Render();
                }
                TextureManager.DrawTexture(coinIcon, 572, 34, 3);

                System.IntPtr fontPointer = SDL_ttf.TTF_OpenFont("Assets/Fonts/super-mario-bros-nes.ttf", 8);
                SDL.SDL_Color fontColor;
                fontColor.r = 255;
                fontColor.g = 255;
                fontColor.b = 255;
                fontColor.a = 255;
                System.IntPtr fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, "  " + inGameTime, fontColor);
                fontTexture7 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
                SDL.SDL_FreeSurface(fontSurface);
                fontRectangle7.x = App.screenWidth - 246;
                fontRectangle7.y = 34;
                fontRectangle7.w = 120;
                fontRectangle7.h = 24;

                SDL.SDL_RenderCopy(_Renderer, fontTexture, System.IntPtr.Zero, ref fontRectangle);

                fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, score, fontColor);
                fontTexture2 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
                SDL.SDL_FreeSurface(fontSurface);
                fontRectangle2.x = 96;
                fontRectangle2.y = 34;
                fontRectangle2.w = 140;
                fontRectangle2.h = 24;

                SDL.SDL_RenderCopy(_Renderer, fontTexture2, System.IntPtr.Zero, ref fontRectangle2);
                SDL.SDL_RenderCopy(_Renderer, fontTexture3, System.IntPtr.Zero, ref fontRectangle3);
                SDL.SDL_RenderCopy(_Renderer, fontTexture4, System.IntPtr.Zero, ref fontRectangle4);
                SDL.SDL_RenderCopy(_Renderer, fontTexture5, System.IntPtr.Zero, ref fontRectangle5);
                SDL.SDL_RenderCopy(_Renderer, fontTexture6, System.IntPtr.Zero, ref fontRectangle6);
                if (!inMainMenu) SDL.SDL_RenderCopy(_Renderer, fontTexture7, System.IntPtr.Zero, ref fontRectangle7);
                if (inMainMenu) {
                    TextureManager.DrawTexture(titleGraphic, (App.screenWidth - titleGraphic.Width) / 2 - 50, (App.screenHeight - titleGraphic.Height) / 2 - 109, 1);
                    SDL.SDL_RenderCopy(_Renderer, fontTexture8, System.IntPtr.Zero, ref fontRectangle8);
                    SDL.SDL_RenderCopy(_Renderer, fontTexture9, System.IntPtr.Zero, ref fontRectangle9);
                    SDL.SDL_RenderCopy(_Renderer, fontTexture10, System.IntPtr.Zero, ref fontRectangle10);
                }
            }
            if (_Player.isReseting && inGameTime == 1)
            {
                SDL.SDL_SetRenderDrawColor(_Renderer, 0, 0, 0, 255);
                SDL.SDL_RenderClear(_Renderer);
                SDL.SDL_RenderPresent(_Renderer);
                SDL.SDL_Delay(5000);
                _Player.onGround = true;
                _Player.isEnding = false;
                _Player.isReseting = false;
                _Player.isWinning = false;
                _CurrentLevel.flagDescend = 0;
                inMainMenu = true;
                score = "000000";
                inGameTime = 400;
                _ScrollSpeed = 0;
                _CurrentLevel.cameraOffset = 0;
                SDL.SDL_SetRenderDrawColor(_Renderer, 142, 140, 237, 255);
            }
            SDL.SDL_RenderPresent(_Renderer);
        }

        public void CleanUp()
        {
            SDL.SDL_DestroyRenderer(_Renderer);
            _CurrentLevel.CleanMapTexture();
            _Player.Clean();
            for (int i = 0; i < _Enemies.Length; i++)
            {
                _Enemies[i].Clean();
            }
            SDL.SDL_DestroyWindow(_Window);
            SDL.SDL_Quit();
        }
    }
}
