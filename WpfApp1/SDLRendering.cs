using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;
using static SDL2.SDL_ttf;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    static public class SDLRendering
    {
        private static IntPtr _renderer;
        private static IDictionary<string, IntPtr> _fonts = new Dictionary<string, IntPtr>();
        private static IDictionary<string, IntPtr> _textures = new Dictionary<string, IntPtr>();

        private static List<IRenderable> _renderables = new List<IRenderable>();

        public static void Init(IntPtr renderer)
        {
            _renderer = renderer;
        }

        public static void Quit()
        {
            foreach (KeyValuePair<string, IntPtr> it in _fonts)
            {
                TTF_CloseFont(it.Value);
            }
            _fonts.Clear();

            foreach (KeyValuePair<string, IntPtr> it in _textures)
            {
                SDL_DestroyTexture(it.Value);
            }
            _textures.Clear();

            _renderer = IntPtr.Zero;
        }

        public static void LoadFont(string path, int size, string id)
        {
            IntPtr font = TTF_OpenFont(path, size);

            if (font.Equals(IntPtr.Zero))
            {
                throw new ApplicationException("Couldn't load font under " + path);
            }

            _fonts.Add(id, font);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontId"></param>
        /// <param name="color"></param>
        /// <returns> texture handle for rendered text </returns>
        public static IntPtr GenerateTextTexture(string text, string fontId, Color color)
        {
            SDL_Color c = CSharpColorToSDLColor(color);
            IntPtr surface = TTF_RenderText_Solid(_fonts[fontId], text, c);

            IntPtr texture = SDL_CreateTextureFromSurface(_renderer, surface);
            StringBuilder sb = new StringBuilder();

            sb.Append(text).Append('.').Append(fontId).Append(".").Append(color.ToString());

            _textures.Add(sb.ToString(), texture);

            SDL_FreeSurface(surface);

            return texture;
        }

        public static IntPtr GetTextTexture(string text, string fontId, Color color)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(text).Append('.').Append(fontId).Append(".").Append(color.ToString());

            if (_textures.Any(v => v.Key.Equals(sb.ToString())))
            {
                return _textures[sb.ToString()];
            }

            return GenerateTextTexture(text, fontId, color);
        }

        public static SDL_Color CSharpColorToSDLColor(Color color)
        {
            SDL_Color c = new SDL_Color();
            c.a = color.A;
            c.r = color.R;
            c.g = color.G;
            c.b = color.B;

            return c;
        }

        public static int DrawCircle(int x, int y, int radius, Color color)
        {
            int offsetx, offsety, d;
            int status;

            offsetx = 0;
            offsety = radius;
            d = radius - 1;
            status = 0;

            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);

            while (offsety >= offsetx)
            {
                status += SDL_RenderDrawPoint(_renderer, x + offsetx, y + offsety);
                status += SDL_RenderDrawPoint(_renderer, x + offsety, y + offsetx);
                status += SDL_RenderDrawPoint(_renderer, x - offsetx, y + offsety);
                status += SDL_RenderDrawPoint(_renderer, x - offsety, y + offsetx);
                status += SDL_RenderDrawPoint(_renderer, x + offsetx, y - offsety);
                status += SDL_RenderDrawPoint(_renderer, x + offsety, y - offsetx);
                status += SDL_RenderDrawPoint(_renderer, x - offsetx, y - offsety);
                status += SDL_RenderDrawPoint(_renderer, x - offsety, y - offsetx);

                if (status < 0)
                {
                    status = -1;
                    break;
                }

                if (d >= 2 * offsetx)
                {
                    d -= 2 * offsetx + 1;
                    offsetx += 1;
                }
                else if (d < 2 * (radius - offsety))
                {
                    d += 2 * offsety - 1;
                    offsety -= 1;
                }
                else
                {
                    d += 2 * (offsety - offsetx - 1);
                    offsety -= 1;
                    offsetx += 1;
                }
            }

            return status;
        }

        public static int FillCircle(int x, int y, int radius, Color color)
        {
            int offsetx, offsety, d;
            int status;


            offsetx = 0;
            offsety = radius;
            d = radius - 1;
            status = 0;

            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);

            while (offsety >= offsetx)
            {

                status += SDL_RenderDrawLine(_renderer, x - offsety, y + offsetx,
                                             x + offsety, y + offsetx);
                status += SDL_RenderDrawLine(_renderer, x - offsetx, y + offsety,
                                             x + offsetx, y + offsety);
                status += SDL_RenderDrawLine(_renderer, x - offsetx, y - offsety,
                                             x + offsetx, y - offsety);
                status += SDL_RenderDrawLine(_renderer, x - offsety, y - offsetx,
                                             x + offsety, y - offsetx);

                if (status < 0)
                {
                    status = -1;
                    break;
                }

                if (d >= 2 * offsetx)
                {
                    d -= 2 * offsetx + 1;
                    offsetx += 1;
                }
                else if (d < 2 * (radius - offsety))
                {
                    d += 2 * offsety - 1;
                    offsety -= 1;
                }
                else
                {
                    d += 2 * (offsety - offsetx - 1);
                    offsety -= 1;
                    offsetx += 1;
                }
            }

            return status;
        }

        public static void DrawRect(int x, int y, int w, int h, Color color)
        {
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
            SDL_Rect r = new SDL_Rect();
            r.x = x;
            r.y = y;
            r.w = w;
            r.h = h;

            SDL_RenderDrawRect(_renderer, ref r);
        }

        public static void FillRect(int x, int y, int w, int h, Color color)
        {
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
            SDL_Rect r = new SDL_Rect();
            r.x = x;
            r.y = y;
            r.w = w;
            r.h = h;

            SDL_RenderFillRect(_renderer, ref r);
        }

        public static void DrawSprite(string textureId, Rect spriteBounds, Rect sourceRect, double angle)
        {
            IntPtr texture = _textures[textureId];
            SDL_Rect spriteRect = spriteBounds.AsSDLRect;
            SDL_Rect src = sourceRect.AsSDLRect;
            SDL_Point p = new SDL_Point();
            p.x = p.y = 0;

            SDL_RenderCopyEx(_renderer, texture, ref src, ref spriteRect, angle, ref p, SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public static void DrawSprite(IntPtr texture, Rect spriteBounds, Rect sourceRect, double angle)
        {
            SDL_Rect spriteRect = spriteBounds.AsSDLRect;
            SDL_Rect src = sourceRect.AsSDLRect;
            SDL_Point p = new SDL_Point();
            p.x = p.y = 0;

            SDL_RenderCopyEx(_renderer, texture, ref src, ref spriteRect, angle, ref p, SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public static void DrawSprite(IntPtr texture, Vector center, Rect spriteBounds, Rect sourceRect, double angle)
        {
            SDL_Rect spriteRect = spriteBounds.AsSDLRect;
            SDL_Rect src = sourceRect.AsSDLRect;
            SDL_Point p = new SDL_Point();
            p.x = (int)center.X;
            p.y = (int)center.Y;

            SDL_RenderCopyEx(_renderer, texture, ref src, ref spriteRect, angle, ref p, SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public static void DrawText(string text, string fontId, double posX, double posY, Color color)
        {
            var texture = GetTextTexture(text, fontId, color);

            TTF_SizeText(_fonts[fontId], text, out int w, out int h);

            DrawSprite(texture, new Rect(posX, posY, w, h), Rect.Unlimited, 0);
        }

        public static void DrawTextOnCenterPivot(string text, string fontId, double posX, double posY, Color color)
        {
            var texture = GetTextTexture(text, fontId, color);

            TTF_SizeText(_fonts[fontId], text, out int w, out int h);

            DrawSprite(texture, Rect.FromOriginAndExtend(new Vector(posX, posY), new Vector(w, h)), Rect.Unlimited, 0);
        }

        public static Vector GetTextSize(string text, string fontId)
        {
            TTF_SizeText(_fonts[fontId], text, out int w, out int h);
            return new Vector(w, h);
        }
        public static void DrawLine(Vector start, Vector end, Color color)
        {
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
            SDL_RenderDrawLine(_renderer, (int)start.X, (int)start.Y, (int)end.X, (int)end.Y);
        }

        public static void DrawPoint(Vector point, Color color)
        {
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
            SDL_RenderDrawPoint(_renderer, (int)point.X, (int)point.Y);
        }

        public static void AddTexture(IntPtr texture, string id)
        {
            Debug.Assert(!texture.Equals(IntPtr.Zero));

            _textures.Add(id, texture);
        }

        public static IntPtr LoadTexture(string path, string id)
        {
            IntPtr surface = IMG_Load(path);
            if (surface.Equals(IntPtr.Zero))
            {
                throw new ApplicationException("Couldn't load texture from " + path);
            }

            IntPtr texture = SDL_CreateTextureFromSurface(_renderer, surface);
            SDL_FreeSurface(surface);
            AddTexture(texture, id);

            return texture;
        }

        public static void RenderFrame()
        {
            SDL_RenderPresent(_renderer);
        }

        public static void ClearFrame()
        {
            SDL_SetRenderDrawColor(_renderer, 255, 255, 255, 255);
            SDL_RenderClear(_renderer);
        }

        public static void RenderRenderable(IRenderable renderable)
        {
            if (renderable.ShouldDraw)
            {
                ChooseRenderMethod(renderable);
            }
        }

        private static void ChooseRenderMethod(IRenderable renderable)
        {
            switch (renderable.RenderingMode)
            {
                case RenderMode.Sprite:
                    Debug.Assert(!string.IsNullOrEmpty(renderable.SpriteTextureId));
                    DrawSprite(renderable.SpriteTextureId, renderable.Bounds, renderable.SourceTextureBounds, renderable.RotationAngle);
                    break;
                case RenderMode.Rect:
                    DrawRect((int)renderable.PosX, (int)renderable.PosY, (int)renderable.Bounds.Width, (int)renderable.Bounds.Height, renderable.ProxyShapeColor);
                    break;
                case RenderMode.Circle:
                    DrawCircle((int)renderable.PosX, (int)renderable.PosY, (int)renderable.CircleRadius, renderable.ProxyShapeColor);
                    break;
                case RenderMode.Line:
                    DrawLine(new Vector(renderable.Bounds.Left, renderable.Bounds.Top), new Vector(renderable.Bounds.Right, renderable.Bounds.Down), renderable.ProxyShapeColor);
                    break;
                case RenderMode.Point:
                    DrawPoint(new Vector(renderable.PosX, renderable.PosY), renderable.ProxyShapeColor);
                    break;
                case RenderMode.FilledRect:
                    FillRect((int)renderable.PosX, (int)renderable.PosY, (int)renderable.Bounds.Width, (int)renderable.Bounds.Height, renderable.ProxyShapeColor);
                    break;
                case RenderMode.FilledCircle:
                    FillCircle((int)renderable.PosX, (int)renderable.PosY, (int)renderable.CircleRadius, renderable.ProxyShapeColor);
                    break;
            }
        }

        public static void AddRenderable(IRenderable renderable)
        {
            Debug.Assert(renderable != null);

            _renderables.Add(renderable);
            renderable.ZIndexChanged += SortByZIndex;
            SortByZIndex(0);
        }

        public static void RemoveRenderable(IRenderable renderable)
        {
            Debug.Assert(renderable != null);

            _renderables.Remove(renderable);
            renderable.ZIndexChanged -= SortByZIndex;
            SortByZIndex(0);
        }

        private static void SortByZIndex(int index)
        {
            _renderables.Sort((a, b) => a.ZIndex - b.ZIndex);
        }
    }
}
