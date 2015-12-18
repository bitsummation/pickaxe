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
using System.Threading.Tasks;
using Pickaxe.Runtime.Debug;
using Pickaxe.Runtime.Internal;
using System.Threading;
using log4net;
using System.Reflection;

namespace Pickaxe.Runtime
{
    public abstract class RuntimeBase : IDebug, IBreak, IRuntime
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private BreakProcesser _processor;

        protected RuntimeBase()
        {
            TotalOperations = 0;
            CompletedOperations = 0;
            HighlightExecution = false;
            IsRunning = true;
            _processor = BreakProcesser.Continue;
            RequestFactory = HttpRequestFactory.NoProxy;
        }

        public event Action<ProgressArgs> Progress;
        public event Action<RuntimeTable<ResultRow>> Select;
        public event Action<int> Highlight;

        public IHttpRequestFactory RequestFactory { get; set; }
        public int TotalOperations { get; set; }
        public bool  IsRunning { get; private set; }
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
            CompletedOperations++;
            OnProgress(new ProgressArgs(CompletedOperations, TotalOperations));
        }

        protected void OnProgress(ProgressArgs args)
        {
            if (Progress != null)
                Progress(args);
        }

        public void Stop()
        {
            IsRunning = false;
            Log.Info("Program stopping......");
        }

        public bool HighlightExecution { get; set; }

        public void Call(int line)
        {
            if (!IsRunning)
            {
                Log.Info("Program Stopped");
                Thread.CurrentThread.Abort();
            }

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
