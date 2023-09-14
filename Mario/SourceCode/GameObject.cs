using SDL2;
using System.Linq;

namespace Mario
{
    internal class GameObject
    {
        public int _positionX, _positionY, _frames, velocityX, velocityY;
        public SDL.SDL_RendererFlip flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
        public bool IsTouchingGround = true;
        bool isWinning = false, isReseting = false, isEnding = false, isDying = false,
                    hasReachedPit = false, hasLost = false;
        protected SDL.SDL_Rect surface, destination;
        protected TextureManager.TextureInfo textureInfo;
        protected SDL.SDL_Point point;

        public GameObject(string path, int positionX, int positionY, int frames)
        {
            _frames = frames;
            _positionX = positionX;
            _positionY = positionY;
            point.x = 0;
            point.y = 0;
            textureInfo = TextureManager.LoadTexture(path);
        }

        public virtual void Update() {}

        public virtual void UpdateAnimation () { }
        public void Render ()
        {
            SDL.SDL_RenderCopyEx(Game.Renderer, textureInfo.Texture, ref surface, ref destination, 0, ref point, flipFlag);
        }

        public void Clean()
        {
            SDL.SDL_DestroyTexture(textureInfo.Texture);
        }

        public TextureManager.TextureInfo GetTextureInfo()
        {
            return textureInfo;
        }
    }
}
