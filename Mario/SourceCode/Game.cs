using System;
using System.IO;
using SDL2;

namespace Mario
{
    internal class Game
    {
        public static class Controls
        {
            public static bool isPressingW = false, isPressingA = false, isPressingD = false, isPressingS = false, isPressingShift = false;
        }
        private SDL.SDL_Rect fontRectangle, fontRectangle2, fontRectangle3,
                fontRectangle4, fontRectangle5, fontRectangle6, fontRectangle7,
                fontRectangle8, fontRectangle9, fontRectangle10;
        public static Map _CurrentLevel;
        public static GameObject _Player;
        public static IntPtr _Renderer = IntPtr.Zero, gameMusic = IntPtr.Zero, joystick= IntPtr.Zero;
        public static int[] immpasableBlocks = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 23, 26 };
        public static int _ScrollSpeed = 0, inGameTime = 400, flagHeight = 700, counter = 0, pointer = 0;
        public static string score = "000000", highScore = "TOP- ", highscoreValue = "000000";
        public bool _IsRunning = true, inMainMenu = true, fireworkSoundInPlay = false;
        private IntPtr _Window, fontTexture, fontTexture2, fontTexture3, fontTexture4,
                        fontTexture5, fontTexture6, fontTexture7, fontTexture8, fontTexture9,
                        fontTexture10;
        private int[] _EnemiesPositionsX = new int[] {
            48 * 22,        48 * 40,        48 * 51,        48 * 52 + 24,   48 * 80,
            48 * 82,        48 * 98,        48 * 99 + 24,   48 * 109,       48 * 116,
            48 * 117 + 24,  48 * 126,       48 * 127 + 24,  48 * 130,       48 * 131 + 24,
            48 * 174,       48 * 175 + 24
        };
        private int[] _EnemiesPositionsY = new int[] {
            1008 - 48 * 3,  1008 - 48 * 3,  1008 - 48 * 3,  1008 - 48 * 3,          1008 - 48 * 11,
            1008 - 48 * 11, 1008 - 48 * 3,  1008 - 48 * 3,  1008 - (48 * 3 + 24),   1008 - 48 * 3,
            1008 - 48 * 3,  1008 - 48 * 3,  1008 - 48 * 3,  1008 - 48 * 3,          1008 - 48 * 3,
            1008 - 48 * 3,  1008 - 48 * 3
        };
        private GameObject[] _Enemies = new GameObject[17];
        private TextureManager.TextureInfo titleGraphic, coinIcon, castleFlag, firework;
        private static Random rand = new Random();
        private int[,] fireworkPositions = new int[,]
        {
             
            {rand.Next(1000, 1500), rand.Next(flagHeight - 480, flagHeight - 48)}, {rand.Next(1000, 1500), rand.Next(flagHeight - 480, flagHeight - 48)},
            {rand.Next(1000, 1500), rand.Next(flagHeight - 480, flagHeight - 48)}, {rand.Next(1000, 1500), rand.Next(flagHeight - 480, flagHeight - 48)},
            {rand.Next(1000, 1500), rand.Next(flagHeight - 480, flagHeight - 48)}, {rand.Next(1000, 1500), rand.Next(flagHeight - 480, flagHeight - 48)}
        };

