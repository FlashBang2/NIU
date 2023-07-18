using System;
using System.Timers;

namespace ConsoleApp1
{
    public class SDLTimer
    {
        public event Action onTimeElapsed;
        public event Action onTick;

        public bool shouldLoop;
        private Timer _handle;

        public SDLTimer(float seconds, bool shouldLoop)
        {
            _handle = new Timer(seconds * 1000)
            {
                AutoReset = shouldLoop
            };

            _handle.Elapsed += OnState;
            _handle.Start();

            this.shouldLoop = shouldLoop;
        }

        private void OnState(object o, ElapsedEventArgs args)
        {
            if (!shouldLoop)
            {
                onTimeElapsed?.Invoke();
                _handle.Stop();
            }

            onTick?.Invoke();
        }
    }
}
