using static SDL2.SDL;
using static SDL2.SDL_image;
using System.Runtime.InteropServices;
using System;

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
            IntPtr tempSurface = IMG_Load(path);
            SDL_Surface surf = (SDL_Surface)Marshal.PtrToStructure(tempSurface, typeof(SDL_Surface));
            IntPtr texture = SDL_CreateTextureFromSurface(Game.Renderer, tempSurface);
            SDL_FreeSurface(tempSurface);
            return new TextureInfo(texture, surf.w, surf.h);
        }

        public static void DrawTexture(TextureInfo textureInfo, int positionX, int positionY, int frames)
        {
            SDL_Rect surface = App.AssignValuesForRectangle((textureInfo.Width / frames) * (int)((SDL_GetTicks() / 300) % frames), 0, textureInfo.Width / frames, textureInfo.Height);
            SDL_Rect destination = App.AssignValuesForRectangle(positionX, positionY, textureInfo.Width / frames, textureInfo.Height);
            SDL_RenderCopy(Game.Renderer, textureInfo.Texture, ref surface, ref destination);
        }
    }
}
