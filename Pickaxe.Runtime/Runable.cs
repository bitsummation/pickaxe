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
using System.Reflection;
using System.Text;
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

        public Runable(Assembly assembly)
        {
            _instance = assembly.CreateInstance("Code") as RuntimeBase;
            _instance.Progress += OnProgress;
            _instance.Select += OnSelect;
            _instance.Highlight += OnHighlight;

            _runType = assembly.GetType("Code");
        }

        public void Stop()
        {
            _instance.Stop();
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
            var method = _runType.GetMethod("Run");
            method.Invoke(_instance, null);
        }
    }
}
