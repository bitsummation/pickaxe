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

namespace Pickaxe.Runtime
{
    public class DynamicObjectDownloadTable : ThreadedDownloadTable<DynamicObject>
    {
        public DynamicObjectDownloadTable(LazyDownloadArgs args)
            : base(args)

        {
        }

        public sealed override IEnumerator<DynamicObject> GetEnumerator() //Give out empty lazy wrappers
        {
            Process();
            while (true)
            {
                if (FinishedDownloading)
                {
                    while (ResultCount > 0)
                    {
                        DecrementResultCount();
                        yield return new DynamicObjectWrapper(this);
                    }

                    break;
                }
                else
                {
                    while (!FinishedDownloading && ResultCount == 0) //spin wait
                    { }
                    if (FinishedDownloading)
                        continue;
                       
                    DecrementResultCount();
                    yield return new DynamicObjectWrapper(this);
                }
            }
        }

        protected override RuntimeTable<DynamicObject> Fetch(IRuntime runtime, IHttpWire wire)
        {
            return Http.DownloadJSPage(runtime, wire);
        }
    }
}
