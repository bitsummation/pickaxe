using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pickaxe.Console
{
    public class ConsoleAppender : AppenderSkeleton
    {
        public static object ConsoleWriteLock = new object();

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            lock (ConsoleWriteLock)
            {
                System.Console.WriteLine(RenderLoggingEvent(loggingEvent));
            }
        }

    }
}
