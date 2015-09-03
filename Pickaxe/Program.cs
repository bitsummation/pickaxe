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
using Pickaxe.CodeDom.Visitor;
using Pickaxe.Parser;
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

        private static void Compile(string source)
        {
            Log.Info("Compiling...");

            var errors = new List<Exception>();
            var parser = new CodeParser(source);
            var ast = parser.Parse();

            if (parser.Errors.Any()) //antlr parse errors
                errors.AddRange(parser.Errors);

            CodeCompileUnit unit = null;
            if (!errors.Any())
            {
                var generator = new CodeDomGenerator(ast);
                unit = generator.Generate();
                if (generator.Errors.Any()) //Semantic erros
                    errors.AddRange(generator.Errors);
            }

            Assembly generatedAssembly = null;
            if (!errors.Any())
            {
                var persist = new Persist(unit);
                generatedAssembly = persist.ToAssembly();
                if (persist.Errors.Any()) //c# compile errors
                    errors.AddRange(persist.Errors.Select(x => new Exception(x)));
            }

            if (errors.Any())
                ListErrors(errors.Select(x => x.Message).ToArray());

            if (!errors.Any())
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

            //read the files
            var reader = new StreamReader(args[0]);
            string source = reader.ReadToEnd();

            Thread thread = new Thread(() => Compile(source));
            thread.Start();
            thread.Join();
        }
    }
}
