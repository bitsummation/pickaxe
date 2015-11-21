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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pickaxe.Runtime
{
    public class FileTable<TRow> : CodeTable<TRow> where TRow : IRowWriter, IRowReader, new()
    {
        private StreamWriter _writer;
        private string _rowTerminator;

        public string FieldTerminator { get; set; }
        public string Location { get; set; }

        public string RowTerminator {
            get {return _rowTerminator;}
            set { _rowTerminator = CleanUpTerminator(value); }
        }

        public void Load()
        {
            if(File.Exists(Location))
            {
                using(var reader = new StreamReader(Location))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var columns = line.Split(new[] { FieldTerminator }, StringSplitOptions.None);
                        var row = new TRow();
                        row.Load(columns);
                        Rows.Add(row);
                    }
                }
            }
        }

        public override void BeforeInsert(bool overwrite)
        {
            base.BeforeInsert(overwrite);
            _writer = new StreamWriter(Location, !overwrite);
        }

        public override void AfterInsert()
        {
            _writer.Dispose();
        }

        private static string CleanUpTerminator(string terminator)
        {
            var builder = new StringBuilder();
            for (int x = 0; x < terminator.Length; x++)
            {
                if (terminator[x] == '\\')
                {
                    if(x+1 < terminator.Length)
                    {
                        if (terminator[x+1] == 'n')
                            builder.Append('\n');
                        if(terminator[x+1] == 'r')
                            builder.Append('\r');

                        x++;
                    }

                } 
                else
                    builder.Append(terminator[x]);
            }

            return builder.ToString();
        }

        public override void Add(TRow row)
        {
            base.Add(row);

            var values = row.Line();
            string line = string.Join(FieldTerminator, values) + RowTerminator;
            _writer.Write(line);
        }
    }
}
