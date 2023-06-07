namespace WpfApp1
{
    public partial class SDLApp
    {
        public struct AnimationData
        {
            public AnimationData(int startFrame, int endFrame, int width, int height, int frameRatePerSecond) : this()
            {
                StartFrame = startFrame;
                EndFrame = endFrame;
                Width = width;
                Height = height;
                FrameRatePerSecond = frameRatePerSecond;
                ShouldFlip = false;
            }

            public int EndFrame;
            public int StartFrame;

            public int Width;
            public int Height;
            public int FrameRatePerSecond;

            public bool ShouldFlip;
        }
    }
}
