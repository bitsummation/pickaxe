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
using System.Net;
using System.Reflection;
using System.Text;

namespace Pickaxe.Runtime.Internal
{
    internal abstract class HttpRequest : IHttpRequest
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected HttpRequest(IHttpWire wire)
        {
            Wire = wire;
        }

        protected IHttpWire Wire { get; private set; }

        protected abstract bool OnError(DownloadError error);

        protected virtual void OnBeforeDownload() { }
        
        protected virtual void OnSuccess()
        {
            Log.InfoFormat("Download success, Url = {0}", Wire.Url);
        }

        public object Download()
        {
            object bytes = new byte[0];

            while (true)
            {
                try
                {
                    OnBeforeDownload();
                    bytes = Wire.Download();
                    
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
