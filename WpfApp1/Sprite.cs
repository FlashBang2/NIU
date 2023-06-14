using System;
using System.Collections.Generic;

namespace WpfApp1
{
    public class Sprite : Component, IRenderable
    {
        public double RotationAngle { get => 0; set => throw new NotImplementedException(); }

        bool IRenderable.ShouldDraw => SkeletonComponent.IsPostCalibrationStage;

        Rect IRenderable.SourceTextureBounds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string spriteId;

        private readonly Dictionary<AnimationType, AnimationDataCache> _animData = new Dictionary<AnimationType, AnimationDataCache>();
        private int _currentFrame = 0;
        private float _lastFrameTime = 0;
        private AnimationType _currentAnim = AnimationType.Undefined;

        public SDL2.SDL.SDL_RendererFlip FlipMode = SDL2.SDL.SDL_RendererFlip.SDL_FLIP_NONE;

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _lastFrameTime += dt;
        }

        public override void ReceiveRender()
        {
            base.ReceiveRender();

            if (_currentAnim == AnimationType.Undefined)
            {
                // draw all sprite
                SDLRendering.DrawSprite(spriteId, Owner.Bounds, Rect.Unlimited, RotationAngle, FlipMode);
            }
            else
            {
                UpdateAnimation();
            }
        }

        private void UpdateAnimation()
        {
            AnimationDataCache data = _animData[_currentAnim];

            Rect rect = data.GetRect(_currentFrame);
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
