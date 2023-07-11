using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace WpfApp1
{
    public class Sprite : Component, IRenderable
    {
        public double RotationAngle { get => 0; set => throw new NotImplementedException(); }

        public bool shouldMove = false;
        bool IRenderable.ShouldDraw => SkeletonComponent.IsPostCalibrationStage;

        Rect IRenderable.SourceTextureBounds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string spriteId;

        private readonly Dictionary<AnimationType, AnimationDataCache> _animData = new Dictionary<AnimationType, AnimationDataCache>();
        private int _currentFrame = 0;
        private float _lastFrameTime = 0;
        private AnimationType _currentAnim = AnimationType.Undefined;

        public SDL_RendererFlip FlipMode = SDL2.SDL.SDL_RendererFlip.SDL_FLIP_NONE;

        public bool ShouldUseSharedAnimationManager = false;
        SharedAnimationManager animationManager;

        public override void Spawned()
        {
            base.Spawned();
            animationManager = Entity.RootEntity.GetComponent<SharedAnimationManager>();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _lastFrameTime += dt;
        }

        public override void ReceiveRender()
        {
            base.ReceiveRender();

            if (string.IsNullOrEmpty(spriteId))
            {
                return;
            }

            if (!ShouldUseSharedAnimationManager)
            {
                if (_currentAnim == AnimationType.Undefined)
                {
                    // draw all sprite
                    SDLRendering.DrawSprite(spriteId, Owner.Bounds, SdlRectMath.UnlimitedRect, RotationAngle, FlipMode);
                }
                else
                {
                    UpdateAnimation();
                }
            }
            else
            {
                animationManager.GetRenderRect(spriteId, out SdlRectMath.DummyEndResult);
                SDLRendering.DrawSprite(spriteId, Owner.Bounds, SdlRectMath.DummyEndResult, RotationAngle, FlipMode);
            }
        }

        private void UpdateAnimation()
        {
            AnimationDataCache data = _animData[_currentAnim];

            SDL_Rect rect = data.GetRect(_currentFrame);
            SDLRendering.DrawSprite(spriteId, Owner.Bounds, rect, 0, FlipMode);

            if (data.CanAdvanceToNextFrame(_lastFrameTime))
            {
                _currentFrame++;
                _lastFrameTime = 0;
            }

            if (data.HasNextFrameResetsAnimation(_currentFrame))
            {
                _currentFrame = data.GetNextFrame(_currentFrame);
                _lastFrameTime = 0;
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
                _currentFrame = _animData[animationType].Data.StartFrame;
                _lastFrameTime = 0;
            }
        }
    }
}
