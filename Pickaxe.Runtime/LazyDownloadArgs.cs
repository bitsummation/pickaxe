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

using Pickaxe.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime
{
    public class LazyDownloadArgs
    {
        public static LazyDownloadArgs CreateWebRequestArgs(IRuntime runtime, int line, int threadCount, string url)
        {
            var args = new LazyDownloadArgs(runtime, threadCount);
            args.Wires.Add(new WebRequestHttpWire(url, runtime, line));
            return args;
        }

        public static LazyDownloadArgs CreateWebRequestArgs(IRuntime runtime, int line, int threadCount, Table<ResultRow> table)
        {
            var args = new LazyDownloadArgs(runtime, threadCount);
            foreach (var row in table)
            {
                if (row[0] != null)
                    args.Wires.Add(new WebRequestHttpWire(row[0].ToString(), runtime, line));
            }

            return args;
        }

        public static LazyDownloadArgs CreateSeleniumArgs(IRuntime runtime, int line, int threadCount, string cssElement, int cssTimeout, string url)
        {
            var args = new LazyDownloadArgs(runtime, threadCount);
            args.Wires.Add(new SeleniumHttpWire(url, cssElement, cssTimeout, runtime, line));
            return args;
        }

        public static LazyDownloadArgs CreateSeleniumArgs(IRuntime runtime, int line, int threadCount, string cssElement, int cssTimeout, Table<ResultRow> table)
        {
            var args = new LazyDownloadArgs(runtime, threadCount);
            foreach (var row in table)
                args.Wires.Add(new SeleniumHttpWire(row[0].ToString(), cssElement, cssTimeout, runtime, line));

            return args;
        }


        public static LazyDownloadArgs CreateJavaScriptArgs(IRuntime runtime, int line, int threadCount, string cssElement, int cssTimeout, string url, string js)
        {
            var args = new LazyDownloadArgs(runtime, threadCount);
            args.Wires.Add(new SeleniumExecJsHttpWire(url, cssElement, cssTimeout, runtime, line, js));
            return args;
        }


        public static LazyDownloadArgs CreateJavaScriptArgs(IRuntime runtime, int line, int threadCount, string cssElement, int cssTimeout, Table<ResultRow> table, string js)
        {
            var args = new LazyDownloadArgs(runtime, threadCount);
            foreach (var row in table)
                args.Wires.Add(new SeleniumExecJsHttpWire(row[0].ToString(), cssElement, cssTimeout, runtime, line, js));

            return args;
        }

        private LazyDownloadArgs(IRuntime runtime, int threadCount)
        {
            Runtime = runtime;
            Wires = new List<IHttpWire>();
            ThreadCount = threadCount;
        }

        public IRuntime Runtime { get; private set; }
        public IList<IHttpWire> Wires { get; private set; }
        public int ThreadCount { get; private set; }
    }
}
