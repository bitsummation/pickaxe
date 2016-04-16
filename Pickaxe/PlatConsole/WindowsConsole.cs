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

        public void Print(string value)
        {
            Console.WriteLine(value);
            CurrentLine++;
        }
    }
}
