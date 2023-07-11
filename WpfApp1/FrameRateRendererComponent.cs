using System;
using System.Windows.Media;

namespace WpfApp1
{
    public class FrameRateRendererComponent : Component, IRenderable
    {
        public bool ShouldDraw => true;

        public float[] deltaTimes = new float[50];
        public int index = 0;


        public int LastFramerate = 0;
        public int frames = 0;

        public override void ReceiveRender()
        {
            base.ReceiveRender();

            deltaTimes[index++] = SDLApp.DeltaTime;
            index %= deltaTimes.Length;


            if (frames > 40)
            {
                float AverageDeltaTime = deltaTimes[0];

                for (int i = 1; i < index; i++)
                {
                    AverageDeltaTime += deltaTimes[i];
                }

                AverageDeltaTime /= index;
                AverageDeltaTime = 1000.0f / AverageDeltaTime;

                LastFramerate = (int)Math.Floor(AverageDeltaTime);
                frames = 0;
            }

            SDLRendering.DrawText(LastFramerate.ToString(), "arial-16", SDLRendering._screenWidth - 100, 60, Color.FromRgb(255, 255, 255));
            frames++;
        }
    }

}