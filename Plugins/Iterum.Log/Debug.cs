using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Iterum.Logs
{
    [Flags]
    public enum Level
    {
        None       = 0,
        Debug      = 1 << 8,
        Info       = 2 << 8,
        Success    = 3 << 8,
        Warn       = 4 << 8,
        Error      = 5 << 8,
        Exception  = 6 << 8,
        Fatal      = 7 << 8,
    }

    
    public static partial class Log
    {
        public static event LogDelegate LogCallback;

        public static Level Enabled = Level.Debug | Level.Info | Level.Success | Level.Warn | Level.Error |
                                            Level.Exception | Level.Fatal;
        
        #region Back Color
        
        private static ConsoleColor backColor;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Back(ConsoleColor consoleColor)
        {
            backColor = Console.BackgroundColor;
            Console.BackgroundColor = consoleColor;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetBack()
        {
            Console.BackgroundColor = backColor;
        }
        #endregion
        
        // ReSharper disable Unity.PerformanceAnalysis
        private static void Send(Level level, string group, string s, 
            ConsoleColor color = ConsoleColor.White, ConsoleColor groupColor = ConsoleColor.DarkGray, 
            bool timestamp = true)
        {
            if (!Enabled.HasFlag(level)) return;
            
            var dateTime = DateTime.Now;
            var finalText = string.Empty;
            var logText = string.Empty;
            
            // Timestamp
            {
                var foreground = Console.ForegroundColor;
                if (timestamp)
                {
                    var text = $"{dateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern)} ";
                    
#if UNITY_2018_3_OR_NEWER
                    logText += text;
                    text = Tagged(text, ConsoleColor.DarkGray);
                    finalText += text;
#else
                    finalText += text;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(text);
                    Console.ForegroundColor = foreground;
#endif
                }
            }
            
            // Level
            {
                var foreground = Console.ForegroundColor;
                if (timestamp)
                {
#if UNITY_2018_3_OR_NEWER
                    logText += GetLevel(level);
                    finalText += Tagged(GetLevelUnity(level), GetColorLevel(level));
                    
#else
                    finalText += $"[{level}] ";
                    Console.ForegroundColor = GetColorLevel(level);
                    Console.Write(text);
                    Console.ForegroundColor = foreground;
#endif
                }
            }

            // Group
            {
                var foreground = Console.ForegroundColor;
                if (group != null)
                {
                    var text = $"[{group}] ";
#if UNITY_2018_3_OR_NEWER
                    logText += text;
                    text = Tagged(text, groupColor);
                    finalText += text;
#else
                    finalText += text;
                    Console.ForegroundColor = groupColor;
                    Console.Write(text);
                    Console.ForegroundColor = foreground;
#endif
                }
            }

            // Text
            {
                
#if UNITY_2018_3_OR_NEWER 
                logText += s + Environment.NewLine;
                if (level != Level.Exception)
                    s = Tagged(s, color);
                finalText += s;
#else
                finalText += s;
                var foreground = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.Write(s);
                Console.ForegroundColor = foreground;
                Console.Write(Environment.NewLine);
#endif
            }

#if UNITY_2018_3_OR_NEWER   
            UnityEngine.Debug.Log(finalText);
            OnLogCallback(dateTime, level, group, s, logText, color);
#endif
        }

        private static string GetLevel(Level level)
        {
            switch (level)
            {
                case Level.None:
                    break;
                case Level.Debug:
                    return "[Debug]   ";
                case Level.Info:
                    return "[Info]    ";
                case Level.Success:
                    return "[Success] ";
                case Level.Warn:
                    return "[Warning] ";
                case Level.Error:
                    return "[Error]   ";
                case Level.Exception:
                    return "[Exception] ";
                case Level.Fatal:
                    return "[Fatal]     ";
            }

            return string.Empty;
        }
        
        private static string GetLevelUnity(Level level)
        {
            switch (level)
            {
                case Level.None:
                    break;
                case Level.Debug:
                    return "[Debug]     ";
                case Level.Info:
                    return "[Info]          ";
                case Level.Success:
                    return "[Success] ";
                case Level.Warn:
                    return "[Warning] ";
                case Level.Error:
                    return "[Error]     ";
                case Level.Exception:
                    return "[Exception] ";
                case Level.Fatal:
                    return "[Fatal]     ";
            }

            return string.Empty;
        }


        private static ConsoleColor GetColorLevel(Level level)
        {
            var color = level switch
            {
                Level.Debug     => ConsoleColor.Gray,
                Level.Info      => ConsoleColor.Blue,
                Level.Success   => ConsoleColor.Green,
                Level.Warn      => ConsoleColor.Yellow,
                Level.Error     => ConsoleColor.Red,
                Level.Exception => ConsoleColor.Red,
                Level.Fatal     => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            return color;
        }
        
        private static string Tagged(string text, ConsoleColor color)
        {
            string textColor = color switch
            {
                ConsoleColor.Black       => "#000",
                ConsoleColor.Blue        => "#88f",
                ConsoleColor.Cyan        => "#0ff",
                ConsoleColor.DarkBlue    => "#009",
                ConsoleColor.DarkCyan    => "#099",
                ConsoleColor.DarkGray    => "#909090",
                ConsoleColor.DarkGreen   => "#090",
                ConsoleColor.DarkMagenta => "#909",
                ConsoleColor.DarkRed     => "#b00",
                ConsoleColor.DarkYellow  => "#aa0",
                ConsoleColor.Gray        => "#ccc",
                ConsoleColor.Green       => "#0f0",
                ConsoleColor.Magenta     => "#f0f",
                ConsoleColor.Red         => "#f00",
                ConsoleColor.White       => "#eee",
                ConsoleColor.Yellow      => "#ff0",
                _ => "#fff"
            };

            return $"<color={textColor}>{text}</color>";
        }


        private static void OnLogCallback(DateTime time, Level level, string group, string msg, string fullMessage, ConsoleColor color)
        {
            LogCallback?.Invoke(time, level, group, msg, fullMessage, color);
        }
    }
}
