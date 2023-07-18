using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace WpfApp1
{
    public class Sprite : Component, IRenderable
    {
        public bool shouldMove = false;
        public bool shouldUseSharedAnimationManager = true;
        public string spriteId;

        public float rotationAngle = 0.0f;
        public SDL_RendererFlip FlipMode = SDL_RendererFlip.SDL_FLIP_NONE;

        bool IRenderable.shouldDraw => SkeletonComponent.isPostCalibrationStage;

        private readonly IDictionary<AnimationType, AnimationDataCache> _animData = new Dictionary<AnimationType, AnimationDataCache>();
        private int _currentFrame = 0;
        private float _totalFrameTime = 0;
        private AnimationType _currentAnim = AnimationType.Undefined;
        private SharedAnimationManager _animationManager;

        public override void Spawned()
        {
            base.Spawned();
            _animationManager = Entity.rootEntity.GetComponent<SharedAnimationManager>();
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            _totalFrameTime += deltaTime;

            if (_currentAnim != AnimationType.Undefined)
            {
                UpdateAnimation();
            }
        }

        public override void ReceiveRender()
        {
            base.ReceiveRender();

            if (string.IsNullOrEmpty(spriteId))
            {
                return;
            }

            if (!shouldUseSharedAnimationManager)
            {
                RenderSelfAnimation();
            }
            else
            {
                _animationManager.GetRenderRect(spriteId, out SdlRectMath.DummyEndResult);
                SDLRendering.DrawSprite(spriteId, owner.bounds, SdlRectMath.DummyEndResult, rotationAngle, FlipMode);
            }
        }

        private void RenderSelfAnimation()
        {
            if (_currentAnim == AnimationType.Undefined)
            {
                // draw all sprite
                SDLRendering.DrawSprite(spriteId, owner.bounds, SdlRectMath.UnlimitedRect, rotationAngle, FlipMode);
            }
            else
            {
                AnimationDataCache data = _animData[_currentAnim];

                SDL_Rect rect = data.GetRect(_currentFrame);
                SDLRendering.DrawSprite(spriteId, owner.bounds, rect, 0, FlipMode);
            }
        }

        private void UpdateAnimation()
        {
            AnimationDataCache data = _animData[_currentAnim];
            if (data.CanAdvanceToNextFrame(_totalFrameTime))
            {
                _currentFrame++;
                _totalFrameTime = 0;
            }

            if (data.HasNextFrameResetsAnimation(_currentFrame))
            {
                _currentFrame = data.GetNextFrame(_currentFrame);
                _totalFrameTime = 0;
            }
        }

        public void AddAnimation(AnimationType animationType, AnimationData data)
        {
            AnimationDataCache cache = new AnimationDataCache(data);
            _animData.Add(animationType, cache);
        }

        public void PlayAnim(AnimationType animationType)
        {
            if (_currentAnim != animationType)
            {
                _currentAnim = animationType;
                _currentFrame = _animData[animationType].data.startFrame;
                _totalFrameTime = 0;
            }
        }
    }
}
