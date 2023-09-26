using SDL2;
using Microsoft.Kinect;
using System;
using System.Linq;

namespace Mario
{
    internal class Player : GameObject
    {
        public int endTimer = 0;
        private int offset = 0, counter = 0, animationFrames = 1;
        private enum ActionType
        {
            None = 0,
            MoveLeft = 1,
            MoveRight = 2
        }

        public Player(string path, int positionX, int positionY, int frames) : base(path, positionX, positionY, frames) {}

        public override void Update()
        {
            if (Kinnect.IsKinnectAvailable) HandleKinnectMovement();

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
                    IntPtr playerDeathSFX = SDL_mixer.Mix_LoadWAV("Assets/SFX/deathMario.wav");
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
                        IntPtr levelClear = SDL_mixer.Mix_LoadWAV("Assets/SFX/levelClear.wav");
                        SDL_mixer.Mix_Volume(-1, 20);
                        SDL_mixer.Mix_PlayChannel(-1, levelClear, 0);
                        endTimer = Game.inGameTime.ToString()[Game.inGameTime.ToString().Length - 1] - '0';
                    }
                }
                if (isEnding)
                {
                    offset = 48;
                    animationFrames = 4;
                    flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
                }
                else return;
            }

            if (Game.Controls.isPressingD && !isEnding)
            {
                if (onGround) flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
                if (_positionX - Game._CurrentLevel.cameraOffset < App.screenWidth / 2) velocityX = 5;
                else
                {
                    velocityX = 5;
                    Game._ScrollSpeed = 5;
                }
            }
            if (Game.Controls.isPressingA && !isEnding)
            {  
                if (onGround) flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
                velocityX = -5;
                Game._ScrollSpeed = 0;
            }
            if (Game.Controls.isPressingW && counter < 13 && !isEnding)
            { 
                onGround = false;
                offset = 288;
                animationFrames = 1;
                velocityY = -13;
                if (counter == 0)
                {
                    IntPtr jumpSfx = SDL_mixer.Mix_LoadWAV("Assets/SFX/jumpSmallMario.wav");
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
            if (!Game.Controls.isPressingW) counter = 0;

            if (onGround && (Game.Controls.isPressingD || Game.Controls.isPressingA) && !isEnding)
            {
                animationFrames = 4;
                offset = 48; 
            }
            if (onGround && !Game.Controls.isPressingD && !Game.Controls.isPressingA && !isEnding) offset = 0;

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
            if (_positionX > Game._CurrentLevel.cameraOffset && _positionX < Game._CurrentLevel.cameraOffset + 10) 
            {
                _positionX -= velocityX;
                if (onGround)
                {
                    offset = 0;
                    animationFrames = 1;
                }
            }

            if (velocityX <= 0)
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48].value))
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
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1].value))
                {
                    if ((Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1].value == 23 ||
                        Game._CurrentLevel.data[(int)Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1].value == 23) &&
                        !isWinning)
                    {
                        isWinning = true;
                        _positionX += 26;
                        SDL_mixer.Mix_FreeChunk(Game.gameMusic);
                        IntPtr flagSlideSound = SDL_mixer.Mix_LoadWAV("Assets/SFX/flagpoleSlide.wav");
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
                    IntPtr bumpSFX = SDL_mixer.Mix_LoadWAV("Assets/SFX/bump.wav");
                    SDL_mixer.Mix_Volume(2, 60);
                    SDL_mixer.Mix_PlayChannel(2, bumpSFX, 0);  
                }
            }
            else
            {
                if (Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game._CurrentLevel.data[(int)Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48 + 1].value))
                {
                    onGround = true;
                    _positionY -= velocityY;
                    velocityY = 0;
                }
            }

            if (velocityY > 10) velocityY = 10;

            if ((isEnding && Game._CurrentLevel.data[oldPositionY / 48, _positionX / 48].value == 28 ||
                Game._CurrentLevel.data[(int)Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48].value == 28) || isReseting)
            {
                isReseting = true;
                _positionX -= velocityX;
                velocityX = 0;
                offset = 0;
                animationFrames = 1;
                _positionX = 96;
                _positionY = 864;
                if (Game.inGameTime >= 0)
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

        private void HandleKinnectMovement()
        {
            ActionType actionType = ActionType.None;
            if (Kinnect.GetKinnect()[JointType.AnkleRight].Y > Kinnect.GetKinnect()[JointType.KneeLeft].Y) actionType = ActionType.MoveRight;
            if (Kinnect.GetKinnect()[JointType.AnkleLeft].Y > Kinnect.GetKinnect()[JointType.KneeRight].Y) actionType = ActionType.MoveLeft;
            bool isJumping = Kinnect.GetKinnect()[JointType.Head].Y < Kinnect.GetKinnect()[JointType.HandLeft].Y ||
                 Kinnect.GetKinnect()[JointType.Head].Y < Kinnect.GetKinnect()[JointType.HandRight].Y;

            switch (actionType)
            {
                case ActionType.MoveLeft:
                    Game.Controls.isPressingA = true; 
                    break;
                case ActionType.MoveRight:
                    Game.Controls.isPressingD = true;
                    break;
                default:
                    Game.Controls.isPressingA = false;
                    Game.Controls.isPressingD = false;
                    break;
            }

            if (isJumping && onGround) Game.Controls.isPressingW = true;
            if (!isJumping) Game.Controls.isPressingW = false;
        }
    }
}
