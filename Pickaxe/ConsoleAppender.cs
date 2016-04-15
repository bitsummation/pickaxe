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

        //public static int StartCursorTop { get; set; }
        //public static int CurrentLine { get; set; }

        public static bool IsWindows
        {
            get
            {
                return !(Environment.OSVersion.Platform == PlatformID.MacOSX
                    || Environment.OSVersion.Platform == PlatformID.Unix);
            }
        }

        /*public static void SetCursor(int position)
        {
            if (IsWindows)
                Console.SetCursorPosition(0, position);
            else
            {
                Console.Write("\u001b[2J");
                Console.Write("\u001b[{0};{1}H", 1, 0); //set line
                Console.WriteLine("Print this line");
                Console.Write("\u001b[{0};{1}H", 1, 0); //set line
                Console.Write("\u001b[K"); //erase line
                Console.WriteLine("Erase");
                Console.WriteLine("Another");
                Console.Write("\u001b[K"); //erase line


                Console.Write("\u001b[{0};{1}H", 5, 0); //set line
                Console.WriteLine("Down");
                Console.WriteLine("Down again");
            }
        }*/

        /*public static void ClearConsoleLine(int line)
        {
            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, line);
        }*/
    }
}
