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
            public float totalFrameTime;
            
            public void UpdateAnimation(float deltaTime)
            {
                AnimationDataCache data = animData[usedAnimation];
                totalFrameTime += deltaTime;

                if (data.CanAdvanceToNextFrame(totalFrameTime))
                {
                    currentFrame++;
                    totalFrameTime = 0;
                }

                if (data.HasNextFrameResetsAnimation(currentFrame))
                {
                    currentFrame = data.GetNextFrame(currentFrame);
                    totalFrameTime = 0;
                }
            }
        }

        private IDictionary<string, SharedAnimation> _animations = new Dictionary<string, SharedAnimation>();

        public override void Spawned()
        {
            base.Spawned();
            shouldTick = true;
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            UpdateAnimations(deltaTime);
        }

        public void GetRenderRect(string spriteId, out SDL_Rect renderRect)
        {
            if (_animations.ContainsKey(spriteId))
            {
                SharedAnimation animation = _animations.FirstOrDefault(s => s.Key.Equals(spriteId)).Value;
                renderRect = animation.animData[animation.usedAnimation].GetRect(animation.currentFrame);
            }
            else
            {
                renderRect = SdlRectMath.UnlimitedRect;
            }
        }

        private void UpdateAnimations(float deltaTime)
        {
            foreach (var sprite in _animations.Keys)
            {
                _animations[sprite].UpdateAnimation(deltaTime);
            }
        }

        public void AddAnimation(string sprite, AnimationType animationType, AnimationData data)
        {
            if (!_animations.ContainsKey(sprite))
            {
                CreateNewAnimationPool(sprite);
            }

            AnimationDataCache cache = new AnimationDataCache(data);
            SharedAnimation animation = _animations[sprite];

            animation.animData.Add(animationType, cache);
            _animations[sprite] = animation;
        }

        private void CreateNewAnimationPool(string sprite)
        {
            SharedAnimation animation = new SharedAnimation();
            animation.animData = new Dictionary<AnimationType, AnimationDataCache>();
            animation.currentFrame = 0;
            animation.totalFrameTime = 0;
            animation.usedAnimation = AnimationType.Undefined;

            _animations.Add(sprite, animation);
        }

        public void PlayAnim(string sprite, AnimationType animationType)
        {
            SharedAnimation anims = _animations[sprite];

            if (anims.usedAnimation != animationType)
            {
                SwitchToNewAnimation(sprite, animationType, anims);
            }
        }

        private void SwitchToNewAnimation(string sprite, AnimationType newAnimation, SharedAnimation anims)
        {
            anims.usedAnimation = newAnimation;
            anims.currentFrame = anims.animData[newAnimation].data.startFrame;
            anims.totalFrameTime = 0;
            _animations[sprite] = anims;
        }
    }
}
