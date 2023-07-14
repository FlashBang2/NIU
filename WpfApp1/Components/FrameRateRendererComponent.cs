using System;
using System.Windows.Media;

namespace WpfApp1
{
    public class FrameRateRendererComponent : Component, IRenderable
    {
        public bool shouldDraw => true;
        public float[] deltaTimes = new float[50];
        public int lastAddedIndex = 0;

        public int lastFramerate = 0;
        public int numFramesPassedSinceLastFrameTextUpdate = 0;

        public override void ReceiveRender()
        {
            base.ReceiveRender();

            deltaTimes[lastAddedIndex++] = SDLApp.DeltaTime;
            lastAddedIndex %= deltaTimes.Length;

            if (numFramesPassedSinceLastFrameTextUpdate > 40)
            {
                UpdateFrameRate();
            }

            SDLRendering.DrawText(lastFramerate.ToString(), "arial-16", SDLRendering._screenWidth - 100, 60, Color.FromRgb(255, 255, 255));
            numFramesPassedSinceLastFrameTextUpdate++;
        }

        private void UpdateFrameRate()
        {
            float averageDeltaTime = deltaTimes[0];

            for (int i = 1; i < lastAddedIndex; i++)
            {
                averageDeltaTime += deltaTimes[i];
            }

            averageDeltaTime /= lastAddedIndex;
            averageDeltaTime = 1.0f / averageDeltaTime;

            lastFramerate = (int)Math.Floor(averageDeltaTime);
            numFramesPassedSinceLastFrameTextUpdate = 0;
        }
    }

}