using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.PlatConsole
{
    internal class UnixConsole : IConsole
    {
        public void Init()
        {
            Console.Write("\u001b[2J");
            StartLine = 1;
            CurrentLine = 1;
        }

        public int StartLine { get; set; }
        public int CurrentLine { get; set; }

        public void MoveCursor(int line)
        {
            Console.Write("\u001b[{0};{1}H", line, 0);
            CurrentLine = line;
        }

        public void ClearLine(int line)
        {
            MoveCursor(line);
            Console.Write("\u001b[K");
            MoveCursor(line);
        }

        public void Print(string value)
        {
            Console.WriteLine(value);
            CurrentLine++;
        }
    }
}
