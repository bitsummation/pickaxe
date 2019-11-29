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
                var line = System.Console.ReadLine();
                builder.Append(line);
                if(line.EndsWith(';')) //run it
                {
                    builder.Remove(builder.Length - 1, 1); //remove ;
                    var source = builder.ToString();
                    Thread thread = new Thread(() => Runner.Run(new[] { source }, new string[0]));
                    thread.Start();
                    thread.Join();

                    builder.Clear();

                    System.Console.Write("pickaxe> ");
                    continue;
                }
                
                System.Console.Write("      -> ");
            }
        }
    }
}
