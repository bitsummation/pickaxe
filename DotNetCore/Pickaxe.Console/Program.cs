using System;

namespace Pickaxe.Console
{
    public class Program
    {
        /*usage: pickaxe [options]
         *usage: pickaxe [path-to-source]
         * 
         * Options:
         * -i   interactive prompt
         * 
         * 
         * 
         */
        public static void Main(string[] args)
        {
            Interactive.Prompt();
        }
    }
}
