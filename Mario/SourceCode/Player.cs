using SDL2;
using System.Linq;

namespace Mario
{
    internal class Player : GameObject
    {
        private int offset = 0, counter = 0, animationFrames = 1;

        public Player(string path, int positionX, int positionY, int frames) : base(path, positionX, positionY, frames) {}

        public override void Update()
        {
            if (isWinning) 
            {
                if (_positionY < 816)
                {
                    velocityY = 1;
                    _positionY += velocityY;
                    animationFrames = 1;
                }
                if (_positionY >= 816 && Game._CurrentLevel.flagDescend >= 432)
                {
                    if (!isEnding)
                    {
                        isEnding = true;
                        _positionX += 46;
                        flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
                        System.IntPtr levelClear = SDL_mixer.Mix_LoadWAV("Assets/SFX/levelClear.wav");
                        SDL_mixer.Mix_Volume(-1, 20);
                        SDL_mixer.Mix_PlayChannel(-1, levelClear, 0);
                    }
                }
                if (isEnding)
                {
                    offset = 48;
                    animationFrames = 4;
                    flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
                }
                else
                {
                    return;
                }
            }

            if (Game.Controls.isPressingD && !isEnding)
            {
                if (onGround)
                {
                    flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
                }
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
            if (Game.Controls.isPressingA && !isEnding)
            {  
                if (onGround)
                {
                    flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
                }
                velocityX = -5;
                Game._ScrollSpeed = -5;
            }
            if (Game.Controls.isPressingW && counter < 13 && !isEnding)
            { 
                onGround = false;
                offset = 288;
                animationFrames = 1;
                velocityY = -12;
                if (counter == 0)
                {
                    System.IntPtr jumpSfx = SDL_mixer.Mix_LoadWAV("Assets/SFX/jumpSmallMario.wav");
                    SDL_mixer.Mix_Volume(1, 20);
                    SDL_mixer.Mix_PlayChannel(1, jumpSfx, 0);
                }
                counter++;
            }
            if (!Game.Controls.isPressingD && !Game.Controls.isPressingA && onGround && !isEnding)
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
            if (onGround && (Game.Controls.isPressingD || Game.Controls.isPressingA) && !isEnding)
            {
                animationFrames = 4;
                offset = 48; 
            }
            if (onGround && !Game.Controls.isPressingD && !Game.Controls.isPressingA && !isEnding)
            {
                offset = 0;
            }

            if (isEnding) velocityX = 5;

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
                    if (onGround)
                    {
                        offset = 0;
                        animationFrames = 1;
                    }
                    Game._ScrollSpeed = 0;
                }
            }
            else
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1]) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1]))
                {
                    if ((Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1] == 23 ||
                        Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1] == 23) &&
                        !isWinning)
                    {
                        isWinning = true;
                        _positionX += 26;
                        SDL_mixer.Mix_FreeChunk(Game.gameMusic);
                        System.IntPtr flagSlideSound = SDL_mixer.Mix_LoadWAV("Assets/SFX/flagpoleSlide.wav");
                        SDL_mixer.Mix_Volume(-1, 20);
                        SDL_mixer.Mix_PlayChannel(-1, flagSlideSound, 0);
                        animationFrames = 2;
                        offset = 384;
                    }
                    _positionX -= velocityX;
                    velocityX = 0;
                    if (onGround && !isWinning)
                    {
                        offset = 0;
                        animationFrames = 1;
                    }
                    Game._ScrollSpeed = 0;
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

            if ((isEnding && Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48] == 28 ||
                Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48] == 28) || isReseting)
            {
                isReseting = true;
                _positionX -= velocityX;
                velocityX = 0;
                offset = 0;
                animationFrames = 1;
                _positionX = 96;
                _positionY = 864;
                if (Game.inGameTime > 0)
                {
                    Game.inGameTime -= 1;
                    int newValue = int.Parse(Game.score) + 100;
                    string replaceValue = newValue.ToString();
                    while (replaceValue.Length < 6)
                    {
                        replaceValue = "0" + replaceValue;
                    }
                    Game.score = replaceValue;
                }
            }
        }

        public override void UpdateAnimation ()
        {
            surface = App.AssignValuesForRectangle(offset + 48 * (int)((SDL.SDL_GetTicks() / 125) % animationFrames), 0, textureInfo.Width / _frames, textureInfo.Height);
            destination = App.AssignValuesForRectangle(_positionX - Game._CurrentLevel.cameraOffset, _positionY, textureInfo.Width / _frames, textureInfo.Height);
        }
    }
}
