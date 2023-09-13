using SDL2;

namespace Mario
{
    internal class Player : GameObject
    {
        private int offset = 0, counter = 0, animationFrames = 1;
        public Player(string path, int positionX, int positionY, int frames) : base(path, positionX, positionY, frames) {}

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
                    Game._ScrollSpeed = 5;
                }
            }
            if (Game.Controls.isPressingA)
            {
                flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
                velocityX = -5;
                Game._ScrollSpeed = -5;
            }
            if (Game.Controls.isPressingW && counter < 13)
            {
                if (onGround == true) {
                    System.Media.SoundPlayer smallJump = new System.Media.SoundPlayer(@"Assets\Sounds\jump.wav");
                    smallJump.Play();
                }
                onGround = false;
                offset = 288;
                animationFrames = 1;
                velocityY = -12;
                counter++;
            }
            if (!Game.Controls.isPressingD && !Game.Controls.isPressingA && onGround)
            {
                velocityX = 0;
                Game._ScrollSpeed = 0;
                offset = 0;
                animationFrames = 1;
            }
            if (!Game.Controls.isPressingW)
            {
                counter = 0;
            }
            if (onGround && (Game.Controls.isPressingD || Game.Controls.isPressingA))
            {
                animationFrames = 4;
                offset = 48;
            }
            if (onGround && !Game.Controls.isPressingD && !Game.Controls.isPressingA)
            {
                offset = 0;
            }
            base.Update();
            surface = App.AssignValuesForRectangle(offset + 48 * (int)((SDL.SDL_GetTicks() / 125) % animationFrames), 0, textureInfo.Width / _frames, textureInfo.Height);
            destination = App.AssignValuesForRectangle(_positionX - Game._CurrentLevel.cameraOffset, _positionY, textureInfo.Width / _frames, textureInfo.Height);
        }
    }
}
