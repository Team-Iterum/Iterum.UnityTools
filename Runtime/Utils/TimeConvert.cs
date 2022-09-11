using System.Diagnostics;

namespace Iterum.Utils
{
    public static class TimeConvert
    {
        public static double TicksToSeconds(double ticks)
        {
            return ticks / Stopwatch.Frequency;
        }

        public static double TicksToMs(double ticks)
        {
            return ticks / Stopwatch.Frequency * 1000.0;
        }

        public static float SecondsToMs(float seconds)
        {
            return seconds * 1000f;
        }
    }
}