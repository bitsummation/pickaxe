using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.PlatConsole
{
    internal interface IConsole
    {
        void Init();

        int StartLine { get; set; }
        int CurrentLine { get; set; }

        void MoveCursor(int line);
        void ClearLine(int line);
        void Print(string value);
    }
}
