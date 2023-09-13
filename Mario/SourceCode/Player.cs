using SDL2;

namespace Mario
{
    internal class Player : GameObject
    {
        private int _offset = 0;
        private int _counter = 0;
        private int _animationFrames = 1;

        public Player(string path, int positionX, int positionY, int frames) : 
            base(path, positionX, positionY, frames) 
        { 
        }

        public override void Update()
        {
            if (Game.Controls.isPressingD)
            {
                flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
                if (_positionX < App.screenWidth / 2)
                {
                    velocityX = 5;
                }
                else
                {
                    velocityX = 5;
                    Game.ScrollSpeed = 5;
                }
            }

            if (Game.Controls.isPressingA)
            {
                flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
                velocityX = -5;
                Game.ScrollSpeed = -5;
            }

            if (Game.Controls.isPressingW && _counter < 13)
            {
                IsTouchingGround = false;
                _offset = 288;
                _animationFrames = 1;
                velocityY = -12;
                _counter++;
            }

            if (!Game.Controls.isPressingD && !Game.Controls.isPressingA && IsTouchingGround)
            {
                velocityX = 0;
                Game.ScrollSpeed = 0;
                _offset = 0;
                _animationFrames = 1;
            }

            if (!Game.Controls.isPressingW)
            {
                _counter = 0;
            }

            if (IsTouchingGround && (Game.Controls.isPressingD || Game.Controls.isPressingA))
            {
                _animationFrames = 4;
                _offset = 48;
            }

            if (IsTouchingGround && !Game.Controls.isPressingD && !Game.Controls.isPressingA)
            {
                _offset = 0;
            }

            base.Update();
            surface = App.AssignValuesForRectangle(_offset + 48 * (int)((SDL.SDL_GetTicks() / 125) % _animationFrames), 0, textureInfo.Width / _frames, textureInfo.Height);
            destination = App.AssignValuesForRectangle(_positionX - Game.CurrentLevel.cameraOffset, _positionY, textureInfo.Width / _frames, textureInfo.Height);
        }
    }
}
