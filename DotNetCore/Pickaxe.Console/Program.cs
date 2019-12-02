using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
            string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string log4netPath = Path.Combine(Path.GetDirectoryName(location), "Log4net.config");
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(logRepository, new FileInfo(log4netPath));

            if (args.Length != 0)
            {
                if (!File.Exists(args[0]))
                {
                    System.Console.WriteLine(string.Format("File {0} not found.", args[0]));
                    return;
                }

                //run the file
                var sources = new List<string>();
                using (var reader = new StreamReader(args[0]))
                {
                    sources.Add(reader.ReadToEnd());
                }

                Runner.Run(sources.ToArray(), new string[] { });
            }
            else
            {
                Interactive.Prompt();
            }
        }
    }
}
