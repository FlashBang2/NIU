using System.Collections.Generic;
using static SDL2.SDL;

namespace WpfApp1
{
    public struct AnimationDataCache
    {
        public AnimationData data;
        public float deltaFrameSeconds;
        public List<SDL_Rect> spriteBounds;

        public AnimationDataCache(AnimationData data)
        {
            this.data = data;
            spriteBounds = new List<SDL_Rect>();
            // build cache
            deltaFrameSeconds = 1.0f / data.frameRatePerSecond;

            for (int i = 0; i <= data.startFrame; i++)
            {
                SDL_Rect rect = new SDL_Rect();
                SdlRectMath.FromXywh(i * data.width, 0, data.width, data.height, out rect);
                spriteBounds.Insert(i, rect);
            }

            for (int i = data.startFrame; i <= data.endFrame; i++)
            {
                SDL_Rect rect = new SDL_Rect();
                SdlRectMath.FromXywh(i * data.width, 0, data.width, data.height, out rect);
                spriteBounds.Insert(i, rect);
            }
        }

        public SDL_Rect GetRect(int frameNo)
        {
            return spriteBounds[frameNo];
        }

        public int GetNextFrame(int frameNo)
        {
            if (frameNo > data.endFrame)
            {
                frameNo = data.startFrame;
            }

            return frameNo;
        }

        public bool CanAdvanceToNextFrame(float totalFrameTime)
        {
            return totalFrameTime > deltaFrameSeconds;
        }

        public bool HasNextFrameResetsAnimation(int frame)
        {
            return frame > data.endFrame;
        }
    }
}
