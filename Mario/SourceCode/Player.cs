using SDL2;
using System.Linq;
using Mario.SourceCode;
using Microsoft.Kinect;
using System;

namespace Mario
{
    internal class Player : GameObject
    {
        private int _offset = 0;
        private int _counter = 0;
        private int _animationFrames = 1;
        public const int maxVelocity = 5;

        public enum ActionType
        {
            None = 0,
            MoveLeft = 1,
            RunLeft,
            MoveRight = 2,
            RunRight
        }

        public Player(string path, int positionX, int positionY, int frames) :
            base(path, positionX, positionY, frames)
        {
        }

        public override void Update()
        {
            if (Kinnect._isKinnectAvailable)
            {
                HandleKinnectMovement();
            }

            if (isDying)
            {
                if ((_positionY >= App.screenHeight || hasReachedPit) && !hasLost)
                {
                    hasReachedPit = true;
                    _offset = 336;
                    _animationFrames = 1;
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
                    _animationFrames = 1;
                }
                if (_positionY >= 816 && Game.CurrentLevel.flagDescend >= 432)
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
                    _offset = 48;
                    _animationFrames = 4;
                    flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
                }
                else
                {
                    return;
                }
            }

            if (Game.Controls.isPressingD && !isEnding)
            {
                if (IsTouchingGround)
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

            if (Game.Controls.isPressingW && _counter < 13 && !isEnding)
            {
                IsTouchingGround = false;
                _offset = 288;
                _animationFrames = 1;
                velocityY = -12;
                _counter++;
            }

            if (!Game.Controls.isPressingD && !Game.Controls.isPressingA && IsTouchingGround && !Kinnect._isKinnectAvailable)
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
                Game.ScrollSpeed = 0;
                return;
            }

