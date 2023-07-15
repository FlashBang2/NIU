using SDL2;
using static SDL2.SDL_mixer;

namespace WpfApp1
{
    public struct MixerInitializationParams
    {
        public int frequency;
        /// <summary>
        /// One of AUDIO_* enum
        /// </summary>
        public ushort format;

        public int channels;
        
        public int averageChunkSize;

        public void SetDefaults()
        {
            frequency = 22050;
            format = SDL.AUDIO_S16SYS;
            channels = 2;
            averageChunkSize = 4096;
        }

        public void PassToOpenAudio()
        {
            Mix_OpenAudio(frequency, format, channels, averageChunkSize);
        }
    }
}
