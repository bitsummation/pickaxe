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
    internal abstract class HttpRequestFactory : IHttpRequestFactory
    {
        public static IHttpRequestFactory NoProxy = new NoProxyFactoryImpl();

        public abstract IHttpRequest Create(string url);

        public static IHttpRequestFactory CreateProxyFactory(Proxy proxy)
        {
            return new ProxyFactoryImpl(proxy);
        }

        public static IHttpRequestFactory CreateProxySelector(ProxySelector selector)
        {
            return new ProxySelectorImpl(selector);
        }

        private class NoProxyFactoryImpl : HttpRequestFactory
        {
            public override IHttpRequest Create(string url)
            {
                return new RetryHttpRequest(url);
            }
        }

        private class ProxySelectorImpl : HttpRequestFactory
        {
            private ProxySelector _selector;

            public ProxySelectorImpl(ProxySelector selector)
            {
                _selector = selector;
            }

            public override IHttpRequest Create(string url)
            {
                return new ProxyHttpRequestSelector(_selector, url);
            }
        }

        private class ProxyFactoryImpl : HttpRequestFactory
        {
            private Proxy _proxy;

            public ProxyFactoryImpl(Proxy proxy)
            {
                _proxy = proxy;
            }

            public override IHttpRequest Create(string url)
            {
                return new ProxyHttpRequest(_proxy, url);
            }
        }
    }
}
