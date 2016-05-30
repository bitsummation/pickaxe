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
using Pickaxe.Runtime.Debug;
using Pickaxe.Runtime.Internal;
using System.Threading;
using log4net;
using System.Reflection;
using System.Collections.Generic;

namespace Pickaxe.Runtime
{
    public abstract class RuntimeBase : IDebug, IBreak, IRuntime
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private object ProgressLock = new object();

        private BreakProcesser _processor;
        private Dictionary<string, string> _args;

        protected RuntimeBase(string[] args)
        {
            TotalOperations = 0;
            CompletedOperations = 0;
            HighlightExecution = false;
            _processor = BreakProcesser.Continue;
            RegisterArgs(args);
            RequestFactory = HttpRequestFactory.NoProxy;

            DownloadThreads = new List<Thread>();
        }

        public Thread ExecutingThread { get; set; }
        public IList<Thread> DownloadThreads { get; set;}

        public event Action<ProgressArgs> Progress;
        public event Action<RuntimeTable<ResultRow>> Select;
        public event Action<int> Highlight;

        public IHttpRequestFactory RequestFactory { get; set; }
        public int TotalOperations { get; set; }
        protected int CompletedOperations { get; set; }

        protected void InitProxies()
        {
            TotalOperations += Proxies.Length;

            if (Proxies.Length > 0)
            {
                var selector = new ProxySelector();

                foreach (var proxyString in Proxies)
                {
                    var proxy = Proxy.FromString(proxyString);
                    RequestFactory = HttpRequestFactory.CreateProxyFactory(proxy);
                    if (TestProxy())
                    {
                        Log.InfoFormat("Proxy added, Proxy={0}", proxy);
                        selector.Add(proxy);
                    }
                    else
                        Log.InfoFormat("Proxy failed, Proxy={0}", proxy);

                    OnProgress();
                }

                if (selector.ProxyCount == 0)
                {
                    Log.Info("No poxies pass test. Fail.");
                    Thread.CurrentThread.Abort();
                }

                RequestFactory = HttpRequestFactory.CreateProxySelector(selector);
            }
        }

        public string this[string key]
        {
            get
            {
                if (!_args.ContainsKey(key))
                    return null;

                return _args[key];
            }
            set
            {
                _args[key] = value;
            }
        }

        private void RegisterArgs(string[] args)
        {
            _args = new Dictionary<string, string>();
            for(int x = 0; x < args.Length; x++)
            {
                var key = string.Format("@{0}", x + 1);
                _args.Add(key, args[x]);
            }
        }

        protected virtual string[] Proxies
        {
            get
            {
                return new string[0];
            }
        }

        protected virtual bool TestProxy()
        {
            return true;
        }

        protected void OnSelect(RuntimeTable<ResultRow> results)
        {
            if (Select != null)
                Select(results);
        }

        protected void OnHighlight(int line)
        {
            if (Highlight != null)
                Highlight(line);
        }

        public void OnProgress()
        {
            lock (ProgressLock)
            {
                CompletedOperations++;
            }

            OnProgress(new ProgressArgs(CompletedOperations, TotalOperations));
        }

        protected void OnProgress(ProgressArgs args)
        {
            if (Progress != null)
                Progress(args);
        }

        private void StopImpl(Action action)
        {
            Log.Info("Program stopping......");

            ExecutingThread.Abort();

            foreach (var thread in DownloadThreads)
                thread.Abort();

            //We need to wait here until all download threads are fully killed.
            foreach (var thread in DownloadThreads)
                thread.Join(); //wait for all workers to stop

            if (action != null)
                action();
        }

        public void Stop(Action action)
        {
            if (action != null)
            {
                var thread = new Thread(() => StopImpl(action));
                thread.Start();
            }
            else
                StopImpl(action);
        }

        public bool HighlightExecution { get; set; }

        public void Call(int line)
        {
            if(HighlightExecution)
                OnHighlight(line);

            _processor.Call(line, this);
        }

        void IBreak.Break(int line)
        {
            OnHighlight(line);
        }

    }
}
