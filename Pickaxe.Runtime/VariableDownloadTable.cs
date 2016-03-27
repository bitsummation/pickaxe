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
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime
{
    public class VariableDownloadTable : ThreadedDownloadTable
    {
        private IList<LazyDownloadPage> _pages;

        public VariableDownloadTable(IRuntime runtime, int line, int threadCount, string url)
            : base(runtime, line, threadCount, url)
        {
             _pages = new List<LazyDownloadPage>();
             InitPages();
        }

        public VariableDownloadTable(IRuntime runtime, int line, int threadCount, Table<ResultRow> table)
            : base(runtime, line, threadCount, table)
        {
            _pages = new List<LazyDownloadPage>();
            InitPages();
        }

        private void InitPages()
        {
            foreach (string url in Urls)
            {
                _pages.Add(new VariableDownloadPage(this));
            }
        }

        public override IEnumerator<DownloadPage> GetEnumerator() //Give out empty lazy wrappers
        {
            return _pages.GetEnumerator();
        }
    }
}
