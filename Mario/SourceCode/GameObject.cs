using SDL2;
using System.Linq;

namespace Mario
{
    internal class GameObject
    {
        public int _positionX, _positionY, _frames, velocityX, velocityY;
        public SDL.SDL_RendererFlip flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
        public bool onGround = true, isWinning = false, isReseting = false, isEnding = false;
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

        public virtual void Update() 
        {
            velocityY += 1;

            _positionX += velocityX;

            int oldPositionY = _positionY;
            _positionY += velocityY;

            if (velocityX <= 0)
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48]) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48]))
                {
                    _positionX -= velocityX;
                    velocityX = 0;
                }
            }
            else
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1]) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1]))
                {
                    _positionX -= velocityX;
                    velocityX = 0;
                }
            }

            if (velocityY <= 0)
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[_positionY / 48, _positionX / 48]) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[_positionY / 48, _positionX / 48 + 1]))
                {
                    _positionY -= velocityY;
                    velocityY = 0;
                }
            }
            else
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48]) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48 + 1]))
                {
                    onGround = true;
                    _positionY -= velocityY;
                    velocityY = 0;
                }
            }
        }

        public virtual void UpdateAnimation () { }
        public void Render ()
        {
            SDL.SDL_RenderCopyEx(Game._Renderer, textureInfo.Texture, ref surface, ref destination, 0, ref point, flipFlag);
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
