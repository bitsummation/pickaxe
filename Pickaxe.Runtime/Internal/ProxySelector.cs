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
        private Queue<Proxy> _proxies { get; set;}

        public ProxySelector()
        {
            _proxies = new Queue<Proxy>();
        }

        public int ProxyCount
        {
            get
            {
                return _proxies.Count;
            }
        }

        public void Add(Proxy proxy)
        {
            _proxies.Enqueue(proxy);
        }

        public void Remove(Proxy proxy)
        {
            var list = _proxies.ToList();
            list.Remove(proxy);
            _proxies.Clear();
            foreach (var p in list)
                _proxies.Enqueue(p);
        }

        public Proxy Current
        {
            get
            {
                return _proxies.Last();
            }
        }

        public Proxy Next
        {
            get
            {
                var proxy = _proxies.Dequeue();
                _proxies.Enqueue(proxy);
                return proxy;
            }
        }

    }
}
