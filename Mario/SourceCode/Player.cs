using SDL2;
using System.Linq;

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
            if (isDying)
            {
                if ((_positionY >= App.screenHeight || hasReachedPit) && !hasLost)
                {
                    hasReachedPit = true;
                    offset = 336;
                    animationFrames = 1;
                    velocityY = -6;
                    _positionY += velocityY;
                    if (_positionY <= App.screenHeight - 96)
                    {
                        velocityY += 1;
                        _positionY += velocityY;
                        hasLost = true;
                    }
                }
                if (!hasReachedPit)
                {
                    velocityY += 1;
                    _positionY += velocityY;
                    SDL_mixer.Mix_FreeChunk(Game.gameMusic);
                    System.IntPtr playerDeathSFX = SDL_mixer.Mix_LoadWAV("Assets/SFX/deathMario.wav");
                    SDL_mixer.Mix_Volume(-1, 20);
                    SDL_mixer.Mix_PlayChannel(-1, playerDeathSFX, 0);
                }
                return;
            }
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
                    Game.ScrollSpeed = 5;
                }
            }

            if (Game.Controls.isPressingA && !isEnding)
            {
                flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
                velocityX = -5;
                Game.ScrollSpeed = -5;
            }

            if (Game.Controls.isPressingW && counter < 13 && !isEnding)
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

            if (IsTouchingGround && !Game.Controls.isPressingD && !Game.Controls.isPressingA)
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
            if (isEnding) velocityX = 5;

            velocityY += 1;

            _positionX += velocityX;

            int oldPositionY = _positionY;
            _positionY += velocityY;

            if (_positionY + 48 >= App.screenHeight)
            {
                isDying = true;
                velocityX = 0;
                Game._ScrollSpeed = 0;
                return;
            }

            if (velocityX <= 0)
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48].value))
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
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1].value))
                {
                    if ((Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1].value == 23 ||
                        Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1].value == 23) &&
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
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[_positionY / 48, _positionX / 48].value))
                {
                    Game._CurrentLevel.data[_positionY / 48, _positionX / 48].isBumped = true;
                    Game._CurrentLevel.bumpAnimation = 24;
                }
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[_positionY / 48, _positionX / 48 + 1].value))
                {
                    Game._CurrentLevel.data[_positionY / 48, _positionX / 48 + 1].isBumped = true;
                    Game._CurrentLevel.bumpAnimation = 24;
                }
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[_positionY / 48, _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[_positionY / 48, _positionX / 48 + 1].value))
                {
                    _positionY -= velocityY;
                    velocityY = 0;
                    System.IntPtr bumpSFX = SDL_mixer.Mix_LoadWAV("Assets/SFX/bump.wav");
                    SDL_mixer.Mix_Volume(2, 60);
                    SDL_mixer.Mix_PlayChannel(2, bumpSFX, 0);
                }
            }
            else
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48 + 1].value))
                {
                    onGround = true;
                    _positionY -= velocityY;
                    velocityY = 0;
                }
            }

            if ((isEnding && Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48].value == 28 ||
                Game._CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48].value == 28) || isReseting)
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

        public override void UpdateAnimation()
        {
            surface = App.AssignValuesForRectangle(offset + 48 * (int)((SDL.SDL_GetTicks() / 125) % animationFrames), 0, textureInfo.Width / _frames, textureInfo.Height);
            destination = App.AssignValuesForRectangle(_positionX - Game._CurrentLevel.cameraOffset, _positionY, textureInfo.Width / _frames, textureInfo.Height);
        }
    }
}
