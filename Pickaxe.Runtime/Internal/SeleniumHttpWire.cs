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
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime.Internal
{
    internal class SeleniumHttpWire : HttpWire
    {
        public SeleniumHttpWire(string url)
            : base(url)
        {
        }

        public override byte[] Download()
        {
            byte[] bytes = new byte[0];

            //http://stackoverflow.com/questions/18921099/add-proxy-to-phantomjsdriver-selenium-c
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            using (PhantomJSDriver phantom = new PhantomJSDriver(driverService))
            {
                phantom.Navigate().GoToUrl(Url);

                var wait = new WebDriverWait(phantom, TimeSpan.FromSeconds(15));
                wait.Message = "DOM didn't load";
                wait.Until(driver1 => ((IJavaScriptExecutor)phantom).ExecuteScript("return document.readyState").Equals("complete"));

                wait = new WebDriverWait(phantom, TimeSpan.FromSeconds(15));
                wait.Message = "Couldn't find element in page";
                wait.Until(drv => drv.FindElement(By.CssSelector(".pricecontainer")));

                string html = phantom.PageSource;

                bytes = Encoding.UTF8.GetBytes(html);
            }

            return bytes;
        }

    }
}
