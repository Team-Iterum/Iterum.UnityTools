using System.Diagnostics;

namespace Iterum.Utils
{
    public class StopwatchTimer
    {
        private Stopwatch watch;
        private long last;
        
        private bool AutoReset { get; }
        public float Interval { get; }
        public int Count { get; private set; }
        public double Elapsed { get; private set; }

        public StopwatchTimer(float interval, bool autoReset = true)
        {
            watch = new Stopwatch();
            Interval = interval;
            AutoReset = autoReset;
            Start();
        }

        public bool Check()
        {
            long elapsed = watch.ElapsedTicks;
            Elapsed += TimeConvert.TicksToSeconds(elapsed - last);
            last = elapsed;
            
            if (Elapsed >= Interval * (Count+1))
            {
                Count += 1;
                if(AutoReset) Reset();
                return true;
            }
            
            return false;
        }

        public void Reset()
        {
            last = watch.ElapsedTicks;
            Elapsed = 0;
            Count = 0;
        }

        private void Start()
        {
            watch.Start();
            Reset();
        }
    }
}