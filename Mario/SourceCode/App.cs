using SDL2;

namespace Mario
{
    internal class App
    {
        public const int screenWidth = 1792;
        public const int screenHeight = 1008;
        public const int FPS = 60;
        public const int desiredFrameTime = 1000 / FPS;
        public static int gameFrameTime;
        public static int frame = 0;

        public static SDL.SDL_Rect AssignValuesForRectangle(int x, int y, int w, int h)
        {
            SDL.SDL_Rect rect;
            rect.x = x;
            rect.y = y;
            rect.w = w;
            rect.h = h;
            return rect;
        }

        static void Main()
        {
            Game game = new Game();
            game.Init("Mario", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, screenWidth, screenHeight, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            while (game.IsRunning)
            {
                uint lastFrameTimeInMiliseconds = SDL.SDL_GetTicks();
                game.HandleEvents();
                game.Update();
                game.Render();

                gameFrameTime = (int)(SDL.SDL_GetTicks() - lastFrameTimeInMiliseconds);

                if (IsGeneratingTooMuchFrames())
                {
                    SDL.SDL_Delay((uint)(desiredFrameTime - gameFrameTime));
                }

                if (IsFirstFrame())
                {
                    frame = 0;
                    Game.inGameTime -= 1;
                }

                frame++;
            }

            game.CleanUp();
        }

        private static bool IsFirstFrame()
        {
            return frame >= 24 && !Game._inMainMenu && Game.inGameTime > 0 && !Game._player.isWinning &&
                                !Game._player.isDying;
        }

        static bool IsGeneratingTooMuchFrames()
        {
            return desiredFrameTime > gameFrameTime;
        }
    }
}
