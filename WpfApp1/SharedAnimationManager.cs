using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace WpfApp1
{
    public class SharedAnimationManager : Component
    {
        class SharedAnimation
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

            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            UpdateAnimations(dt);
        }

        public void GetRenderRect(string sprite, out SDL_Rect renderRect)
        {
            SharedAnimation animation = animations.FirstOrDefault(s => s.Key.Equals(sprite)).Value;
            renderRect = animation.animData[animation.usedAnimation].GetRect(animation.currentFrame);
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
                SharedAnimation a = new SharedAnimation();
                a.animData = new Dictionary<AnimationType, AnimationDataCache>();
                a.currentFrame = 0;
                a.lastFrameTime = 0;
                a.usedAnimation = AnimationType.Undefined;

                animations.Add(sprite, a);
            }

            AnimationDataCache cache = new AnimationDataCache(data);
            SharedAnimation anims = animations[sprite];

            anims.animData.Add(animationType, cache);

            animations[sprite] = anims;
        }
        public void PlayAnim(string sprite, AnimationType animationType)
        {
            SharedAnimation anims = animations[sprite];

            if (anims.usedAnimation != animationType)
            {
                anims.usedAnimation = animationType;
                anims.currentFrame = anims.animData[animationType].Data.StartFrame;
                anims.lastFrameTime = 0;
                animations[sprite] = anims;
            }

        }
    }
}
