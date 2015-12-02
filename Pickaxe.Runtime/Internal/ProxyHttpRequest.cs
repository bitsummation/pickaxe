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
    internal class ProxyHttpRequest : HttpRequest
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Proxy _proxy;

        public ProxyHttpRequest(Proxy proxy, string url)
            : base(url)
        {
            _proxy = proxy;
        }

        protected override bool OnError(DownloadError error)
        {
            log.InfoFormat("Failed proxy download, Proxy={0}, Url = {1}, Message = {2}", _proxy, Url, error.Message);
            return false;
        }

        protected override HttpWebRequest CreateHttpWebRequest()
        {
            var request = base.CreateHttpWebRequest();

            request.Proxy = new WebProxy(_proxy.ProxyUrl, _proxy.Port);
            return request;
        }
    }
}
