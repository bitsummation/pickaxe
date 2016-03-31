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

namespace Pickaxe.Runtime.Internal
{
    internal class ProxySelector
    {
        private object Lock = new object();

        private int _fetchedProxyArrayCount;

        public ProxySelector()
        {
            Proxies = new Queue<Proxy>();
            _fetchedProxyArrayCount = 0;
        }

        protected ProxySelector(IEnumerable<Proxy> proxies)
        {
            Proxies = new Queue<Proxy>(proxies);
        }

        protected Queue<Proxy> Proxies {get; private set; }

        //Copy of the array for each url request
        public IEnumerable<Proxy> FetchProxies()
        {
            lock (Lock)
            {
                _fetchedProxyArrayCount++;
                int startIndex = _fetchedProxyArrayCount % ProxyCount;
                var list = Proxies.ToList();
                while(startIndex > 0)
                {
                    var element = list[0];
                    list.RemoveAt(0);
                    list.Add(element);
                    startIndex--;
                }
                //Need to change up the order
                return list;
            }
        }

        public int ProxyCount
        {
            get
            {
                return Proxies.Count;
            }
        }

        public void Add(Proxy proxy)
        {
            Proxies.Enqueue(proxy);
        }

        public void Remove(Proxy proxy)
        {
            lock (Lock)
            {
                var list = Proxies.ToList();
                list.Remove(proxy);
                Proxies.Clear();
                foreach (var p in list)
                    Proxies.Enqueue(p);
            }
        }

    }
}
