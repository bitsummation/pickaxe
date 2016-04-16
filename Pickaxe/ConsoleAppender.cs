/* Copyright 2015 Brock Reeve
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