            if (velocityX <= 0)
            {
                if (_positionX < 0)
                {
                    _positionX -= velocityX;
                    velocityX = 0;
                    if (IsTouchingGround)
                    {
                        _offset = 0;
                        _animationFrames = 1;
                    }
                    Game.ScrollSpeed = 0;
                    return;
                }

                if (Game.immpasableBlocks.Contains(Game.CurrentLevel.data[oldPositionY / 48, _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game.CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48].value))
                {
                    _positionX -= velocityX;
                    velocityX = 0;
                    if (IsTouchingGround)
                    {
                        _offset = 0;
                        _animationFrames = 1;
                    }
                    Game.ScrollSpeed = 0;
                }
            }
            else
            {
                if (Game.immpasableBlocks.Contains(Game.CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1].value) ||
                    Game.immpasableBlocks.Contains(Game.CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1].value))
                {
                    if ((Game.CurrentLevel.data[oldPositionY / 48, _positionX / 48 + 1].value == 23 ||
                        Game.CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48 + 1].value == 23) &&
                        !isWinning)
                    {
                        isWinning = true;
                        _positionX += 26;
                        SDL_mixer.Mix_FreeChunk(Game.gameMusic);
                        System.IntPtr flagSlideSound = SDL_mixer.Mix_LoadWAV("Assets/SFX/flagpoleSlide.wav");
                        SDL_mixer.Mix_Volume(-1, 20);
                        SDL_mixer.Mix_PlayChannel(-1, flagSlideSound, 0);
                        _animationFrames = 2;
                        _offset = 384;
                    }
                    _positionX -= velocityX;
                    velocityX = 0;
                    if (IsTouchingGround && !isWinning)
                    {
                        _offset = 0;
                        _animationFrames = 1;
                    }
                    Game.ScrollSpeed = 0;
                }
            }

            if (velocityY <= 0)
            {
                if (Game.immpasableBlocks.Contains(Game.CurrentLevel.data[_positionY / 48, _positionX / 48].value))
                {
                    Game.CurrentLevel.data[_positionY / 48, _positionX / 48].isBumped = true;
                    Game.CurrentLevel.bumpAnimation = 24;
                }
                if (Game.immpasableBlocks.Contains(Game.CurrentLevel.data[_positionY / 48, _positionX / 48 + 1].value))
                {
                    Game.CurrentLevel.data[_positionY / 48, _positionX / 48 + 1].isBumped = true;
                    Game.CurrentLevel.bumpAnimation = 24;
                }
                if (Game.immpasableBlocks.Contains(Game.CurrentLevel.data[_positionY / 48, _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game.CurrentLevel.data[_positionY / 48, _positionX / 48 + 1].value))
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
                if (Game.immpasableBlocks.Contains(Game.CurrentLevel.data[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48].value) ||
                    Game.immpasableBlocks.Contains(Game.CurrentLevel.data[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48 + 1].value))
                {
                    IsTouchingGround = true;
                    _positionY -= velocityY;
                    velocityY = 0;
                }
            }

            if ((isEnding && Game.CurrentLevel.data[oldPositionY / 48, _positionX / 48].value == 28 ||
                Game.CurrentLevel.data[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48].value == 28) || isReseting)
            {
                isReseting = true;
                _positionX -= velocityX;
                velocityX = 0;
                _offset = 0;
                _animationFrames = 1;
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

        private void HandleKinnectMovement()
        {
            if (!Kinnect._isKinnectAvailable || Game._inMainMenu)
            {
                return;
            }

            ActionType actionType = FindActionType();
            bool isJumping = CheckIfJumping();

            switch (actionType)
            {
                case ActionType.MoveLeft:
                    TryAccelerate(-1);
                    _animationFrames = 4;
                    _offset = 48;
                    break;
                case ActionType.MoveRight:
                    TryAccelerate(1);
                    _animationFrames = 4;
                    _offset = 48;
                    break;
                default:
                    SlowDown();
                    break;
            }

            if (isJumping && IsTouchingGround)
            {
                velocityY = (int)(-GetJumpHeight());
                IsTouchingGround = false;
                _offset = 288;
                _animationFrames = 1;
            }
        }

        private static int GetJumpHeight()
        {
            return 25;
        }

        private void TryAccelerate(int direction)
        {
            const int accelerationRate = 1;

            if (direction > 0)
            {
                bool hasExceedMaxVelocity = velocityX + accelerationRate > maxVelocity;
                velocityX = hasExceedMaxVelocity ? maxVelocity : velocityX + accelerationRate;
            }
            else
            {
                bool hasExceedMaxVelocity = velocityX - accelerationRate < -maxVelocity;
                velocityX = hasExceedMaxVelocity ? -maxVelocity : velocityX - accelerationRate;
            }

            if (_positionX > App.screenWidth / 2)
            {
                Game.ScrollSpeed = 5 * direction;
            }
        }
        private void SlowDown()
        {
            velocityX = 0;
            Game.ScrollSpeed = 0;
            _offset = 0;
            _animationFrames = 1;
        }

        private bool CheckIfJumping()
        {
            bool isJumping = false;

            var head = Kinnect.GetKinnect()[JointType.Head];
            var leftHand = Kinnect.GetKinnect()[JointType.HandLeft];
            var rightHand = Kinnect.GetKinnect()[JointType.HandRight];

            if (head.Item2 < leftHand.Item2 || head.Item2 < rightHand.Item2)
            {
                isJumping = true;
            }
            else
            {
                isJumping = false;
            }

            return isJumping;
        }

        private ActionType FindActionType()
        {
            ActionType actionType = ActionType.None;

            // ankle left nearly equal kneeRight -> accelerate
            // ankle right nearly equal kneeLeft -> accelerate
            var ankleLeft = Kinnect.GetKinnect()[JointType.AnkleLeft];
            var kneeRight = Kinnect.GetKinnect()[JointType.KneeRight];
            var ankleRight = Kinnect.GetKinnect()[JointType.AnkleRight];
            var kneeLeft = Kinnect.GetKinnect()[JointType.KneeLeft];

            if (ShouldTurnRight())
            {
                actionType = ActionType.MoveRight;
            }
            else if (ShouldTurnLeft())
            {
                actionType = ActionType.MoveLeft;
            }

            return actionType;
        }

        private bool ShouldTurnRight()
        {
            return Kinnect.GetKinnect()[JointType.AnkleRight].Item2 > Kinnect.GetKinnect()[JointType.KneeLeft].Item2;
        }

        private bool ShouldTurnLeft()
        {
            return Kinnect.GetKinnect()[JointType.AnkleLeft].Item2 > Kinnect.GetKinnect()[JointType.KneeRight].Item2;
        }

        public override void UpdateAnimation()
        {
            surface = App.AssignValuesForRectangle(_offset + 48 * (int)((SDL.SDL_GetTicks() / 125) % _animationFrames), 0, textureInfo.Width / _frames, textureInfo.Height);
            destination = App.AssignValuesForRectangle(_positionX - Game.CurrentLevel.cameraOffset, _positionY, textureInfo.Width / _frames, textureInfo.Height);
        }
    }
}
