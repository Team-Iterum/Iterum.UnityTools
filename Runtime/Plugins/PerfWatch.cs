using System;
using System.Diagnostics;
using Iterum.Logs;
using Iterum.Utils;

namespace Iterum.BaseSystems.Utility
{

    public class PerfWatch : Stopwatch
    {
        public bool Enabled { get; set; } = true;

        private string group;
        private double totalSeconds = 0;

        private PerfWatch(string group)
        {
            this.group = group;
        }

        public static PerfWatch StartNew(string group = "PerfWatch")
        {
            var perfWatch = new PerfWatch(group);
            perfWatch.Start();
            return perfWatch;
        }

        public PerfWatch LogTotal(string elapsedText = "Total", double minSeconds = 0)
        {
            Stop();
            if (!Enabled) return this;


            double seconds = totalSeconds + TimeConvert.TicksToSeconds(ElapsedTicks);

            if (seconds >= minSeconds)
            {
                InternalLog(elapsedText, seconds, minSeconds);
            }

            return this;
        }



        public PerfWatch Log(string elapsedText = "Elapsed", double minSeconds = 0)
        {
            Stop();
            if (!Enabled) return this;

            double seconds = TimeConvert.TicksToSeconds(ElapsedTicks);

            if (seconds >= minSeconds)
            {
                InternalLog(elapsedText, seconds, minSeconds);

                totalSeconds += seconds;

            }

            Restart();

            return this;
        }

        private void InternalLog(string elapsedText, double seconds, double min)
        {
            var consoleColor = ConsoleColor.DarkGray;

            if (seconds >= min) consoleColor = ConsoleColor.Yellow;

            if (seconds >= 1f)
            {
                Logs.Log.Debug(group, $"{elapsedText}: {seconds:F}s", consoleColor);
            }
            else
            {
                double mSeconds = TimeConvert.SecondsToMs((float)seconds);

                Logs.Log.Debug(group, $"{elapsedText}: {mSeconds:F}ms", consoleColor);
            }
        }
    }
}