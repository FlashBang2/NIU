using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace WpfApp1
{
    public class SharedAnimationManager : Component
    {
        internal class SharedAnimation
        {
            public IDictionary<AnimationType, AnimationDataCache> animData;
            public AnimationType usedAnimation;
            public int currentFrame;
            public float lastFrameTime;
            
            public void UpdateAnimation(float dt)
            {
                AnimationDataCache data = animData[usedAnimation];
                lastFrameTime += dt;

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
        }

        private IDictionary<string, SharedAnimation> animations = new Dictionary<string, SharedAnimation>();

        public override void Spawned()
        {
            base.Spawned();
            isActive = true;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            UpdateAnimations(dt);
        }

        public void GetRenderRect(string sprite, out SDL_Rect renderRect)
        {
            if (animations.ContainsKey(sprite))
            {
                SharedAnimation animation = animations.FirstOrDefault(s => s.Key.Equals(sprite)).Value;
                renderRect = animation.animData[animation.usedAnimation].GetRect(animation.currentFrame);
            }
            else
            {
                renderRect = SdlRectMath.UnlimitedRect;
            }
        }

        private void UpdateAnimations(float dt)
        {
            foreach (var sprite in animations.Keys)
            {
                animations[sprite].UpdateAnimation(dt);
            }
        }

        public void AddAnimation(string sprite, AnimationType animationType, AnimationData data)
        {
            if (!animations.ContainsKey(sprite))
            {
                CreateNewAnimationPool(sprite);
            }

            AnimationDataCache cache = new AnimationDataCache(data);
            SharedAnimation animation = animations[sprite];

            animation.animData.Add(animationType, cache);
            animations[sprite] = animation;
        }

        private void CreateNewAnimationPool(string sprite)
        {
            SharedAnimation animation = new SharedAnimation();
            animation.animData = new Dictionary<AnimationType, AnimationDataCache>();
            animation.currentFrame = 0;
            animation.lastFrameTime = 0;
            animation.usedAnimation = AnimationType.Undefined;

            animations.Add(sprite, animation);
        }

        public void PlayAnim(string sprite, AnimationType animationType)
        {
            SharedAnimation anims = animations[sprite];

            if (anims.usedAnimation != animationType)
            {
                SwitchToNewAnimation(sprite, animationType, anims);
            }
        }

        private void SwitchToNewAnimation(string sprite, AnimationType newAnimation, SharedAnimation anims)
        {
            anims.usedAnimation = newAnimation;
            anims.currentFrame = anims.animData[newAnimation].data.startFrame;
            anims.lastFrameTime = 0;
            animations[sprite] = anims;
        }
    }
}