        public Game()
        {
            Directory.SetCurrentDirectory("DLLs");

            if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
            {
                Console.WriteLine("Unable to initialize SDL_mixer. Error {0}", SDL.SDL_GetError());
            }
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_JOYSTICK) < 0)
            {
               Console.WriteLine("Unable to initialize SDL. Error {0}", SDL.SDL_GetError());
            }
            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) != (int)(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG))
            {
                Console.WriteLine("Unable to initialize SDL_image. Error {0}", SDL.SDL_GetError());
            }
            if (SDL_ttf.TTF_Init() < 0)
            {
                Console.WriteLine("Unable to initialize SDL_ttf. Error {0}", SDL.SDL_GetError());
            }

            joystick = SDL.SDL_JoystickOpen(0);

            Directory.SetCurrentDirectory("../");
            if (File.Exists("Data/highscore.bin"))
            {
                BinaryReader binaryReader = new BinaryReader(new FileStream("Data/highscore.bin", FileMode.Open));
                int value = binaryReader.ReadInt32();
                highscoreValue = value.ToString();
                binaryReader.Close();
            }
            new Kinnect();
        }

        public void Init(string title, int x, int y, int w, int h, SDL.SDL_WindowFlags flags)
        {
            _Window = SDL.SDL_CreateWindow(title, x, y, w, h, flags);
            if (_Window == null)
            {
                Console.WriteLine("Unable to create SDL window. Error {0}", SDL.SDL_GetError());
            }
            _Renderer = SDL.SDL_CreateRenderer(_Window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (_Renderer == null)
            {
                Console.WriteLine("Unable to create SDL renderer. Error{0}", SDL.SDL_GetError());
            }

            SDL.SDL_SetRenderDrawColor(_Renderer, 142, 140, 237, 255);
            _CurrentLevel = new Map("Data/Levels/1-1.xml");
            _Player = new Player("Assets/Characters/Player/marioSmall.png", 96, 864, 10);
            for (int i = 0; i < _Enemies.Length; i++)
            {
                if (i == 8)
                {
                    _Enemies[i] = new Enemy("Assets/Characters/Enemies/koopa.png", _EnemiesPositionsX[i], _EnemiesPositionsY[i], 2, "koopa");
                    continue;
                }
                _Enemies[i] = new Enemy("Assets/Characters/Enemies/goomba.png", _EnemiesPositionsX[i], _EnemiesPositionsY[i], 3, "goomba");
            }

            castleFlag = TextureManager.LoadTexture("Assets/Tiles/1-1/castleFlag.png");
            coinIcon = TextureManager.LoadTexture("Assets/HUD/coinIcon.png");
            titleGraphic = TextureManager.LoadTexture("Assets/HUD/Title.png");
            firework = TextureManager.LoadTexture("Assets/Animations/fireworks.png");
            IntPtr fontPointer = SDL_ttf.TTF_OpenFont("Assets/Fonts/super-mario-bros-nes.ttf", 8);
            SDL.SDL_Color fontColor;
            fontColor.r = 255;
            fontColor.g = 255;
            fontColor.b = 255;
            fontColor.a = 255;
            IntPtr fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, "MARIO", fontColor);
            fontTexture = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
            SDL.SDL_FreeSurface(fontSurface);
            fontRectangle.x = 96;
            fontRectangle.y = 10;
            fontRectangle.w = 120;
            fontRectangle.h = 24;
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
                                if (_Player.onGround) Controls.isPressingW = true;
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
                                    SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 4, 2048);
                                    gameMusic = SDL_mixer.Mix_LoadWAV("Assets/Music/OverworldTheme.wav");
                                    SDL_mixer.Mix_Volume(-1, 20);
                                    SDL_mixer.Mix_PlayChannel(-1, gameMusic, -1);
                                }
                                break;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_JOYAXISMOTION:
                        if (inMainMenu) break;
                        if (e.jaxis.axis == 0)
                        {
                            
                            if (e.jaxis.axisValue < -3200)
                            {
                                Controls.isPressingA = true;
                            }
                            if (e.jaxis.axisValue > 3200)
                            {
                                Controls.isPressingD = true;
                            }
                            if (e.jaxis.axisValue >= -3200 && e.jaxis.axisValue <= 3200)
                            {
                                Controls.isPressingA = false;
                                Controls.isPressingD = false;
                            }
                        }
                        if (Controls.isPressingA && Controls.isPressingD)
                        {
                            Controls.isPressingA = false;
                            Controls.isPressingD = false;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_JOYBUTTONDOWN:
                        if ((SDL.SDL_GameControllerButton)e.jbutton.button == SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START && inMainMenu)
                        {
                            inMainMenu = false;
                            SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 4, 2048);
                            gameMusic = SDL_mixer.Mix_LoadWAV("Assets/Music/OverworldTheme.wav");
                            SDL_mixer.Mix_Volume(-1, 20);
                            SDL_mixer.Mix_PlayChannel(-1, gameMusic, -1);
                        }
                        if (inMainMenu) break;
                        if ((SDL.SDL_GameControllerButton)e.jbutton.button == SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A && _Player.onGround)
                        {
                            Controls.isPressingW = true;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_JOYBUTTONUP:
                        if (inMainMenu) break;
                        if ((SDL.SDL_GameControllerButton)e.jbutton.button == SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A)
                        {
                            Controls.isPressingW = false;
                        }
                        break;
                }
            }
        }

        public void Update()
        {
            if (inGameTime == 0 && !_Player.isWinning) _Player.isDying = true;
            if (!_Player.isReseting || inGameTime > 0) 
            {
                _CurrentLevel.UpdateCameraOffset();
                _Player.Update();
                _Player.UpdateAnimation();
                for (int i = 0; i < _Enemies.Length; i++)
                {
                    if (!inMainMenu) _Enemies[i].Update();
                    _Enemies[i].UpdateAnimation();
                }
            }
        }

        public void Render()
        {
            if (!_Player.isReseting || inGameTime >= 0)
            {
                SDL.SDL_RenderClear(_Renderer);
                _CurrentLevel.DrawMap();
                _Player.Render();
                for (int i = 0; i < _Enemies.Length; i++)
                {
                    _Enemies[i].Render();
                }
                TextureManager.DrawTexture(coinIcon, 572, 34, 3);

                IntPtr fontPointer = SDL_ttf.TTF_OpenFont("Assets/Fonts/super-mario-bros-nes.ttf", 8);
                SDL.SDL_Color fontColor;
                fontColor.r = 255;
                fontColor.g = 255;
                fontColor.b = 255;
                fontColor.a = 255;
                IntPtr fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, "  " + inGameTime, fontColor);
                fontTexture7 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
                SDL.SDL_FreeSurface(fontSurface);
                fontRectangle7.x = App.screenWidth - 246;
                fontRectangle7.y = 34;
                fontRectangle7.w = 120;
                fontRectangle7.h = 24;

                SDL.SDL_RenderCopy(_Renderer, fontTexture, IntPtr.Zero, ref fontRectangle);

                fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, score, fontColor);
                fontTexture2 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
                SDL.SDL_FreeSurface(fontSurface);
                fontRectangle2.x = 96;
                fontRectangle2.y = 34;
                fontRectangle2.w = 140;
                fontRectangle2.h = 24;

                SDL.SDL_RenderCopy(_Renderer, fontTexture2, IntPtr.Zero, ref fontRectangle2);
                SDL.SDL_RenderCopy(_Renderer, fontTexture3, IntPtr.Zero, ref fontRectangle3);
                SDL.SDL_RenderCopy(_Renderer, fontTexture4, IntPtr.Zero, ref fontRectangle4);
                SDL.SDL_RenderCopy(_Renderer, fontTexture5, IntPtr.Zero, ref fontRectangle5);
                SDL.SDL_RenderCopy(_Renderer, fontTexture6, IntPtr.Zero, ref fontRectangle6);
                if (!inMainMenu) SDL.SDL_RenderCopy(_Renderer, fontTexture7, IntPtr.Zero, ref fontRectangle7);
                if (inMainMenu) {
                    TextureManager.DrawTexture(titleGraphic, (App.screenWidth - titleGraphic.Width) / 2 - 50, (App.screenHeight - titleGraphic.Height) / 2 - 109, 1);
                    SDL.SDL_RenderCopy(_Renderer, fontTexture8, IntPtr.Zero, ref fontRectangle8);
                    SDL.SDL_RenderCopy(_Renderer, fontTexture9, IntPtr.Zero, ref fontRectangle9);

                    fontSurface = SDL_ttf.TTF_RenderText_Solid(fontPointer, highScore + highscoreValue, fontColor);
                    fontTexture10 = SDL.SDL_CreateTextureFromSurface(_Renderer, fontSurface);
                    SDL.SDL_FreeSurface(fontSurface);
                    fontRectangle10.x = App.screenWidth / 2 - 200;
                    fontRectangle10.y = App.screenHeight / 2 + 146;
                    fontRectangle10.w = 264;
                    fontRectangle10.h = 24;

                    SDL.SDL_RenderCopy(_Renderer, fontTexture10, IntPtr.Zero, ref fontRectangle10);
                }
            }
            if (_Player.isReseting && inGameTime == 0)
            {
                if (flagHeight > 635) flagHeight -= 2; 
                TextureManager.DrawTexture(castleFlag, 1240, flagHeight, 1);
                _CurrentLevel.DrawMap();
                Player player = _Player as Player;
                if (flagHeight <= 635 && (player.endTimer == 6 || player.endTimer == 1 || player.endTimer == 3))
                {
                   if (!fireworkSoundInPlay)
                   {
                        fireworkSoundInPlay = true;
                        IntPtr bumpSFX = SDL_mixer.Mix_LoadWAV("Assets/SFX/bump.wav");
                        SDL_mixer.Mix_Volume(2, 60);
                        SDL_mixer.Mix_PlayChannel(2, bumpSFX, 0);
                    } 
                   TextureManager.DrawTexture(firework, fireworkPositions[pointer, 0], fireworkPositions[pointer, 1], 3);
                   counter++;
                    if (counter > 24 && pointer < player.endTimer - 1)
                   {
                        fireworkSoundInPlay = false;
                        counter = 0;
                        int newValue = int.Parse(score) + 500;
                        string replaceValue = newValue.ToString();
                        while (replaceValue.Length < 6)
                        {
                            replaceValue = "0" + replaceValue;
                        }
                        score = replaceValue;
                        pointer++;
                   }
                }
                if ((counter > 24 && pointer == player.endTimer - 1) || (player.endTimer != 1 && player.endTimer != 3 && player.endTimer != 6))
                {
                    if (File.Exists("Data/highscore.bin"))
                    {
                        BinaryReader binaryRedaer = new BinaryReader(new FileStream("Data/highscore.bin", FileMode.Open));
                        int value = binaryRedaer.ReadInt32();
                        if (int.Parse(highscoreValue) < value) highscoreValue = value.ToString();
                        while (highscoreValue.Length < 6)
                        {
                            highscoreValue = "0" + highscoreValue;
                        }
                        binaryRedaer.Close();
                        if (int.Parse(score) > int.Parse(highscoreValue))
                        {
                            highscoreValue = score;
                            BinaryWriter binaryWriter = new BinaryWriter(File.Open("Data/highscore.bin", FileMode.Create));
                            binaryWriter.Write(int.Parse(score));
                            binaryWriter.Close();
                        }
                    }
                    else
                    {
                        BinaryWriter binaryWriter = new BinaryWriter(File.Open("Data/highscore.bin", FileMode.Create));
                        binaryWriter.Write(int.Parse(score));
                        binaryWriter.Close();
                        highscoreValue = score.ToString();
                    }
                    SDL.SDL_SetRenderDrawColor(_Renderer, 0, 0, 0, 255);
                    SDL.SDL_RenderClear(_Renderer);
                    SDL.SDL_RenderPresent(_Renderer);
                    SDL.SDL_Delay(2000);
                    _Player.onGround = true;
                    _Player.isEnding = false;
                    _Player.isReseting = false;
                    _Player.isWinning = false;
                    fireworkSoundInPlay = false;
                    _CurrentLevel.flagDescend = 0;
                    _CurrentLevel.cameraOffset = 0;
                    inMainMenu = true;
                    score = "000000";
                    inGameTime = 400;
                    _ScrollSpeed = 0;
                    flagHeight = 700;
                    pointer = 0;
                    counter = 0;
                    for (int i = 0; i < _Enemies.Length; i++)
                    {
                        _Enemies[i].velocityX = -2;
                        _Enemies[i].velocityY = 0;
                        _Enemies[i]._positionX = _EnemiesPositionsX[i];
                        _Enemies[i]._positionY = _EnemiesPositionsY[i];
                    }
                    SDL.SDL_SetRenderDrawColor(_Renderer, 142, 140, 237, 255);
                }
            }
            if (_Player.hasLost)
            {
                _Player.velocityY += 1;
                _Player._positionY += _Player.velocityY;
                if (_Player._positionY > App.screenHeight)
                {
                    _Player._positionX = 96;
                    _Player._positionY = 864;
                    _Player.velocityY = 0;
                    _Player.velocityX = 0;
                    _Player.onGround = true;
                    _Player.isDying = false;
                    _Player.hasReachedPit = false;
                    _Player.hasLost = false;
                    _Player.isEnding = false;
                    _Player.isReseting = false;
                    _Player.isWinning = false;
                    inMainMenu = true;
                    _CurrentLevel.cameraOffset = 0;
                    score = "000000";
                    inGameTime = 400;
                    _ScrollSpeed = 0;
                    Controls.isPressingS = false;
                    Controls.isPressingW = false;
                    Controls.isPressingD = false;
                    Controls.isPressingA = false;
                    for (int i = 0; i < _Enemies.Length; i++)
                    {
                        _Enemies[i].velocityX = -2;
                        _Enemies[i].velocityY = 0;
                        _Enemies[i]._positionX = _EnemiesPositionsX[i];
                        _Enemies[i]._positionY = _EnemiesPositionsY[i];
                    }
                }
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
            SDL.SDL_JoystickClose(joystick);
            SDL.SDL_DestroyWindow(_Window);
            SDL.SDL_Quit();
        }
    }
}
