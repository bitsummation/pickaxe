using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.PlatConsole
{
    internal class UnixConsole : IConsole
    {
        public int CurrentLine
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int StartLine
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void ClearConsole()
        {
            throw new NotImplementedException();
        }

        public void ClearLine(int line)
        {
            throw new NotImplementedException();
        }

        public void MoveCursor(int line)
        {
            throw new NotImplementedException();
        }

        public void Print(string value)
        {
            throw new NotImplementedException();
        }
    }
}
