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
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Pickaxe.Runtime.Internal
{
    public class HttpRequest
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HttpRequest(string url)
        {
            Url = url;
        }

        protected string Url { get; private set; }
       
        protected virtual bool OnError(DownloadError error)
        {
            Log.InfoFormat("Failed downoad, Url = {0}, Message = {1}", Url, error.Message);
            return false;
        }

        protected virtual void OnSuccess()
        {
            Log.InfoFormat("Download success, Url = {0}", Url);
        }

        protected virtual HttpWebRequest CreateHttpWebRequest()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Timeout = 30000; //30 seconds
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:39.0) Gecko/20100101 Firefox/39.0";

            request.Method = "GET";
            return request;
        }

        private byte[] TryDownload(HttpWebRequest request)
        {
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

        public byte[] Download()
        {
            byte[] bytes = new byte[0];

            while (true)
            {
                try
                {
                    HttpWebRequest request = CreateHttpWebRequest();
                    bytes = TryDownload(request);
                    OnSuccess();
                    break;
                }
                catch (WebException ex)
                {
                    var error = new DownloadError { Status = ex.Status, Message = ex.Message };
                    var response = ex.Response as HttpWebResponse; //this can be null
                    if(response != null)
                        error.HttpStatus = response.StatusCode;

                    if (!OnError(error))
                        break;
                    
                    //Forbidden
                    //NotFound

                    //if (ex.Status == WebExceptionStatus.ConnectFailure) //if we cannot even contact the web server we give up.
                    //  break;
                }
                catch(Exception e)
                {
                    var error = new DownloadError { Message = e.Message };
                    if (!OnError(error))
                        break;
                }
            }

            return bytes;
        }
        
    }
}
