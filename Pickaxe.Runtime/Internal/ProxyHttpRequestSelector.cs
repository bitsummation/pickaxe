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
using System.Net;
using System.Reflection;
using System.Text;

namespace Pickaxe.Runtime.Internal
{
    internal class ProxyHttpRequestSelector : HttpRequest
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int RemoveProxyCount = 10;

        private HttpProxySelector _selectorCopy;
        private ProxySelector _selector;
        private Stack<AttemptPair> _error;

        public ProxyHttpRequestSelector(ProxySelector selector, string url)
            : base(url)
        {
            _selectorCopy = new HttpProxySelector(selector.FetchProxies());
            _error = new Stack<AttemptPair>();
        }

        protected override bool OnError(DownloadError error)
        {
            Log.InfoFormat("Download error, Proxy={0}, Url = {1}, Message = {2}", _selectorCopy.Current, Url, error.Message);

            bool tryAgain = true;
            _error.Push(new AttemptPair() { Proxy = _selectorCopy.Current, Error = error });

            if (_error.Count >= _selectorCopy.ProxyCount) //We went through all the proxies and they all had an error. We give up.
            {
                _error.Clear();
                tryAgain = false;
            }

            return tryAgain;
        }

        protected override void OnSuccess()
        {
            Log.InfoFormat("Download success, Proxy={0}, Url = {1}", _selectorCopy.Current, Url);

            if (_error.Count > 0)
            {
                //If there are errors and this one is successful then we remove those errors and proxies
                foreach (var attempt in _error)
                {
                    attempt.Proxy.IncrementErrorCount();
                    Log.InfoFormat("Proxy Error Incremeneted, Proxy={0}, ErrorCount = {1}", attempt.Proxy, attempt.Proxy.ErrorCount);
                    if (attempt.Proxy.ErrorCount >= RemoveProxyCount)
                    {
                        Log.InfoFormat("Proxy removed, Proxy={0}", attempt.Proxy);
                        _selector.Remove(attempt.Proxy); //remove bad eggs.
                    }
                }

                Log.InfoFormat("Proxies count remaining, Count={0}", _selector.ProxyCount);
            }

            _error.Clear();
        }

        protected override HttpWebRequest CreateHttpWebRequest()
        {
            var request = base.CreateHttpWebRequest();
            var proxy = _selectorCopy.Next;

            request.Proxy = new WebProxy(proxy.ProxyUrl, proxy.Port);
            return request;
        }

        private class AttemptPair
        {
            public Proxy Proxy {get; set;}
            public DownloadError Error { get; set; }
        }
    }
}
