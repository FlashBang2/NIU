using SDL2;
using System;
using Mario.SourceCode;

namespace Mario
{
    internal class Game
    {
        public static IntPtr Renderer = IntPtr.Zero;
        public static int ScrollSpeed = 0;
        public static Map CurrentLevel;

        const int AvailableNumFonts = 10;

        public static int[] immpasableBlocks =
        {
            1, 2, 3, 4, 5, 6, 7, 8, 23, 25, 26
        };

        public bool IsRunning = true;
        public static int inGameTime = 400;
        public static string score = "000000";
        public static bool _inMainMenu = true;

        public struct Font
        {
            public IntPtr Texture;
            public Predicate<Font> AvailabilityCondition;
            public SDL.SDL_Rect RenderRectangle;

            public Font(IntPtr texture, Predicate<Font> availabilityCondition, int fontX, int fontY, int fontWidth, int fontHeight) : this()
            {
                Texture = texture;
                AvailabilityCondition = availabilityCondition;
                RenderRectangle = App.AssignValuesForRectangle(fontX, fontY, fontWidth, fontHeight);
            }
        }

        private IntPtr _window;
        private Font[] _fonts = new Font[AvailableNumFonts];

        public static GameObject _player;

        private GameObject[] _enemies = new GameObject[17];
        private int[] _enemiesPositionsX = new int[] {
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

        private int[] _enemiesPositionsY = new int[] {
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

        private readonly int[] _fontRects =
        {
            96, 10, 120, 24,
            96, 34, 140, 24,
            596, 34, 120, 24,
            996, 10, 120, 24,
            996, 34, 120, 24,
            App.ScreenWidth-246, 10, 120, 24,
            App.ScreenWidth-246, 34, 120, 24,
            App.ScreenWidth / 2, App.ScreenHeight / 2, 288, 24,
            App.ScreenWidth / 2, App.ScreenHeight / 2 - 24, 336, 24,
            App.ScreenWidth / 2, App.ScreenHeight / 2 - 48, 264, 24
        };

        private readonly string[] _fontLabels =
        {
            "MARIO",
            "000000",
            " x 00",
            "WORLD",
            " 1-1 ",
            " TIME",
            "  400",
            "PRESS LMB TO",
            "START NEW GAME",
            "TOP- 000000"
        };

        public static class Controls
        {
            public static bool isPressingW = false;
            public static bool isPressingA = false;
            public static bool isPressingD = false;
            public static bool isPressingS = false;
            public static bool isPressingShift = false;

            public static bool ShouldDoRightAction()
            {
                return isPressingD || joystick.IsRightActionPressed;
            }

            public static bool ShouldDoLeftAction()
            {
                return isPressingA || joystick.IsLeftActionPressed;
            }

            public static bool ShouldDoJumpAction()
            {
                return isPressingW || joystick.IsJumpActionPressed;
            }
        }

        public interface Joystick
        {
            bool IsRightActionPressed { get; }
            bool IsLeftActionPressed { get; }
            bool IsJumpActionPressed { get; }

            void UpdateByEvent(ref SDL.SDL_Event evt);
        }

        class NullJoystick : Joystick
        {
            public bool IsRightActionPressed => false;
            public bool IsLeftActionPressed => false;
            public bool IsJumpActionPressed => false;

            public void UpdateByEvent(ref SDL.SDL_Event evt) { }
        }

        private class SdlJoystick : Joystick
        {
            private bool _isRightActionPressed = false;
            private bool _isLeftActionPressed = false;
            private bool _isJumpActionPressed = false;

            // _joystick must be kept, because it would crash, if any of the unmanaged code use it
            private IntPtr _joystick;

            public SdlJoystick()
            {
                _joystick = SDL.SDL_JoystickOpen(0);
            }

            public bool IsRightActionPressed => _isRightActionPressed;
            public bool IsLeftActionPressed => _isLeftActionPressed;
            public bool IsJumpActionPressed => _isJumpActionPressed;

            public void UpdateByEvent(ref SDL.SDL_Event evt)
            {

                if (_inMainMenu)
                {
                    if (ShouldStartGame(evt))
                    {
                        StartPlayingGame();
                    }

                    return;
                }

                if (!IsJoystickEvent(ref evt))
                {
                    return;
                }

                if (evt.type == SDL.SDL_EventType.SDL_JOYHATMOTION)
                {
                    UpdateMovementButtons(ref evt);
                    return;
                }


                SDL.SDL_GameControllerButton button = (SDL.SDL_GameControllerButton)evt.jbutton.button;
                switch (button)
                {
                    case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X:
                        TryUpdateJumpButton(ref evt);
                        break;
                }
            }

            private static bool ShouldStartGame(SDL.SDL_Event evt)
            {
                return evt.type == SDL.SDL_EventType.SDL_JOYBUTTONDOWN && evt.jbutton.button == (byte)SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START;
            }

            private void UpdateMovementButtons(ref SDL.SDL_Event evt)
            {
                if (evt.jhat.hatValue == SDL.SDL_HAT_LEFT)
                {
                    _isLeftActionPressed = true;
                    _isRightActionPressed = false;
                }
                else if (evt.jhat.hatValue == SDL.SDL_HAT_RIGHT)
                {
                    _isRightActionPressed = true;
                    _isLeftActionPressed = false;
                }
                else if (evt.jhat.hatValue == SDL.SDL_HAT_CENTERED)
                {
                    _isRightActionPressed = false;
                    _isLeftActionPressed = false;
                }
            }

            private static bool IsJoystickEvent(ref SDL.SDL_Event evt)
            {
                return evt.type == SDL.SDL_EventType.SDL_JOYBUTTONDOWN || evt.type == SDL.SDL_EventType.SDL_JOYBUTTONUP ||
                                    evt.type == SDL.SDL_EventType.SDL_JOYHATMOTION;
            }

            private static void StartPlayingGame()
            {
                _inMainMenu = false;
                SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
                gameMusic = SDL_mixer.Mix_LoadWAV("Assets/Music/OverworldTheme.wav");
                SDL_mixer.Mix_Volume(-1, 20);
                SDL_mixer.Mix_PlayChannel(-1, gameMusic, -1);
            }

            private void TryUpdateJumpButton(ref SDL.SDL_Event evt)
            {
                if (_player.IsTouchingGround)
                {
                    _isJumpActionPressed = evt.type == SDL.SDL_EventType.SDL_JOYBUTTONDOWN;
                }
                else
                {
                    _isJumpActionPressed = false;
                }
            }
        }


        private TextureManager.TextureInfo _titleGraphic;
        private TextureManager.TextureInfo _coinIcon;
        private Kinnect kinnect;
        private IntPtr fontPointer;
        private bool _isJoystickEnabled = false;
        public static Joystick joystick;

        public static IntPtr gameMusic;

        public Game()
        {
            System.IO.Directory.SetCurrentDirectory("DLLs");

            if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
            {
                string exceptionMessage = string.Format("Unable to initialize SDL_mixer. Error {0}", SDL.SDL_GetError());
                throw new ApplicationException(exceptionMessage);
            }

            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
            {
                string exceptionMessage = string.Format("Unable to initialize SDL. Error {0}", SDL.SDL_GetError());
                throw new ApplicationException(exceptionMessage);
            }

            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) != (int)(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG))
            {
                string exceptionMessage = string.Format("Unable to initialize SDL_image. Error {0}", SDL.SDL_GetError());
                throw new ApplicationException(exceptionMessage);
            }
            if (SDL_ttf.TTF_Init() < 0)
            {
                string exceptionMessage = string.Format("Unable to initialize SDL_ttf. Error {0}", SDL.SDL_GetError());
                throw new ApplicationException(exceptionMessage);
            }

            SetNewJoystickImplementation();

            System.IO.Directory.SetCurrentDirectory("../");
            kinnect = new Kinnect();
        }

