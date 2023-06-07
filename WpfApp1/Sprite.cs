using System;
using System.Collections.Generic;

namespace WpfApp1
{
    public partial class SDLApp
    {

        public struct AnimationData
        {
            public int StartFrame;
            public int EndFrame;

            public int Width;
            public int Height;
            public int FrameRatePerSecond;

            public bool ShouldFlip;
        }

        public enum AnimationType
        {
            Undefined,
            Idle,
            Jump,
            Walk
        }


        class Sprite : Component, IRenderable
        {
            public double RotationAngle { get => 0; set => throw new NotImplementedException(); }

            bool IRenderable.ShouldDraw => SkeletonComponent.IsPostCalibrationStage;

            Rect IRenderable.SourceTextureBounds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public string spriteId = "firstPlatform";

            private Dictionary<AnimationType, AnimationData> _animData = new Dictionary<AnimationType, AnimationData>();
            private int _currentFrame = 0;
            private float _lastFrameTime = 0;
            private AnimationType _currentAnim = AnimationType.Undefined;

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
                    SDLRendering.DrawSprite(spriteId, Owner.Bounds, Rect.Unlimited, RotationAngle);
                    return;
                }

                AnimationData data = _animData[_currentAnim];
                float deltaAnimTime = 1.0f / data.FrameRatePerSecond;

                Rect r = new Rect(_currentFrame * data.Width, 0, data.Width, data.Height);

                SDLRendering.DrawSprite(spriteId, Owner.Bounds, r, 0);

                if (_lastFrameTime > deltaAnimTime)
                {
                    _currentFrame++;
                    _lastFrameTime = 0;
                }

                if (_currentFrame > data.EndFrame)
                {
                    _currentFrame = data.StartFrame;
                    _lastFrameTime = 0;
                }
            }

            public void AddAnimation(AnimationType animationType, AnimationData data)
            {
                _animData.Add(animationType, data);
            }
            public void PlayAnim(AnimationType animationType)
            {
                _currentAnim = animationType;
            }
        }
    }
}
