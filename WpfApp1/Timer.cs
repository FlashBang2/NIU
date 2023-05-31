using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ConsoleApp1
{
    public struct TimeEvent
    {
        public float SecondsSinceLastTick;
        public float TotalSeconds;

        public TimeEvent(float secondsSinceLastTick, float totalSeconds)
        {
            SecondsSinceLastTick = secondsSinceLastTick;
            TotalSeconds = totalSeconds;
        }
    }
    
    public class SDLTimer
    {
        public event Action TimeElapsed;
        public event Action Tick;

        public bool ShouldLoop;

        private int _id;
        private float _totalTime;

        private Timer _Handle;

        public SDLTimer(float seconds, bool shouldLoop)
        {
            _Handle = new Timer(seconds * 1000);
            _Handle.AutoReset = shouldLoop;
            _Handle.Elapsed += OnState;
            _Handle.Start();

            ShouldLoop = false;
            _totalTime = 0;
            ShouldLoop = shouldLoop; 
        }

        private void OnState(object o, ElapsedEventArgs args)
        {
            if (!ShouldLoop)
            {
                TimeElapsed?.Invoke();
                _Handle.Stop();
            }

            Tick?.Invoke();
        }
    }
}
