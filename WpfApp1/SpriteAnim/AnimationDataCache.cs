using System.Collections.Generic;

namespace WpfApp1
{
    public struct AnimationDataCache
    {
        public AnimationData Data;
        public float DeltaFrameSeconds;
        public List<Rect> SpriteBounds;

        public AnimationDataCache(AnimationData data)
        {
            Data = data;
            SpriteBounds = new List<Rect>();

            // build cache
            DeltaFrameSeconds = 1.0f / data.FrameRatePerSecond;

            for (int i = 0; i <= data.StartFrame; i++)
            {
                Rect rect = new Rect(i * data.Width, 0, data.Width, data.Height);
                SpriteBounds.Insert(i, rect);
            }

            for (int i = data.StartFrame; i <= data.EndFrame; i++)
            {
                Rect rect = new Rect(i * data.Width, 0, data.Width, data.Height);
                SpriteBounds.Insert(i, rect);
            }
        }

        public Rect GetRect(int frame)
        {
            return SpriteBounds[frame];
        }

        public int GetNextFrame(int frame)
        {
            if (frame > Data.EndFrame)
            {
                frame = Data.StartFrame;
            }

            return frame;
        }

        public bool CanAdvanceToNextFrame(float dt)
        {
            return dt > DeltaFrameSeconds;
        }

        public bool HasNextFrameResetsAnimation(int frame)
        {
            return frame > Data.EndFrame;
        }
    }
}
