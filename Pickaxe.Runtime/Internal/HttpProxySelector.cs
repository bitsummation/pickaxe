using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime.Internal
{
    internal class HttpProxySelector : ProxySelector
    {
        public HttpProxySelector(IEnumerable<Proxy> proxies)
            : base(proxies)
        {
        }

        public Proxy Current
        {
            get
            {
                return Proxies.Last();
            }
        }

        public Proxy Next
        {
            get
            {
                var proxy = Proxies.Dequeue();
                Proxies.Enqueue(proxy);
                return proxy;
            }
        }

    }
}
