using SDL2;
using System;
using System.Runtime.InteropServices;

namespace Mario
{
    internal class TextureManager
    {

        public struct TextureInfo
        {
            public TextureInfo(IntPtr texture, int width, int height)
            {
                Texture = texture;
                Width = width;
                Height = height;

            }

            public IntPtr Texture { get; }
            public int Width { get; }
            public int Height { get; }
            
        }
        public static TextureInfo LoadTexture(string path)
        {
            IntPtr tempSurface = SDL_image.IMG_Load(path);
            SDL.SDL_Surface surf = (SDL.SDL_Surface) Marshal.PtrToStructure(tempSurface, typeof(SDL.SDL_Surface));
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(Game._Renderer, tempSurface);
            SDL.SDL_FreeSurface(tempSurface);
            return new TextureInfo(texture, surf.w, surf.h);
        }

        public static void DrawTexture(TextureInfo textureInfo, int positionX, int positionY, int frames)
        {
            SDL.SDL_Rect surface = App.AssignValuesForRectangle((textureInfo.Width / frames) * (int)((SDL.SDL_GetTicks() / 300) % frames), 0, textureInfo.Width / frames, textureInfo.Height);
            SDL.SDL_Rect destination = App.AssignValuesForRectangle(positionX, positionY, textureInfo.Width / frames, textureInfo.Height);
            SDL.SDL_RenderCopy(Game._Renderer, textureInfo.Texture, ref surface, ref destination);
        }
    }
}
