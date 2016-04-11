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
                Console.SetCursorPosition(0, StartCursorTop);
                ClearConsoleLine(Console.CursorTop);
                Console.WriteLine(RenderLoggingEvent(loggingEvent));
            }
        }

        public static void SetCursor(int position)
        {
            Console.SetCursorPosition(0, position);
        }

        public static void ClearConsoleLine(int line)
        {
            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, line);
        }
    }
}
