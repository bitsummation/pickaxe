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

using Pickaxe.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Pickaxe.Runtime
{
    public class Proxy
    {
        private Proxy(string proxyUrl, int port)
        {
            ProxyUrl = proxyUrl;
            Port = port;
            ErrorCount = 0;
        }

        public static bool TryParse(string proxy)
        {
            int port;

            var splits = Regex.Split(proxy, ":");
            if (splits.Length < 2)
                return false;
            if (!int.TryParse(splits[1], out port))
                return false;

            return true;
        }
        
        public static Proxy FromString(string proxy)
        {
            var splits = Regex.Split(proxy, ":");
            return new Proxy(splits[0], int.Parse(splits[1]));
        }

        public string ProxyUrl { get; private set; }
        public int Port { get; private set; }
        public int ErrorCount { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", ProxyUrl, Port);
        }
    }
}
