using Pickaxe.Emit;
using Pickaxe.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pickaxe.Console
{
    internal class Runner
    {
        private const int MaxColumnWidth = 75;

        private static void ListErrors(string[] errors)
        {
            foreach (var error in errors)
                System.Console.WriteLine(error);
        }

        private static void PrintRunning()
        {
            System.Console.WriteLine("Running...");
        }

        private static string Truncate(string text)
        {
            if (text.Length > MaxColumnWidth)
                text = text.Substring(0, MaxColumnWidth);

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
                if ((row + 1).ToString().Length + 2 > lengths[0])
                    lengths[0] = (row + 1).ToString().Length + 2;

                for (int col = 0; col < lengths.Count - 1; col++)
                {
                    int len = "NULL".ToString().Length + 2;
                    if (result[row][col] != null)
                        len = Truncate(result[row][col].ToString()).Length + 2;

                    if (len > lengths[col + 1])
                        lengths[col + 1] = len;
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
                string columnValue = Truncate(columns[x]);
                middle.Append("|");
                int totalPadding = (lengths[x] - columnValue.Length);
                int leftPadding = 1;
                int righPaddding = totalPadding - leftPadding;
                for (int pad = 0; pad < leftPadding; pad++)
                    middle.Append(" ");

                middle.Append(string.Format("{0}", Truncate(columnValue)));
                for (int pad = 0; pad < righPaddding; pad++)
                    middle.Append(" ");
            }
            middle.Append("|");

            return middle.ToString();
        }

        private static void OnSelectResults(RuntimeTable<ResultRow> result)
        {
            var lengths = Measure(result);

            //+--+-------------------+------------+               
            //|  |  (No column name) | .content a |
            //+--+-------------------+------------+

            var border = Border(lengths);
            System.Console.WriteLine(border);
            var values = result.Columns().ToList();
            values.Insert(0, "");
            System.Console.WriteLine(Values(lengths, values.ToArray()));
            System.Console.WriteLine(border.ToString());

            for (int row = 0; row < result.RowCount; row++)
            {
                var valueList = new List<string>();
                for (int col = 0; col < lengths.Count - 1; col++)
                {
                    if (result[row][col] != null)
                        valueList.Add(result[row][col].ToString());
                    else
                        valueList.Add("NULL");
                }

                valueList.Insert(0, (row + 1).ToString());
                System.Console.WriteLine(Values(lengths, valueList.ToArray()));
            }

            System.Console.WriteLine(border.ToString());            
        }

        public static void Run(string[] source, string[] args)
        {
            var compiler = new Compiler(source);
            var generatedAssembly = compiler.ToAssembly();

            if (compiler.Errors.Any())
                ListErrors(compiler.Errors.Select(x => x.Message).ToArray());

            if (!compiler.Errors.Any())
            {
                var runable = new Runable(generatedAssembly, args);
                runable.Select += OnSelectResults;
                //runable.Progress += OnProgress;

                try
                {
                    PrintRunning();
                    runable.Run();
                    runable.Select -= OnSelectResults;
                }
                catch (ThreadAbortException)
                {
                    System.Console.WriteLine("Program aborted");
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Unexpected Exception: {0}", e);
                }
            }
        }
    }
}
