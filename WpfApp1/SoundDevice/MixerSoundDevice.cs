using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static SDL2.SDL;
using static SDL2.SDL_mixer;

namespace WpfApp1.Components
{
    internal class MixerSoundDevice : SoundDevice
    {
        private IDictionary<string, IntPtr> _musics;
        private IDictionary<string, IntPtr> _sounds;

        private IntPtr _currentSound;
        private MusicFinishedDelegate endMusic;
        public MixerSoundDevice() :
            base()
        {
            _musics = new Dictionary<string, IntPtr>();
            _sounds = new Dictionary<string, IntPtr>();
            _currentSound = IntPtr.Zero;
            endMusic = BroadcastMusicEnded;
        }

        public override void OnFreeDispatched()
        {
            foreach (var music in _musics.Values)
            {
                Mix_FreeMusic(music);
            }

            foreach (var sound in _sounds.Values)
            {
                Mix_FreeChunk(sound);
            }

            _musics.Clear();
            _sounds.Clear();

            Mix_Quit();
        }

        protected override string GetCurrentPlayingMusicKey()
        {
            if (_currentSound == IntPtr.Zero)
            {
                return string.Empty;
            }

            var str = _musics.First(v => v.Value.Equals(_currentSound)).Key;
            return str;
        }

        protected override void PauseCurrentPlayingMusicImpl()
        {
            if (_currentSound != IntPtr.Zero)
            {
                Mix_PauseMusic();
                BroadcastMusicPaused();
            }
        }

        protected override void PlaySoundImpl(string key, int channel)
        {
            if (!_sounds.ContainsKey(key))
            {
                return;
            }

            IntPtr sound = _sounds[key];
            Mix_PlayChannel(channel, sound, 0);
        }

        protected override void RegisterMusicImpl(string path, string key)
        {
            if (_musics.ContainsKey(key))
            {
                Debug.WriteLine("Music with that key already exists");
                return;
            }

            IntPtr music = Mix_LoadMUS(path);

            if (music == IntPtr.Zero)
            {
                Debug.WriteLine("Error during loading music: " + SDL_GetError());
                return;
            }

            Mix_HookMusicFinished(endMusic);
            _musics.Add(key, music);
        }
        


        protected override void RegisterSoundImpl(string path, string key)
        {
            if (_sounds.ContainsKey(key))
            {
                Debug.WriteLine("Sound with that key already exists");
                return;
            }
            IntPtr sound = Mix_LoadWAV(path);

            if (sound == IntPtr.Zero)
            {
                Debug.WriteLine("Error during loading sound: " + SDL_GetError());
                return;
            }

            _sounds.Add(key, sound);
        }

        protected override void ResumeCurrentPlayingMusicImpl()
        {
            if (_currentSound != IntPtr.Zero)
            {
                Mix_ResumeMusic();
                BroadcastMusicResumed();
            }
        }

        protected override void StartPlayingMusicImpl(string key, int loops)
        {
            if (!_musics.ContainsKey(key))
            {
                Debug.WriteLine("Such sound doesn't exists");
                return;
            }

            _currentSound = _musics[key];
            Mix_HaltMusic();
            Mix_PlayMusic(_currentSound, loops);
        }

        protected override void StopCurrentPlayingMusicImpl()
        {
            Mix_HaltMusic();
            _currentSound = IntPtr.Zero;
        }
    }
}
