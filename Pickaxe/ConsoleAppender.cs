using log4net.Appender;
using Pickaxe.PlatConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe
{
    public class ConsoleAppender : AppenderSkeleton
    {
        public static object ConsoleWriteLock = new object();
        internal static IConsole PlatConsole;

        static ConsoleAppender()
        {
            PlatConsole = CreateConsole();
        }

        private static IConsole CreateConsole()
        {
            if (IsWindows)
                return new WindowsConsole();

            return new UnixConsole();
        }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            lock (ConsoleWriteLock)
            {
                PlatConsole.MoveCursor(PlatConsole.StartLine);
                PlatConsole.ClearLine(PlatConsole.StartLine);
                PlatConsole.Print(RenderLoggingEvent(loggingEvent));
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
    }
}
