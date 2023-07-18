using Newtonsoft.Json;
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

        private IDictionary<AnimationType, AnimationDataCache> _animData = new Dictionary<AnimationType, AnimationDataCache>();
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

        /*
         *  readonly IDictionary<AnimationType, AnimationDataCache> _animData = new Dictionary<AnimationType, AnimationDataCache>();
         *  int _currentFrame = 0;
         *  float _totalFrameTime = 0;
         *  AnimationType _currentAnim = AnimationType.Undefined;
         * */
        public override void OnSerialize(Dictionary<string, Tuple<int, string, bool>> keyValues)
        {
            base.OnSerialize(keyValues);

            keyValues.Add("ShouldMove", new Tuple<int, string, bool>(0, string.Empty, shouldMove));
            keyValues.Add("shouldUseSharedAnimationManager", new Tuple<int, string, bool>(0, string.Empty, shouldUseSharedAnimationManager));
            keyValues.Add("rotationAngle", new Tuple<int, string, bool>((int)rotationAngle, string.Empty, false));

            keyValues.Add("AnimationData", new Tuple<int, string, bool>(0, JsonConvert.SerializeObject(_animData), false));
            keyValues.Add("CurrentFrame", new Tuple<int, string, bool>(0, JsonConvert.SerializeObject(_currentFrame), false));
            keyValues.Add("TotalFrameTime", new Tuple<int, string, bool>(0, JsonConvert.SerializeObject(_totalFrameTime), false));

            keyValues.Add("SpriteId", new Tuple<int, string, bool>(0, spriteId, false));
            keyValues.Add("CurrentAnimation", new Tuple<int, string, bool>(0, JsonConvert.SerializeObject(_currentAnim), false));
        }

        public override void OnDeserialize(Dictionary<string, Tuple<int, string, bool>> keyValues)
        {
            base.OnDeserialize(keyValues);

            shouldMove = keyValues["ShouldMove"].Item3;
            shouldUseSharedAnimationManager = keyValues["shouldUseSharedAnimationManager"].Item3;
            rotationAngle = keyValues["rotationAngle"].Item1;

            _animData = JsonConvert.DeserializeObject<IDictionary<AnimationType, AnimationDataCache>>(keyValues["AnimationData"].Item2);
            _currentFrame = JsonConvert.DeserializeObject<int>(keyValues["CurrentFrame"].Item2);
            _totalFrameTime = JsonConvert.DeserializeObject<float>(keyValues["TotalFrameTime"].Item2);
            _currentAnim = JsonConvert.DeserializeObject<AnimationType>(keyValues["CurrentAnimation"].Item2);

            spriteId = keyValues["SpriteId"].Item2;
        }
    }
}
