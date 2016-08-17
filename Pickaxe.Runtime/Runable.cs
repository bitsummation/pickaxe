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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pickaxe.Runtime
{
    public class Runable
    {
        private RuntimeBase _instance;
        private Type _runType;

        public event Action<RuntimeTable<ResultRow>> Select;
        public event Action<ProgressArgs> Progress;
        public event Action<int> Highlight;

        public Runable(Assembly assembly) : this(assembly, new string[0])
        { }

        public Runable(Assembly assembly, string[] args)
        {
            var constructor = assembly.GetType("Code").GetConstructor(new Type[] { typeof(string[]) });
            _instance = constructor.Invoke(new object[] { args }) as RuntimeBase;
            _instance.Progress += OnProgress;
            _instance.Select += OnSelect;
            _instance.Highlight += OnHighlight;

            _runType = assembly.GetType("Code");
        }

        public void SetRequestFactory(IHttpRequestFactory requestFactory)
        {
            _instance.RequestFactory = requestFactory;
        }

        public void Stop(Action action)
        {
            _instance.Stop(action);
        }

        private void OnSelect(RuntimeTable<ResultRow> results)
        {
            if (Select != null)
                Select(results);
        }

        private void OnProgress(ProgressArgs args)
        {
            if (Progress != null)
                Progress(args);
        }

        private void OnHighlight(int line)
        {
            if (Highlight != null)
                Highlight(line);
        }

        public void Run()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            var cultureInfo = new CultureInfo("en-US");
            cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss.fff";
            cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            _instance.ExecutingThread = Thread.CurrentThread;
            var method = _runType.GetMethod("Run");
            method.Invoke(_instance, null);
        }
    }
}