        private void SetNewJoystickImplementation()
        {
            _isJoystickEnabled = SDL.SDL_NumJoysticks() > 0;
            if (_isJoystickEnabled)
            {
                joystick = new SdlJoystick();
            }
            else
            {
                joystick = new NullJoystick();
            }
        }

        public void Init(string title, int x, int y, int w, int h, SDL.SDL_WindowFlags flags)
        {
            _window = SDL.SDL_CreateWindow(title, x, y, w, h, flags);
            if (_window == IntPtr.Zero)
            {
                string exceptionMessage = string.Format("Unable to create SDL window. Error {0}", SDL.SDL_GetError());
                throw new ApplicationException(exceptionMessage);
            }

            Renderer = SDL.SDL_CreateRenderer(_window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            if (Renderer == IntPtr.Zero)
            {
                string exceptionMessage = string.Format("Unable to create SDL renderer. Error{0}", SDL.SDL_GetError());
                throw new ApplicationException(exceptionMessage);
            }

            SDL.SDL_SetRenderDrawColor(Renderer, 142, 140, 237, 255);
            CurrentLevel = new Map("Data/Levels/1-1.xml");
            _player = new Player("Assets/Characters/Player/marioSmall.png", 96, 864, 10);

            for (int i = 0; i < _enemies.Length; i++)
            {
                if (i == 8)
                {
                    _enemies[i] = new Enemy("Assets/Characters/Enemies/koopa.png", _enemiesPositionsX[i], _enemiesPositionsY[i], 2, "koopa");
                    continue;
                }
                _enemies[i] = new Enemy("Assets/Characters/Enemies/goomba.png", _enemiesPositionsX[i], _enemiesPositionsY[i], 2, "goomba");
            }

            GenerateFonts();

            _coinIcon = TextureManager.LoadTexture("Assets/HUD/coinIcon.png");
            _titleGraphic = TextureManager.LoadTexture("Assets/HUD/Title.png");
        }

        private void GenerateFonts()
        {
            fontPointer = SDL_ttf.TTF_OpenFont("Assets/Fonts/super-mario-bros-nes.ttf", 8);

            if (fontPointer == IntPtr.Zero)
            {
                string exceptionMessage = string.Format("Error occured during loading font \'Assets/Fonts/super-mario-bros-nes.ttf\': Error {0}", SDL.SDL_GetError());
                throw new ApplicationException(exceptionMessage);
            }

            SDL.SDL_Color fontColor;
            fontColor.r = 255;
            fontColor.g = 255;
            fontColor.b = 255;
            fontColor.a = 255;

            for (int i = 0; i < AvailableNumFonts; i++)
            {
                IntPtr fontRenderSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, _fontLabels[i], fontColor);
                IntPtr texture = SDL.SDL_CreateTextureFromSurface(Renderer, fontRenderSurface);
                SDL.SDL_FreeSurface(fontRenderSurface);

                _fonts[i] = new Font(texture, _font => true, _fontRects[i * 4], _fontRects[i * 4 + 1], _fontRects[i * 4 + 2], _fontRects[i * 4 + 3]);
            }

            _fonts[6].AvailabilityCondition = _font => !_inMainMenu;

            _fonts[7].AvailabilityCondition = _font => _inMainMenu;
            _fonts[8].AvailabilityCondition = _font => _inMainMenu;
            _fonts[9].AvailabilityCondition = _font => _inMainMenu;
        }

