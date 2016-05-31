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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pickaxe.Runtime
{
    public abstract class ThreadedDownloadTable : RuntimeTable<DownloadPage>
    {
        [ThreadStatic]
        public static string LogValue;
        
        private object ResultLock = new object();
        private object UrlLock = new object();
        private Queue<DownloadPage> _results;

        private LazyDownloadArgs _args;

        private bool _running;
        private bool _callOnProgres;

        protected ThreadedDownloadTable(LazyDownloadArgs args)
            : base()
        {
            Wires = new Queue<IHttpWire>();
            _results = new Queue<DownloadPage>();

            _args = args;
            _running = false;

            foreach (var wire in args.Wires)
                Wires.Enqueue(wire);

            _args.Runtime.TotalOperations += args.Wires.Count;
            _callOnProgres = true;            
        }

        protected Queue<IHttpWire> Wires {get; private set;}

        private void Work(string logValue) //multi threaded
        {
            ThreadContext.Properties[Config.LogKey] = logValue;

            IHttpWire wire = null;
            while (true)
            {
                lock (UrlLock)
                {
                    if (Wires.Count > 0)
                        wire = Wires.Dequeue();
                    else
                        wire = null;
                }

                if (wire == null) //nothing left in queue
                    break;

                var downloadResult = Http.DownloadPage(_args.Runtime, wire);
                
                if(_callOnProgres)
                    _args.Runtime.OnProgress();

                lock (ResultLock)
                {
                    foreach(var p in downloadResult)
                        _results.Enqueue(p);
                }
            }
        }

        private void ProcesImpl(string logValue)
        {
            var threads = new List<Thread>();
            for (int x = 0; x < _args.ThreadCount; x++)
                threads.Add(new Thread(() => Work(logValue)));

            _args.Runtime.DownloadThreads.Clear();
            foreach (var thread in threads)
            {
                _args.Runtime.DownloadThreads.Add(thread);
                thread.Start();
            }
        }

        private DownloadPage FetchResult()
        {
            DownloadPage result = null;
            lock (ResultLock)
            {
                if (_results.Count > 0)
                    result = _results.Dequeue();
            }

            return result;
        }

        public DownloadPage GetResult()
        {
            DownloadPage result = null;
            while (result == null)
                result = FetchResult();
         
            return result;
        }

        public void Process()
        {
            if (!_running)
            {
                _running = true;
                var logValue = ThreadedDownloadTable.LogValue;
                Thread thread = new Thread(() => ProcesImpl(logValue));

                thread.Start();
            }
        }
    }
}
