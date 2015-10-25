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

using log4net;
using Pickaxe.Runtime;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pickaxe.Emit;

namespace Pickaxe
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void ListErrors(string[] errors)
        {
            foreach (var error in errors)
                Console.WriteLine(error);
        }

        private static void Compile(string[] source)
        {
            Log.Info("Compiling...");

            var compiler = new Compiler(source);
            var generatedAssembly = compiler.ToAssembly();

            if (compiler.Errors.Any())
                ListErrors(compiler.Errors.Select(x => x.Message).ToArray());

            if (!compiler.Errors.Any())
            {
                var runable = new Runable(generatedAssembly);
                //runable.Select += OnSelectResults;
                //runable.Progress += OnProgress;
                //runable.Highlight += OnHighlight;

                try
                {
                    Log.Info("Running...");
                    runable.Run();
                }
                catch (ThreadAbortException)
                {
                    Log.Info("Program aborted");
                }
                catch (Exception e)
                {
                    Log.Fatal("Unexpected Exception", e);
                }
            }
        }

        public static void Main(string[] args)
        {
            string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string log4netPath = Path.Combine(Path.GetDirectoryName(location), "Log4net.config");
            log4net.Config.XmlConfigurator.Configure(new FileInfo(log4netPath));

            var sources = new List<string>();
            foreach (var arg in args)
            {
                //read the files
                var reader = new StreamReader(arg);
                sources.Add(reader.ReadToEnd());
            }

            Thread thread = new Thread(() => Compile(sources.ToArray()));
            thread.Start();
            thread.Join();
        }
    }
}