        public void HandleEvents()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event evt) != 0)
            {
                switch (evt.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        IsRunning = false;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        UpdateKeyUpState(evt);
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        UpdateKeyDownState(evt);
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        StartPlayingGame(evt);
                        break;
                    case SDL.SDL_EventType.SDL_JOYDEVICEADDED:
                    case SDL.SDL_EventType.SDL_JOYDEVICEREMOVED:
                        SetNewJoystickImplementation();
                        break;
                }

                joystick.UpdateByEvent(ref evt);
            }
        }

        private void UpdateKeyUpState(SDL.SDL_Event evt)
        {
            if (_inMainMenu)
            {
                return;
            }

            switch (evt.key.keysym.sym)
            {
                case SDL.SDL_Keycode.SDLK_w:
                    if (_player.IsTouchingGround)
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
        }

        private void UpdateKeyDownState(SDL.SDL_Event evt)
        {
            if (_inMainMenu)
            {
                return;
            }

            switch (evt.key.keysym.sym)
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
        }

        private void StartPlayingGame(SDL.SDL_Event e)
        {
            const byte LeftMouseButton = (byte)SDL.SDL_BUTTON_LEFT;

            // Block user from playing too many music in same time
            if (!_inMainMenu)
            {
                return;
            }

            switch (e.button.button)
            {
                case LeftMouseButton:
                    {
                        _inMainMenu = false;
                        SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
                        gameMusic = SDL_mixer.Mix_LoadWAV("Assets/Music/OverworldTheme.wav");
                        SDL_mixer.Mix_Volume(-1, 20);
                        SDL_mixer.Mix_PlayChannel(-1, gameMusic, -1);
                        break;
                    }
            }
        }

        public void Update()
        {
            if (!_player.IsReseting || inGameTime > 0)
            {
                CurrentLevel.UpdateMap();
                _player.Update();
                _player.UpdateAnimation();
                for (int i = 0; i < _enemies.Length; i++)
                {
                    if (!_inMainMenu) _enemies[i].Update();
                    _enemies[i].UpdateAnimation();
                }
            }
        }

        public void Render()
        {
            if (!_player.IsReseting || inGameTime > 0)
            {
                SDL.SDL_RenderClear(Renderer);
                CurrentLevel.DrawMap();
                _player.Render();

                for (int i = 0; i < _enemies.Length; i++)
                {
                    _enemies[i].Render();
                }

                TextureManager.DrawTexture(_coinIcon, 572, 34, 3);
                SDL.SDL_Color fontColor;
                fontColor.r = 255;
                fontColor.g = 255;
                fontColor.b = 255;
                fontColor.a = 255;

                var fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, score, fontColor);
                SDL.SDL_DestroyTexture(_fonts[1].Texture);
                _fonts[1].Texture = SDL.SDL_CreateTextureFromSurface(Renderer, fontSurface);
                SDL.SDL_FreeSurface(fontSurface);

                for (int i = 0; i < _fonts.Length; i++)
                {
                    if (_fonts[i].AvailabilityCondition(_fonts[i]))
                    {
                        SDL.SDL_RenderCopy(Renderer, _fonts[i].Texture, IntPtr.Zero, ref _fonts[i].RenderRectangle);
                    }
                }

                if (_player.IsReseting && inGameTime == 1)
                {
                    SDL.SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 255);
                    SDL.SDL_RenderClear(Renderer);
                    SDL.SDL_RenderPresent(Renderer);
                    SDL.SDL_Delay(2000);
                    _player.IsTouchingGround = true;
                    _player.IsEnding = false;
                    _player.IsReseting = false;
                    _player.IsWinning = false;
                    CurrentLevel.FlagDescend = 0;
                    CurrentLevel.CameraOffset = 0;
                    _inMainMenu = true;
                    score = "000000";
                    inGameTime = 400;
                    ScrollSpeed = 0;
                    for (int i = 0; i < _enemies.Length; i++)
                    {
                        _enemies[i]._positionX = _enemiesPositionsX[i];
                        _enemies[i]._positionY = _enemiesPositionsY[i];
                    }
                    SDL.SDL_SetRenderDrawColor(Renderer, 142, 140, 237, 255);
                }
                if (_player.HasLost)
                {
                    _player.velocityY += 1;
                    _player._positionY += _player.velocityY;
                    if (_player._positionY > App.ScreenHeight)
                    {
                        _player._positionX = 96;
                        _player._positionY = 864;
                        _player.velocityY = 0;
                        _player.IsTouchingGround = true;
                        _player.IsDying = false;
                        _player.HasReachedPit = false;
                        _player.HasLost = false;
                        _player.IsEnding = false;
                        _player.IsReseting = false;
                        _player.IsWinning = false;
                        _inMainMenu = true;
                        CurrentLevel.CameraOffset = 0;
                        score = "000000";
                        inGameTime = 400;
                        ScrollSpeed = 0;
                        for (int i = 0; i < _enemies.Length; i++)
                        {
                            _enemies[i]._positionX = _enemiesPositionsX[i];
                            _enemies[i]._positionY = _enemiesPositionsY[i];
                        }
                    }

                }
                SDL.SDL_RenderPresent(Renderer);
            }
            else if (_player.IsReseting)
            {
                Console.Write("OK\n");
            }
        }

        public void CleanUp()
        {
            SDL.SDL_DestroyRenderer(Renderer);
            CurrentLevel.CleanMapTexture();
            _player.Clean();

            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i].Clean();
            }

            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
        }
    }
}
