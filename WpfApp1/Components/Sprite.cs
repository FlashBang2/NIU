using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace WpfApp1
{
    public class Sprite : Component, IRenderable
    {
        public bool shouldMove = false;
        public bool ShouldUseSharedAnimationManager = true;
        public string spriteId;

        public float RotationAngle = 0.0f;
        public SDL_RendererFlip FlipMode = SDL_RendererFlip.SDL_FLIP_NONE;

        bool IRenderable.shouldDraw => SkeletonComponent.isPostCalibrationStage;

        private readonly Dictionary<AnimationType, AnimationDataCache> animData = new Dictionary<AnimationType, AnimationDataCache>();
        private int currentFrame = 0;
        private float lastFrameTime = 0;
        private AnimationType currentAnim = AnimationType.Undefined;
        private SharedAnimationManager animationManager;

        public override void Spawned()
        {
            base.Spawned();
            animationManager = Entity.RootEntity.GetComponent<SharedAnimationManager>();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            lastFrameTime += dt;

            if (currentAnim != AnimationType.Undefined)
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

            if (!ShouldUseSharedAnimationManager)
            {
                RenderSelfAnimation();
            }
            else
            {
                animationManager.GetRenderRect(spriteId, out SdlRectMath.DummyEndResult);
                SDLRendering.DrawSprite(spriteId, Owner.Bounds, SdlRectMath.DummyEndResult, RotationAngle, FlipMode);
            }
        }

        private void RenderSelfAnimation()
        {
            if (currentAnim == AnimationType.Undefined)
            {
                // draw all sprite
                SDLRendering.DrawSprite(spriteId, Owner.Bounds, SdlRectMath.UnlimitedRect, RotationAngle, FlipMode);
            }
            else
            {
                AnimationDataCache data = animData[currentAnim];

                SDL_Rect rect = data.GetRect(currentFrame);
                SDLRendering.DrawSprite(spriteId, Owner.Bounds, rect, 0, FlipMode);
            }
        }

        private void UpdateAnimation()
        {
            AnimationDataCache data = animData[currentAnim];
            if (data.CanAdvanceToNextFrame(lastFrameTime))
            {
                currentFrame++;
                lastFrameTime = 0;
            }

            if (data.HasNextFrameResetsAnimation(currentFrame))
            {
                currentFrame = data.GetNextFrame(currentFrame);
                lastFrameTime = 0;
            }
        }

        public void AddAnimation(AnimationType animationType, AnimationData data)
        {
            AnimationDataCache cache = new AnimationDataCache(data);
            animData.Add(animationType, cache);
        }

        public void PlayAnim(AnimationType animationType)
        {
            if (currentAnim != animationType)
            {
                currentAnim = animationType;
                currentFrame = animData[animationType].Data.StartFrame;
                lastFrameTime = 0;
            }
        }
    }
}
