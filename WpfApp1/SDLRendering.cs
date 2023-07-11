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
    public static class SDLRendering
    {
        private static IntPtr _renderer;
        private static readonly IDictionary<string, IntPtr> _fonts = new Dictionary<string, IntPtr>();
        private static readonly IDictionary<string, IntPtr> _textures = new Dictionary<string, IntPtr>();

        // TODO GetWindowDimensions
        private static readonly List<IRenderable> _renderables = new List<IRenderable>();

        private static Vector _cameraCenter = new Vector();
        public static int _screenWidth = 0;
        public static int _screenHeight = 0;
        private static bool isMovingRight = false;

        public static void Init(IntPtr renderer)
        {
            _renderer = renderer;
            SDL_GetRendererOutputSize(renderer, out _screenWidth, out _screenHeight);

            IntPtr surface = SDL_CreateRGBSurface(0, 1, 1, 32, 0, 0, 0, 0xff000000);
            AddTexture(SDL_CreateTextureFromSurface(renderer, surface), "Empty");
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

            x -= (int)_cameraCenter.X;
            y -= (int)_cameraCenter.Y;

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

            x -= (int)_cameraCenter.X;
            y -= (int)_cameraCenter.Y;

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
            SDL_Rect r = new SDL_Rect
            {
                x = x - (int)_cameraCenter.X,
                y = y - (int)_cameraCenter.Y,
                w = w,
                h = h
            };

            SDL_RenderDrawRect(_renderer, ref r);
        }

        public static void FillRect(int x, int y, int w, int h, Color color)
        {
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
            SDL_Rect r = new SDL_Rect
            {
                x = x - (int)_cameraCenter.X,
                y = y - (int)_cameraCenter.Y,
                w = w,
                h = h
            };

            SDL_RenderFillRect(_renderer, ref r);
        }

        public static void DrawSprite(string textureId, SDL_Rect spriteBounds, SDL_Rect sourceRect, double angle, SDL_RendererFlip flipMode = SDL_RendererFlip.SDL_FLIP_NONE)
        {
            if (!_textures.Any(v => v.Key.Equals(textureId)))
            {
                textureId = "Empty";
                DrawRect(spriteBounds.x, spriteBounds.y, spriteBounds.w, spriteBounds.h, Color.FromRgb(255, 0, 0));
                return;
            }

            IntPtr texture = _textures[textureId];

            if (sourceRect.x == SdlRectMath.UnlimitedRect.x &&
                sourceRect.y == SdlRectMath.UnlimitedRect.y &&
                sourceRect.w == SdlRectMath.UnlimitedRect.w &&
                sourceRect.h == SdlRectMath.UnlimitedRect.h)
            {

                SDL_QueryTexture(texture, out uint Format, out int Access, out int Width, out int Height);
                sourceRect.w = Width;
                sourceRect.h = Height;
                sourceRect.x = sourceRect.y = 0;
            }

            SDL_Point p = new SDL_Point();
            p.x = p.y = 0;

            spriteBounds.x -= (int)_cameraCenter.X;
            spriteBounds.y -= (int)_cameraCenter.Y;
            SDL_RenderCopyEx(_renderer, texture, ref sourceRect, ref spriteBounds, angle, ref p, flipMode);
        }

        public static void DrawSprite(IntPtr texture, Rect spriteBounds, Rect sourceRect, double angle)
        {
            SDL_Rect spriteRect = spriteBounds.AsSDLRect;
            SDL_Rect src = sourceRect.AsSDLRect;
            SDL_Point p = new SDL_Point();
            p.x = p.y = 0;

            spriteRect.x -= (int)_cameraCenter.X;
            spriteRect.y = spriteRect.y - (int)_cameraCenter.Y;
            SDL_RenderCopyEx(_renderer, texture, ref src, ref spriteRect, angle, ref p, SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public static void DrawSprite(IntPtr texture, Vector center, Rect spriteBounds, Rect sourceRect, double angle)
        {
            SDL_Rect spriteRect = spriteBounds.AsSDLRect;
            SDL_Rect src = sourceRect.AsSDLRect;
            SDL_Point p = new SDL_Point
            {
                x = (int)center.X,
                y = (int)center.Y
            };

            spriteRect.x -= (int)_cameraCenter.X;
            spriteRect.y -= (int)_cameraCenter.Y;

            SDL_RenderCopyEx(_renderer, texture, ref src, ref spriteRect, angle, ref p, SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public static void DrawText(string text, string fontId, double posX, double posY, Color color)
        {
            IntPtr texture = GetTextTexture(text, fontId, color);

            TTF_SizeText(_fonts[fontId], text, out int w, out int h);

            DrawSprite(texture, new Rect(posX + _cameraCenter.X, posY + _cameraCenter.Y, w, h), Rect.Unlimited, 0);
        }

        public static void DrawTextOnCenterPivot(string text, string fontId, double posX, double posY, Color color)
        {
            IntPtr texture = GetTextTexture(text, fontId, color);

            TTF_SizeText(_fonts[fontId], text, out int w, out int h);

            DrawSprite(texture, Rect.FromOriginAndExtend(new Vector(posX - _cameraCenter.X, posY - _cameraCenter.Y), new Vector(w, h)), Rect.Unlimited, 0);
        }

        public static Vector GetTextSize(string text, string fontId)
        {
            TTF_SizeText(_fonts[fontId], text, out int w, out int h);
            return new Vector(w, h);
        }
        public static void DrawLine(Vector start, Vector end, Color color)
        {
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
            SDL_RenderDrawLine(_renderer, (int)start.X - (int)_cameraCenter.X, (int)start.Y - (int)_cameraCenter.Y, (int)end.X - (int)_cameraCenter.X, (int)end.Y - (int)_cameraCenter.Y);
        }

        public static void DrawPoint(Vector point, Color color)
        {
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
            SDL_RenderDrawPoint(_renderer, (int)point.X - (int)_cameraCenter.X, (int)point.Y - (int)_cameraCenter.Y);
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
                Console.Error.WriteLine("Couldn't load texture from " + path);
                Object[] frames = new StackTrace(true).GetFrames();
                Console.WriteLine("Drawing empty sprite \n" + string.Join("\n", frames));
                return IntPtr.Zero;
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
            SDL_SetRenderDrawColor(_renderer, 142, 140, 237, 255);
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
        }

        public static void AddRenderable(IRenderable renderable)
        {
            Debug.Assert(renderable != null);

            _renderables.Add(renderable);
        }

        public static void RemoveRenderable(IRenderable renderable)
        {
            Debug.Assert(renderable != null);

            _renderables.Remove(renderable);
        }

        public static void SetCameraFollow(IEntity entity)
        {
            SDL_Rect rect = new SDL_Rect();
            SDL_RenderGetViewport(_renderer, out rect);
            _cameraCenter.X = entity.PosX + entity.Width / 2 - _screenWidth / 2 - 30;
            _cameraCenter.Y = entity.PosY + entity.Height / 2 - 100 - _screenHeight / 2;

            if (_cameraCenter.X < 0)
            {
                _cameraCenter.X = 0;
            }

            if (_cameraCenter.Y < 0)
            {
                _cameraCenter.Y = 0;
            }

            if (_cameraCenter.X > 2 * worldWidth - rect.w)
            {
                _cameraCenter.X = 2 * worldWidth - rect.w;
            }
            if (_cameraCenter.Y > 2 * worldHeight - rect.h)
            {
                _cameraCenter.Y = 2 * worldHeight - rect.h;
            }

            _cameraCenter.Y = 0;
        }

        private static int worldWidth;
        private static int worldHeight;

        public static void SetWorldBounds(int w, int h)
        {
            worldWidth = w;
            worldHeight = h;
        }
    }
}
