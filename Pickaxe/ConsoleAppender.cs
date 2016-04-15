using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe
{
    public class ConsoleAppender : AppenderSkeleton
    {
        public static int StartCursorTop = 0;
        public static object ConsoleWriteLock = new object();

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            lock (ConsoleWriteLock)
            {
                SetCursor(StartCursorTop);
                ClearConsoleLine(Console.CursorTop);
                Console.WriteLine(RenderLoggingEvent(loggingEvent));
            }
        }

        public static bool IsWindows
        {
            get
            {
                return !(Environment.OSVersion.Platform == PlatformID.MacOSX
                    || Environment.OSVersion.Platform == PlatformID.Unix);
            }
        }

        public static void SetCursor(int position)
        {
            if (IsWindows)
                Console.SetCursorPosition(0, position);
            else
                Console.WriteLine("\033[{0};{1}H", position, 0);
        }

        public static void ClearConsoleLine(int line)
        {
            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, line);
        }
    }
}
