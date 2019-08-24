using System;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using Magistr.Math;

namespace Magistr.Log
{
    public static class Debug
    {
        static Debug()
        {
        }
        private static ConsoleColor backColor;
        public static void Back(ConsoleColor consoleColor)
        {
            backColor = System.Console.BackgroundColor;
            Console.BackgroundColor = consoleColor;
        }
        public static void ResetBack()
        {
            Console.BackgroundColor = backColor;
        }

        public static void Log(string str, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            var msg = timestamp ? $"[{DateTime.Now.ToLongTimeString()}] {str}" : str;
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = foreground;
        }
        public static void LogError(string str)
        {
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = System.ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {str}");
            Console.ForegroundColor = foreground;
            
        }
    }
}

#endif