using SDL2;
using System;
using System.Linq;

namespace Mario
{
    internal class Enemy : GameObject
    {
        private string type;
        private int offset = 0, animationFrames = 2;
        private float downPartOfSprite = 0.36f;

        public Enemy(string path, int positionX, int positionY, int frames, string Type) : base(path, positionX, positionY, frames) 
        {
            velocityX = -2;
            type = Type;
            _frames = frames;
            if (type == "koopa") downPartOfSprite = 0.9f;
        }

        public override void Update()
        {
            if (_positionX < 0) 
            {
                velocityY = 0;
                if (_positionX > -48)
                {
                    velocityX = -2;
                    _positionX += velocityX;
                }
                return;
            }

            if (_positionY > App.screenHeight - 90) 
            {
                if (_positionY > -96)
                {
                    velocityY += 1;
                    _positionY += velocityY;
                }
                else
                {
                    velocityY = 0;
                }
                return;
            }

            velocityY += 1;

           if (_positionX < Game._CurrentLevel.cameraOffset + App.screenWidth) _positionX += velocityX;

            int oldPositionY = _positionY;
            _positionY += velocityY;

            if (velocityX <= 0)
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)Math.Round((float)oldPositionY / 48 + downPartOfSprite), _positionX / 48].value))
                {
                    _positionX -= velocityX;
                    velocityX *= -1;
                }
            }
            else 
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)Math.Round((float)oldPositionY / 48 + downPartOfSprite), _positionX / 48 + 1].value))
                {
                    _positionX -= velocityX;
                    velocityX *= -1;
                }
            }
            if (velocityY <= 0)
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[_positionY / 48, _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[_positionY / 48, _positionX / 48 + 1].value))
                {
                    _positionY -= velocityY;
                    velocityY = 0;
                }
            }
            else
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)Math.Round((float)_positionY / 48 + downPartOfSprite), _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)Math.Round((float)_positionY / 48 + downPartOfSprite), _positionX / 48 + 1].value))
                {
                    _positionY -= velocityY;
                    velocityY = 0;
                }
            }    
        }

        public override void UpdateAnimation()
        {
            surface = App.AssignValuesForRectangle(offset + 48 * (int)((SDL.SDL_GetTicks() / 300) % animationFrames), 0, textureInfo.Width / _frames, textureInfo.Height);
            destination = App.AssignValuesForRectangle(_positionX - Game._CurrentLevel.cameraOffset, _positionY, textureInfo.Width / _frames, textureInfo.Height);
        }
    }
}
