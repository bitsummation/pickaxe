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

using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Runtime.Internal
{
    internal class SeleniumExecJsHttpWire : SeleniumHttpWire
    {
        private string _js;

        public SeleniumExecJsHttpWire(string url, string cssElement, int cssTimeout, IRuntime runtime, int line, string js)
            : base(url, cssElement, cssTimeout, runtime, line)
        {
            _js = js;
        }

        protected override object RunPostDownload(IWebDriver driver)
        {
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

            string script = string.Format(@"

var pickaxe = {{}};

pickaxe.url = arguments[0];
pickaxe.func = function(url) {{
	{0}
}};

return JSON.stringify(pickaxe.func(pickaxe.url));

", _js);

            string json = (string)jsExecutor.ExecuteScript(script, Url);
            return json;
        }
    }
}
