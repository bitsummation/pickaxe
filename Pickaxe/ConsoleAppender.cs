using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe
{
    public class ConsoleAppender : AppenderSkeleton
    {
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            Console.SetCursorPosition(0, 0);
            ClearConsoleLine(Console.CursorTop);
            Console.WriteLine(RenderLoggingEvent(loggingEvent));
        }

        public static void ClearConsoleLine(int line)
        {
            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, line);
        }
    }
}
