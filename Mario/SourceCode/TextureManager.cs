
namespace Mario
{
    internal class TextureManager
    {
        public struct TextureInfo
        {
            public TextureInfo(System.IntPtr texture, int width, int height)
            {
                Texture = texture;
                Width = width;
                Height = height;
            }

            public System.IntPtr Texture { get; }
            public int Width { get; }
            public int Height { get; }
            
        }
        public static TextureInfo LoadTexture(string path)
        {
            System.IntPtr tempSurface = SDL2.SDL_image.IMG_Load(path);
            SDL2.SDL.SDL_Surface surf = (SDL2.SDL.SDL_Surface)System.Runtime.InteropServices.Marshal.PtrToStructure(tempSurface, typeof(SDL2.SDL.SDL_Surface));
            System.IntPtr texture = SDL2.SDL.SDL_CreateTextureFromSurface(Game.Renderer, tempSurface);
            SDL2.SDL.SDL_FreeSurface(tempSurface);
            return new TextureInfo(texture, surf.w, surf.h);
        }

        public static void DrawTexture(TextureInfo textureInfo, int positionX, int positionY, int frames)
        {
            SDL2.SDL.SDL_Rect surface = App.AssignValuesForRectangle((textureInfo.Width / frames) * (int)((SDL2.SDL.SDL_GetTicks() / 300) % frames), 0, textureInfo.Width / frames, textureInfo.Height);
            SDL2.SDL.SDL_Rect destination = App.AssignValuesForRectangle(positionX, positionY, textureInfo.Width / frames, textureInfo.Height);
            SDL2.SDL.SDL_RenderCopy(Game.Renderer, textureInfo.Texture, ref surface, ref destination);
        }
    }
}
