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

        public static void Main(string[] args)
        {
            ConsoleAppender.PlatConsole.Init();

            string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string log4netPath = Path.Combine(Path.GetDirectoryName(location), "Log4net.config");
            log4net.Config.XmlConfigurator.Configure(new FileInfo(log4netPath));

            if (args.Length == 0) //interactive prompt
                Interactive();
            else
            { 
                //run the file
                var sources = new List<string>();

                if (!File.Exists(args[0]))
                {
                    ConsoleAppender.PlatConsole.Print(string.Format("File {0} not found.", args[0]));
                    return;
                }

                //read the files
                var reader = new StreamReader(args[0]);
                sources.Add(reader.ReadToEnd());

                Thread thread = new Thread(() => Compile(sources.ToArray(), args.Skip(1).ToArray()));
                thread.Start();
                thread.Join();
            }
        }

        private static void ListErrors(string[] errors)
        {
            foreach (var error in errors)
                ConsoleAppender.PlatConsole.Print(error);
        }

        private static void PrintRunning()
        {
            ConsoleAppender.PlatConsole.MoveCursor(ConsoleAppender.PlatConsole.StartLine + 2);
            ConsoleAppender.PlatConsole.ClearLine(ConsoleAppender.PlatConsole.StartLine + 2);
            ConsoleAppender.PlatConsole.Print("Running...");
        }

        private static void Compile(string[] source, string[] args)
        {
            ConsoleAppender.PlatConsole.StartLine = ConsoleAppender.PlatConsole.CurrentLine + 1;
            ConsoleAppender.PlatConsole.MoveCursor(ConsoleAppender.PlatConsole.StartLine);

            var compiler = new Compiler(source);
            var generatedAssembly = compiler.ToAssembly();

            if (compiler.Errors.Any())
                ListErrors(compiler.Errors.Select(x => x.Message).ToArray());

            if (!compiler.Errors.Any())
            {
                var runable = new Runable(generatedAssembly, args);
                runable.Select += OnSelectResults;
                runable.Progress += OnProgress;

                try
                {
                    PrintRunning();
                    runable.Run();
                    ConsoleAppender.PlatConsole.Print("Finished.");
                    ConsoleAppender.PlatConsole.StartLine = ConsoleAppender.PlatConsole.CurrentLine;
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

        private static void Interactive()
        {
            //interactive prompt ; delimited.
            var builder = new StringBuilder();
            Console.Write("pickaxe> ");
            while (true)
            {
                char character = Convert.ToChar(Console.Read());

                if (character == '\n')
                    Console.Write("      -> ");
                if (character == ';') //run it. 
                {
                    while (Convert.ToChar(Console.Read()) != '\n') {} //clear buf

                    Thread thread = new Thread(() => Compile(new[]{builder.ToString()}, new string[0]));
                    thread.Start();
                    thread.Join();

                    builder.Clear();
                    Console.Write("pickaxe> ");
                    continue;
                }

                builder.Append(character);
            }
        }

        private static string Truncate(string text)
        {
            if (text.Length > 50)
                text = text.Substring(0, 50);

            return text;
        }

        private static List<int> Measure(RuntimeTable<ResultRow> result)
        {
            var lengths = new List<int>();
            lengths.Add(1);

            foreach (var column in result.Columns()) //headers
                lengths.Add(Truncate(column).Length + 2);

            for (int row = 0; row < result.RowCount; row++)
            {
                if ((row+1).ToString().Length+2 > lengths[0])
                    lengths[0] = (row+1).ToString().Length+2;

                for (int col = 0; col < lengths.Count - 1; col++)
                {
                    int len = Truncate(result[row][col].ToString()).Length + 2;
                    if (len > lengths[col+1])
                        lengths[col+1] = len;
                }
            }

            return lengths;
        }

        private static string Border(List<int> lengths)
        {
            var topBottom = new StringBuilder();
            for (int x = 0; x < lengths.Count; x++)
            {
                topBottom.Append("+");
                for (int len = 0; len < lengths[x]; len++)
                    topBottom.Append("-");
            }
            topBottom.Append("+");
            return topBottom.ToString();
        }

        private static string Values(List<int> lengths, string[] values)
        {
            var middle = new StringBuilder();
            var columns = values;
            for (int x = 0; x < lengths.Count; x++)
            {
                middle.Append("|");
                int totalPadding = (lengths[x] - columns[x].Length);
                int leftPadding = 1;
                int righPaddding = totalPadding - leftPadding;
                for (int pad = 0; pad < leftPadding; pad++)
                    middle.Append(" ");

                middle.Append(string.Format("{0}", Truncate(columns[x])));
                for (int pad = 0; pad < righPaddding; pad++)
                    middle.Append(" ");
            }
            middle.Append("|");

            return middle.ToString();
        }

        private static string RenderProgress(ProgressArgs e)
        {
            float value = 0;
            if (e.TotalOperations > 0)
                value = (e.CompletedOperations / (float)e.TotalOperations);

            //[###-----------------] 35/100  35%
            var builder = new StringBuilder();
            int map = (int)(Math.Round(value * 20));
            builder.Append("[");
            for (int x = 0; x < 20; x++)
            {
                if (x < map)
                    builder.Append("#");
                else
                    builder.Append("-");
            }
            builder.Append("]");

            return string.Format("{0} {1}/{2} {3}%", builder.ToString(), e.CompletedOperations, e.TotalOperations, (int)Math.Round(value * 100));
        }

        private static void OnProgress(ProgressArgs e)
        {
            lock (ConsoleAppender.ConsoleWriteLock)
            {
                ConsoleAppender.PlatConsole.MoveCursor(ConsoleAppender.PlatConsole.StartLine + 1);
                ConsoleAppender.PlatConsole.ClearLine(ConsoleAppender.PlatConsole.StartLine + 1);

                ConsoleAppender.PlatConsole.Print(RenderProgress(e));
                PrintRunning();
            }
        }

        private static void OnSelectResults(RuntimeTable<ResultRow> result)
        {
            lock (ConsoleAppender.ConsoleWriteLock)
            {
                ConsoleAppender.PlatConsole.MoveCursor(ConsoleAppender.PlatConsole.StartLine + 3);

                var lengths = Measure(result);

                //+--+-------------------+------------+               
                //|  |  (No column name) | .content a |
                //+--+-------------------+------------+

                var border = Border(lengths);
                ConsoleAppender.PlatConsole.Print(border);
                var values = result.Columns().ToList();
                values.Insert(0, "");
                ConsoleAppender.PlatConsole.Print(Values(lengths, values.ToArray()));
                ConsoleAppender.PlatConsole.Print(border.ToString());

                for (int row = 0; row < result.RowCount; row++)
                {
                    var valueList = new List<string>();
                    for (int col = 0; col < lengths.Count - 1; col++)
                        valueList.Add(result[row][col].ToString());

                    valueList.Insert(0, (row + 1).ToString());
                    ConsoleAppender.PlatConsole.Print(Values(lengths, valueList.ToArray()));
                }
                ConsoleAppender.PlatConsole.Print(border.ToString());

                ConsoleAppender.PlatConsole.StartLine = ConsoleAppender.PlatConsole.CurrentLine;
            }
        }
    }
}
