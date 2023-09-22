namespace Mario
{
    internal class App
    {
        public const int ScreenWidth = 1792;
        public const int ScreenHeight = 1008;
        public const int Fps = 60;
        public const int DesiredFrameTime = 1000 / Fps;
        public static int GameFrameTime;
        public static int Frame = 0;

        public static SDL2.SDL.SDL_Rect AssignValuesForRectangle(int x, int y, int w, int h)
        {
            SDL2.SDL.SDL_Rect rect;
            rect.x = x;
            rect.y = y;
            rect.w = w;
            rect.h = h;
            return rect;
        }

        static void Main()
        {
            Game game = new Game();
            game.Init("Mario", SDL2.SDL.SDL_WINDOWPOS_CENTERED, SDL2.SDL.SDL_WINDOWPOS_CENTERED, ScreenWidth, ScreenHeight, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            while (game.IsRunning)
            {
                uint lastFrameTimeInMiliseconds = SDL2.SDL.SDL_GetTicks();
                game.HandleEvents();
                game.Update();
                game.Render();

                GameFrameTime = (int)(SDL2.SDL.SDL_GetTicks() - lastFrameTimeInMiliseconds);

                if (IsGeneratingTooMuchFrames())
                {
                    SDL2.SDL.SDL_Delay((uint)(DesiredFrameTime - GameFrameTime));
                }

                if (IsFirstFrame())
                {
                    Frame = 0;
                    Game.inGameTime -= 1;
                }

                Frame++;
            }

            game.CleanUp();
        }

        private static bool IsFirstFrame()
        {
            return Frame >= 24 && !Game._inMainMenu && Game.inGameTime > 0 && !Game._player.IsWinning &&
                                !Game._player.IsDying;
        }

        static bool IsGeneratingTooMuchFrames()
        {
            return DesiredFrameTime > GameFrameTime;
        }
    }
}
