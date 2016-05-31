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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Pickaxe.Runtime.Internal
{
    internal class WebRequestHttpWire : HttpWire
    {
        public WebRequestHttpWire(string url, IRuntime runtime, int line)
            : base(url, runtime, line)
        {
        }

        private HttpWebRequest CreateHttpWebRequest()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Timeout = 30000; //30 seconds
            request.UserAgent = "pickaxe/1.0";

            request.Method = "GET";
            if(Proxy != null)
                request.Proxy = new WebProxy(Proxy.ProxyUrl, Proxy.Port);

            return request;
        }

        protected override byte[] DownloadImpl()
        {
            var request = CreateHttpWebRequest();
            byte[] bytes = new byte[0];

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = response.GetResponseStream())
            using (var memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                do
                {
                    bytesRead = reader.Read(buffer, 0, buffer.Length);
                    memoryStream.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);

                bytes = memoryStream.ToArray();
            }

            return bytes;
        }

    }
}
