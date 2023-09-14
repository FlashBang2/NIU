using SDL2;
using System.Linq;
using static Mario.TextureManager;

namespace Mario
{
    internal class Enemy : GameObject
    {
        private string type;

        public Enemy(string path, int positionX, int positionY, int frames, string Type) : base(path, positionX, positionY, frames) 
        {
            velocityX = 2;
            type = Type;
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

            if (_positionY > App.screenHeight - 48)
            {
                return;
            }

            velocityY += 1;

            _positionX += velocityX;

            int oldPositionY = _positionY;
            _positionY += velocityY;

            if (velocityX <= 0)
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48].value))
                {
                    _positionX -= velocityX;
                    velocityX *= -1;
                }
            }
            else 
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1].value))
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
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48 + 1].value))
                {
                    _positionY -= velocityY;
                    velocityY = 0;
                }
            }    
        }

        public override void UpdateAnimation()
        {
            surface = App.AssignValuesForRectangle((textureInfo.Width / _frames) * (int)((SDL.SDL_GetTicks() / 300) % _frames), 0, textureInfo.Width / _frames, textureInfo.Height);
            destination = App.AssignValuesForRectangle(_positionX - Game._CurrentLevel.cameraOffset, _positionY, textureInfo.Width / _frames, textureInfo.Height);
        }
    }
}
