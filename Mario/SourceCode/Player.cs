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

        public const int MarioFrameWidth = 48;

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
            if (!HandleInput())
            {
                return;
            }

            base.Update();
            UpdateTextureFrames();
            int oldPositionY = UpdatePhysics();

            if (oldPositionY == int.MinValue)
            {
                return;
            }

            if (IsNearSideDoor(oldPositionY))
            {
                UpdateScore();
            }
        }

        private int UpdatePhysics()
        {
            ApplyVelocity();

            int oldPositionY = _positionY;
            _positionY += velocityY;

            if (HasFallIntoPit())
            {
                Die();
                return int.MinValue;
            }

            if (velocityX <= 0)
            {
                if (ShouldBlockScrollingLeft())
                {
                    BlockFromScrollingAtLeftCorner();
                    return int.MinValue;
                }

                if (IsPreviousBlockImpassable(oldPositionY))
                {
                    StopMovingTowardsBlock();
                }
            }
            else
            {
                CheckForCollisionFromRightSide(oldPositionY);
            }

            if (velocityY <= 0)
            {
                CheckForHitAboveHead();
            }
            else
            {
                if (IsOnGround())
                {
                    IsTouchingGround = true;
                    _positionY -= velocityY;
                    velocityY = 0;
                }
            }

            return oldPositionY;
        }

        private void CheckForCollisionFromRightSide(int oldPositionY)
        {
            if (IsNextBlockImpassable(oldPositionY))
            {
                if (ShouldStartPoleDownSequence(oldPositionY))
                {
                    StartPostPoleDownSequence();
                }
                else
                {
                    StopMovingTowardsBlock();
                }
            }
        }

        private void CheckForHitAboveHead()
        {
            if (IsBlockOverHeadImpassable())
            {
                HitBlockAt(_positionX / 48, _positionY / 48);
            }
            if (IsBlockNearHeadImpassable())
            {
                HitBlockAt(_positionX / 48 + 1, _positionY / 48);
            }
            if (IsBlockNearHeadImpassable() || IsBlockOverHeadImpassable())
            {
                StartHitSequence();
            }
        }

        private void UpdateScore()
        {
            IsReseting = true;
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

        private bool IsNearSideDoor(int oldPositionY)
        {
            return IsEnding && Game.CurrentLevel.IsSideDoorOnTile(oldPositionY / 48, _positionX / 48) ||
                            Game.CurrentLevel.IsSideDoorOnTile(
                                (int)Math.Round((float)oldPositionY / 48 + 0.36f), _positionX / 48
                            ) || IsReseting;
        }

        private bool IsOnGround()
        {
            return Game.immpasableBlocks.Contains(
                Game.CurrentLevel.Tiles[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48].Value) ||
                Game.immpasableBlocks.Contains(
                    Game.CurrentLevel.Tiles[(int)System.Math.Round((float)_positionY / 48 + 0.36f), _positionX / 48 + 1].Value);
        }

        private void StartHitSequence()
        {
            _positionY -= velocityY;
            velocityY = 0;
            System.IntPtr bumpSFX = SDL_mixer.Mix_LoadWAV("Assets/SFX/bump.wav");
            SDL_mixer.Mix_Volume(2, 60);
            SDL_mixer.Mix_PlayChannel(2, bumpSFX, 0);
        }

        private static void HitBlockAt(int blockX, int blockY)
        {
            Game.CurrentLevel.Tiles[blockY, blockX].WasHit = true;
            Game.CurrentLevel.BumpAnimation = 24;
        }

        private bool IsBlockNearHeadImpassable()
        {
            return Game.immpasableBlocks.Contains(Game.CurrentLevel.Tiles[_positionY / 48, _positionX / 48 + 1].Value);
        }

        private bool IsBlockOverHeadImpassable()
        {
            return Game.immpasableBlocks.Contains(Game.CurrentLevel.Tiles[_positionY / 48, _positionX / 48].Value);
        }

        private bool ShouldStartPoleDownSequence(int oldPositionY)
        {
            int tileY = (int)System.Math.Round((float)oldPositionY / 48 + 0.36f);
            int tileX = _positionX / 48 + 1;

            return (Game.CurrentLevel.Tiles[oldPositionY / 48, tileX].IsFlagPolePlacedHere() ||
                                    Game.CurrentLevel.Tiles[tileY, tileX].IsFlagPolePlacedHere()) &&
                                    !IsWinning;
        }

        private bool IsNextBlockImpassable(int oldPositionY)
        {
            return Game.immpasableBlocks.Contains(Game.CurrentLevel.Tiles[oldPositionY / 48, _positionX / 48 + 1].Value) ||
                                Game.immpasableBlocks.Contains(
                                    Game.CurrentLevel.Tiles[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f),
                                    _positionX / 48 + 1].Value);
        }

        private bool IsPreviousBlockImpassable(int oldPositionY)
        {
            return Game.immpasableBlocks.Contains(Game.CurrentLevel.Tiles[oldPositionY / 48, _positionX / 48].Value) ||
                                Game.immpasableBlocks.Contains(
                                    Game.CurrentLevel.Tiles[(int)System.Math.Round((float)oldPositionY / 48 + 0.36f),
                                    _positionX / 48].Value);
        }

        private void StartPostPoleDownSequence()
        {
            IsWinning = true;
            _positionX += 26;
            SDL_mixer.Mix_FreeChunk(Game.gameMusic);
            System.IntPtr flagSlideSound = SDL_mixer.Mix_LoadWAV("Assets/SFX/flagpoleSlide.wav");
            SDL_mixer.Mix_Volume(-1, 20);
            SDL_mixer.Mix_PlayChannel(-1, flagSlideSound, 0);
            _animationFrames = 2;
            _offset = 384;
        }

        private void StopMovingTowardsBlock()
        {
            _positionX -= velocityX;
            velocityX = 0;
            if (IsTouchingGround && !IsWinning)
            {
                _offset = 0;
                _animationFrames = 1;
            }
            Game.ScrollSpeed = 0;
        }

        private bool ShouldBlockScrollingLeft()
        {
            return _positionX < 0;
        }

        private void BlockFromScrollingAtLeftCorner()
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

        private void Die()
        {
            IsDying = true;
            IsTouchingGround = false;
            velocityX = 0;
            Game.ScrollSpeed = 0;
        }

        private bool HasFallIntoPit()
        {
            return _positionY + 48 >= App.ScreenHeight;
        }

        private void ApplyVelocity()
        {
            if (IsEnding)
            {
                velocityX = 5;
            }

            velocityY += 1;

            _positionX += velocityX;
        }

        private void UpdateTextureFrames()
        {
            _surface = App.AssignValuesForRectangle(_offset + 48 * (int)((SDL.SDL_GetTicks() / 125) % _animationFrames),
                0, _textureInfo.Width / _frames, _textureInfo.Height);

            _destination = App.AssignValuesForRectangle(_positionX - Game.CurrentLevel.CameraOffset, _positionY,
                _textureInfo.Width / _frames, _textureInfo.Height);
        }

        private bool HandleInput()
        {
            if (Kinnect.IsKinnectAvailable)
            {
                HandleKinnectMovement();
            }

            if (IsDying)
            {
                if (ShouldMarioFallDownAndDie())
                {
                    FallDown();
                }
                if (!HasReachedPit)
                {
                    StartMarioDeadAnimation();
                }

                return false;
            }
            if (IsWinning)
            {
                if (ShouldMarioClimbUp())
                {
                    velocityY = 1;
                    _positionY += velocityY;
                    _animationFrames = 1;
                }
                if (HasFlagReachedEndLocation())
                {
                    if (!IsEnding)
                    {
                        StartEndingSequence();
                    }
                }
                if (IsEnding)
                {
                    _offset = 48;
                    _animationFrames = 4;
                    flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
                }
                else
                {
                    return false;
                }
            }

            if (Game.Controls.ShouldDoRightAction() && !IsEnding)
            {
                MoveMarioForward();
            }

            if (Game.Controls.ShouldDoLeftAction() && !IsEnding)
            {
                MoveMarioBackward();
            }

            if (Game.Controls.ShouldDoJumpAction() && _counter < 13 && !IsEnding)
            {
                Jump();
            }

            if (!Game.Controls.ShouldDoRightAction() && !Game.Controls.ShouldDoLeftAction() && IsTouchingGround && !Kinnect.IsKinnectAvailable)
            {
                StopImmediatelly();
            }

            if (!Game.Controls.ShouldDoJumpAction())
            {
                _counter = 0;
            }

            return true;
        }

        private void StopImmediatelly()
        {
            velocityX = 0;
            Game.ScrollSpeed = 0;
            _offset = 0;
            _animationFrames = 1;
        }

        private void Jump()
        {
            IsTouchingGround = false;
            _offset = 288;
            _animationFrames = 1;
            velocityY = -12;
            _counter++;
        }

        private void MoveMarioBackward()
        {
            flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
            velocityX = -5;
            Game.ScrollSpeed = -5;
            _animationFrames = 4;
            _offset = 48;
        }

        private void MoveMarioForward()
        {
            if (IsTouchingGround)
            {
                flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
            }
            if (_positionX < App.ScreenWidth / 2)
            {
                velocityX = 5;
            }
            else
            {
                velocityX = 5;
                Game.ScrollSpeed = 5;
            }

            _animationFrames = 4;
            _offset = 48;
        }

        private void StartEndingSequence()
        {
            IsEnding = true;
            _positionX += 46;
            flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
            IntPtr levelClear = SDL_mixer.Mix_LoadWAV("Assets/SFX/levelClear.wav");
            SDL_mixer.Mix_Volume(-1, 20);
            SDL_mixer.Mix_PlayChannel(-1, levelClear, 0);
        }

        private bool HasFlagReachedEndLocation()
        {
            return _positionY >= 816 && Game.CurrentLevel.FlagDescend >= 432;
        }

        private bool ShouldMarioClimbUp()
        {
            return _positionY < 816;
        }

        private void StartMarioDeadAnimation()
        {
            velocityY += 1;
            _positionY += velocityY;
            SDL_mixer.Mix_FreeChunk(Game.gameMusic);
            System.IntPtr playerDeathSFX = SDL_mixer.Mix_LoadWAV("Assets/SFX/deathMario.wav");
            SDL_mixer.Mix_Volume(-1, 20);
            SDL_mixer.Mix_PlayChannel(-1, playerDeathSFX, 0);
        }

        private void FallDown()
        {
            HasReachedPit = true;
            _offset = 336;
            _animationFrames = 1;
            velocityY = -6;
            _positionY += velocityY;

            if (_positionY <= App.ScreenHeight - 96)
            {
                velocityY += 1;
                _positionY += velocityY;
                HasLost = true;
            }
        }

        private bool ShouldMarioFallDownAndDie()
        {
            return (_positionY >= App.ScreenHeight || HasReachedPit) // out of map
                && !HasLost;
        }

        private void HandleKinnectMovement()
        {
            if (CanHandleKinnectMovement())
            {
                return;
            }

            ActionType actionType = FindActionType();
            bool isJumping = CheckIfJumping();

            switch (actionType)
            {
                case ActionType.MoveLeft:
                    MoveRight();
                    break;
                case ActionType.MoveRight:
                    MoveLeft();
                    break;
                default:
                    SlowDown();
                    break;
            }

            if (isJumping && IsTouchingGround)
            {
                velocityY = -GetJumpHeight();
                IsTouchingGround = false;
                _offset = 288;
                _animationFrames = 1;
            }
        }

        private bool CanHandleKinnectMovement()
        {
            return !Kinnect.IsKinnectAvailable || Game._inMainMenu || IsDying;
        }

        private void MoveLeft()
        {
            TryAccelerate(1);
            _animationFrames = 4;
            _offset = 0;
            flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
        }

        private void MoveRight()
        {
            TryAccelerate(-1);
            _animationFrames = 4;
            _offset = 0;
            flipFlag = SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
        }

        private static int GetJumpHeight()
        {
            return 23;
        }

        private void TryAccelerate(int directionX)
        {
            const int accelerationRate = 1;

            if (directionX > 0)
            {
                bool hasExceedMaxVelocity = velocityX + accelerationRate > maxVelocity;
                velocityX = hasExceedMaxVelocity ? maxVelocity : velocityX + accelerationRate;
            }
            else
            {
                bool hasExceedMaxVelocity = velocityX - accelerationRate < -maxVelocity;
                velocityX = hasExceedMaxVelocity ? -maxVelocity : velocityX - accelerationRate;
            }

            if (_positionX > App.ScreenWidth / 2)
            {
                Game.ScrollSpeed = 5 * directionX;
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
            return IsHeadUnderAnyHand();
        }

        private bool IsHeadUnderAnyHand()
        {
            Vector2 head = Kinnect.GetKinnect()[JointType.Head];
            Vector2 leftHand = Kinnect.GetKinnect()[JointType.HandLeft];
            Vector2 rightHand = Kinnect.GetKinnect()[JointType.HandRight];

            return head.Y < leftHand.Y || head.Y < rightHand.Y;
        }

        private ActionType FindActionType()
        {
            ActionType actionType = ActionType.None;

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
            // Y is directed towards down
            return Kinnect.GetKinnect()[JointType.AnkleRight].Y > Kinnect.GetKinnect()[JointType.KneeLeft].Y;
        }

        private bool ShouldTurnLeft()
        {
            // Y is directed towards down
            return Kinnect.GetKinnect()[JointType.AnkleLeft].Y > Kinnect.GetKinnect()[JointType.KneeRight].Y;
        }

        public override void UpdateAnimation()
        {
            _surface = App.AssignValuesForRectangle(_offset + 48 * (int)((SDL.SDL_GetTicks() / 125) % _animationFrames),
                0, _textureInfo.Width / _frames, _textureInfo.Height);

            _destination = App.AssignValuesForRectangle(_positionX - Game.CurrentLevel.CameraOffset,
                _positionY, _textureInfo.Width / _frames, _textureInfo.Height);
        }
    }
}
