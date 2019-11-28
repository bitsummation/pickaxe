using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Pickaxe.Console
{
    internal class Interactive
    {
        public static void Prompt()
        {
            var builder = new StringBuilder();
            System.Console.Write("pickaxe> ");
            while (true)
            {
                char character = Convert.ToChar(System.Console.Read());

                if (character == '\n')
                {
                    System.Console.Write("      -> ");
                }
                if (character == ';') //run it. 
                {
                    while (Convert.ToChar(System.Console.Read()) != '\n') { } //clear buf

                    Thread thread = new Thread(() => Runner.Run(new[] { builder.ToString() }, new string[0]));
                    thread.Start();
                    thread.Join();

                    builder.Clear();

                    System.Console.Write("pickaxe> ");
                    continue;
                }

                builder.Append(character);
            }
        }
    }
}
