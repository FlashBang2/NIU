namespace WpfApp1
{
    public struct AnimationData
    {
        public AnimationData(int startFrame, int endFrame, int width, int height, int frameRatePerSecond) : this()
        {
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            this.width = width;
            this.height = height;
            this.frameRatePerSecond = frameRatePerSecond;
            shouldFlip = false;
        }

        public int endFrame;
        public int startFrame;

        public int width;
        public int height;
        public int frameRatePerSecond;

        public bool shouldFlip;
    }
}
