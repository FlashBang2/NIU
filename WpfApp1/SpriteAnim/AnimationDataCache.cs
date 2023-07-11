using System.Collections.Generic;
using System.Windows.Controls;
using static SDL2.SDL;

namespace WpfApp1
{
    public struct AnimationDataCache
    {
        public AnimationData Data;
        public float DeltaFrameSeconds;
        public List<SDL_Rect> SpriteBounds;

        public AnimationDataCache(AnimationData data)
        {
            Data = data;
            SpriteBounds = new List<SDL_Rect>();
            // build cache
            DeltaFrameSeconds = 1.0f / data.FrameRatePerSecond;

            for (int i = 0; i <= data.StartFrame; i++)
            {
                SDL_Rect rect = new SDL_Rect();
                SdlRectMath.FromXywh(i * data.Width, 0, data.Width, data.Height, out rect);
                SpriteBounds.Insert(i, rect);
            }

            for (int i = data.StartFrame; i <= data.EndFrame; i++)
            {
                SDL_Rect rect = new SDL_Rect();
                SdlRectMath.FromXywh(i * data.Width, 0, data.Width, data.Height, out rect);
                SpriteBounds.Insert(i, rect);
            }
        }

        public SDL_Rect GetRect(int frame)
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
