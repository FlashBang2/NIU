using System;

namespace WpfApp1.Components
{
    public class NullSoundDevice : SoundDevice
    {
        public NullSoundDevice() : base()
        {
            Console.WriteLine("Null sound device used. No sound will be played !");
        }

        public override void OnFreeDispatched()
        {
        }

        protected override void PauseCurrentPlayingMusicImpl()
        {
        }

        protected override void PlaySoundImpl(string key, int channel)
        {
        }

        protected override void RegisterMusicImpl(string path, string key)
        {
        }

        protected override void RegisterSoundImpl(string path, string key)
        {
        }

        protected override void ResumeCurrentPlayingMusicImpl()
        {
        }

        protected override void StartPlayingMusicImpl(string key, int loops)
        {
        }

        protected override void StopCurrentPlayingMusicImpl()
        {
        }

        protected override string GetCurrentPlayingMusicKey()
        {
            return string.Empty;
        }
    }
}
