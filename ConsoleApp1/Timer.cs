using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    
    public class Timer
    {
        public event Action TimeElapsed;
        public event Action<TimeEvent> Tick;

        public bool ShouldLoop;

        private int _id;
        private float _totalTime;

        public Timer(float seconds, bool shouldLoop)
        {
            ShouldLoop = false;
            _id = SDL.SDL_AddTimer((uint)(seconds * 1000.0f), OnTimerTick, IntPtr.Zero);
            _totalTime = 0;
            ShouldLoop = shouldLoop; 
        }

        private uint OnTimerTick(uint inverval, IntPtr param)
        {
            float secondsSinceLastTick = inverval / 1000.0f;

            _totalTime += secondsSinceLastTick;
            
            Tick?.Invoke(new TimeEvent(secondsSinceLastTick, _totalTime));

            if (!ShouldLoop)
            {
                _id = -1;
                TimeElapsed?.Invoke();
                return 0;
            }

            return inverval;
        }
    }
}
