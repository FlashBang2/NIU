using System;
using System.Diagnostics;
using static SDL2.SDL_mixer;

namespace WpfApp1.Components
{

    public abstract class SoundDevice
    {
        private static SoundDevice instance = null;

        public const int MusicPlayOnceLoopCount = 0;
        protected bool _isAnyMusicPlaying = false;

        public static event Action<string> OnMusicEnded;
        public static event Action<string> OnMusicPaused;
        public static event Action<string> OnMusicResumed;

        public SoundDevice()
        {
            instance = this;
        }

        public static void RegisterSound(string path, string key)
        {
            Debug.Assert(instance != null);
            instance.RegisterSoundImpl(path, key);
        }

        public static void RegisterMusic(string path, string key)
        {
            Debug.Assert(instance != null);
            instance.RegisterMusicImpl(path, key);
        }

        public static void PlaySound(string key, int channel)
        {
            Debug.Assert(instance != null);
            instance.PlaySoundImpl(key, channel);
        }

        public static void PlayMusic(string key, int loops = MusicPlayOnceLoopCount)
        {
            Debug.Assert(instance != null);
            instance.StartPlayingMusicImpl(key, loops);
        }

        public static void StopCurrentPlayingMusic()
        {
            Debug.Assert(instance != null);
            instance.StopCurrentPlayingMusicImpl();
        }

        public static void PauseCurrentPlayingMusic()
        {
            Debug.Assert(instance != null);
            instance.PauseCurrentPlayingMusicImpl();
        }

        public static void ResumeCurrentPlayingMusic()
        {
            Debug.Assert(instance != null);
            instance.ResumeCurrentPlayingMusicImpl();
        }

        public static bool IsAnyMusicPlaying()
        {
            Debug.Assert(instance != null);
            return instance._isAnyMusicPlaying;
        }

        public static SoundDevice CreateSoundDevice()
        {
            MIX_InitFlags mixerFlags = MIX_InitFlags.MIX_INIT_OGG;
            int intMixerFlags = (int)(mixerFlags);

            if (Mix_Init(mixerFlags) == intMixerFlags)
            {
                MixerInitializationParams mixerParams = new MixerInitializationParams();
                mixerParams.SetDefaults();
                mixerParams.PassToOpenAudio();
                return new MixerSoundDevice();
            }

            return new NullSoundDevice();
        }

        protected void BroadcastMusicEnded()
        {
            var str = GetCurrentPlayingMusicKey();
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            OnMusicEnded?.Invoke(str);
        }

        protected void BroadcastMusicPaused()
        {
            var str = GetCurrentPlayingMusicKey();
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            OnMusicPaused?.Invoke(str);
        }

        protected void BroadcastMusicResumed()
        {
            var str = GetCurrentPlayingMusicKey();
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            OnMusicResumed?.Invoke(str);
        }

        protected abstract void RegisterSoundImpl(string path, string key);
        protected abstract void PlaySoundImpl(string key, int channel);
        protected abstract void RegisterMusicImpl(string path, string key);
        protected abstract void StartPlayingMusicImpl(string key, int loops);
        protected abstract void StopCurrentPlayingMusicImpl();
        protected abstract void PauseCurrentPlayingMusicImpl();
        protected abstract void ResumeCurrentPlayingMusicImpl();
        protected abstract string GetCurrentPlayingMusicKey();

        public abstract void OnFreeDispatched();
    }
}
