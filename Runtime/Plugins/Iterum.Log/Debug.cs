#define UNITY_2018_3_OR_NEWER
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

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
        [HideInCallstack]
        private static void Send(Level level, string group, string s, 
            ConsoleColor color = ConsoleColor.White, ConsoleColor groupColor = ConsoleColor.DarkGray, 
            bool timestamp = true)
        {
            if (!Enabled.HasFlag(level)) return;
            
#if UNITY_EDITOR || UNITY_WEBGL
                timestamp = false;
#endif
            
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
                    #if UNITY_EDITOR
                    finalText += Tagged(GetLevelUnity(level), GetColorLevel(level));
                    #else
                    finalText += $"[{level}] ";
                    #endif
                    
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
                if (level != Level.Exception && level != Level.Error && level != Level.Fatal)
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
            if (level == Level.Exception || level == Level.Error  || level == Level.Fatal)
                UnityEngine.Debug.LogError(finalText);
            else
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
            ConsoleColor color;
            switch (level)
            {
                case Level.Debug:
                    color = ConsoleColor.Cyan;
                    break;
                case Level.Info:
                    color = ConsoleColor.White;
                    break;
                case Level.Success:
                    color = ConsoleColor.Green;
                    break;
                case Level.Warn:
                    color = ConsoleColor.Yellow;
                    break;
                case Level.Error:
                    color = ConsoleColor.Red;
                    break;
                case Level.Exception:
                case Level.Fatal:
                    color = ConsoleColor.DarkRed;
                    break;
                default:
                    color = ConsoleColor.White;
                    break;
            }

            return color;
        }
        
        private static string Tagged(string text, ConsoleColor color)
        {
            string textColor;
            
            switch (color)
            {
                case ConsoleColor.Black:
                    textColor = "#000";
                    break;
                case ConsoleColor.Blue:
                    textColor = "#88f";
                    break;
                case ConsoleColor.Cyan:
                    textColor = "#0ff";
                    break;
                case ConsoleColor.DarkBlue:
                    textColor = "#009";
                    break;
                case ConsoleColor.DarkCyan:
                    textColor = "#099";
                    break;
                case ConsoleColor.DarkGray:
                    textColor = "#909090";
                    break;
                case ConsoleColor.DarkGreen:
                    textColor = "#090";
                    break;
                case ConsoleColor.DarkMagenta:
                    textColor = "#909";
                    break;
                case ConsoleColor.DarkRed:
                    textColor = "#b00";
                    break;
                case ConsoleColor.DarkYellow:
                    textColor = "#aa0";
                    break;
                case ConsoleColor.Gray:
                    textColor = "#ccc";
                    break;
                case ConsoleColor.Green:
                    textColor = "#0f0";
                    break;
                case ConsoleColor.Magenta:
                    textColor = "#f0f";
                    break;
                case ConsoleColor.Red:
                    textColor = "#f00";
                    break;
                case ConsoleColor.White:
                    textColor = "#eee";
                    break;
                case ConsoleColor.Yellow:
                    textColor = "#ff0";
                    break;
                default:
                    textColor = "#fff";
                    break;
            }
#if UNITY_EDITOR
            return $"<color={textColor}>{text}</color>";
#else
            return text;
#endif
        }


        private static void OnLogCallback(DateTime time, Level level, string group, string msg, string fullMessage, ConsoleColor color)
        {
            LogCallback?.Invoke(time, level, group, msg, fullMessage, color);
        }
    }
}
