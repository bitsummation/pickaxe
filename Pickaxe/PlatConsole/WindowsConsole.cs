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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.PlatConsole
{
    internal class WindowsConsole : IConsole
    {
        public WindowsConsole()
        {
            StartLine = Console.CursorTop;
            CurrentLine = Console.CursorTop;
        }

        public void Init()
        {
            Console.BufferHeight = Int16.MaxValue - 1;
            Console.BufferWidth = Int16.MaxValue - 1;
        }

        public int StartLine {get; set;}
        public int CurrentLine { get; set; }

        public void MoveCursor(int line)
        {
            Console.SetCursorPosition(0, line);
            CurrentLine = line;
        }

        public void ClearLine(int line)
        {
            MoveCursor(line);
            Console.Write(new string(' ', Console.WindowWidth));
            MoveCursor(line);
        }

        public void PrintError(string value)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Print(value);
            Console.ResetColor();
        }

        public void Print(string value)
        {
            Console.WriteLine(value);
            CurrentLine++;
        }
    }
}
