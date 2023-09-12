using SDL2;

namespace Mario
{
    internal class App
    {
        public const int screenWidth = 1792, screenHeight = 1008, FPS = 60, desiredFrameTime = 1000 / FPS;
        public static int gameFrameTime, frame = 0;
        public static SDL.SDL_Rect AssignValuesForRectangle (int x, int y, int w, int h)
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

            while (game._IsRunning)
            {
                uint lastFrameTimeInMiliseconds = SDL.SDL_GetTicks();
                game.HandleEvents();
                game.Update();
                game.Render();
                gameFrameTime = (int)(SDL.SDL_GetTicks() - lastFrameTimeInMiliseconds);

                if (desiredFrameTime > gameFrameTime)
                {
                    SDL.SDL_Delay((uint)(desiredFrameTime - gameFrameTime));
                }

                frame++;
            }

            game.CleanUp();
        }
    }
}
