using SDL2;
using System;

namespace Mario
{
    internal class App
    {
        public const int screenWidth = 1792, screenHeight = 1008, FPS = 60, desiredFrameTime = 1000 / FPS;
        public static int gameFrameTime, frame = 0;
        private static IntPtr warning = IntPtr.Zero;

        public static SDL.SDL_Rect AssignValuesForRectangle (int x, int y, int w, int h)
        {
            SDL.SDL_Rect rect;
            rect.x = x; rect.y = y; rect.w = w; rect.h = h;
            return rect;
        }
        
        static void Main()
        {

            Game game = new Game();
            game.Init("Mario", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, screenWidth, screenHeight,
                      SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE); 

            while (game._IsRunning)
            {
                uint lastFrameTimeInMiliseconds = SDL.SDL_GetTicks();
                game.HandleEvents();
                game.Update();
                game.Render();

                gameFrameTime = (int)(SDL.SDL_GetTicks() - lastFrameTimeInMiliseconds);

                if (desiredFrameTime > gameFrameTime) SDL.SDL_Delay((uint)(desiredFrameTime - gameFrameTime));

                if (frame >= 24 && !game.inMainMenu && Game.inGameTime > 0 && !Game._Player.isWinning &&
                    !Game._Player.isDying) 
                {
                    frame = 0;
                    Game.inGameTime -= 1;
                    if (Game.inGameTime == 100)
                    {
                        SDL_mixer.Mix_FreeChunk(Game.gameMusic);
                        warning = SDL_mixer.Mix_LoadWAV("Assets/SFX/100secondsLeft.wav");
                        SDL_mixer.Mix_Volume(-1, 20);
                        SDL_mixer.Mix_PlayChannel(-1, warning, 0);
                    }
                    if (Game.inGameTime == 93)
                    {
                        SDL_mixer.Mix_FreeChunk(warning);
                        Game.gameMusic = SDL_mixer.Mix_LoadWAV("Assets/Music/OverworldTheme1.3Faster.wav");
                        SDL_mixer.Mix_Volume(-1, 20);
                        SDL_mixer.Mix_PlayChannel(-1, warning, 0);
                    }
                }
                frame++;
            }

            game.CleanUp();
        }
    }
}
