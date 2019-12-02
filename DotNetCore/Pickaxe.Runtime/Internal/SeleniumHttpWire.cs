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
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Text;

namespace Pickaxe.Runtime.Internal
{
    internal class SeleniumHttpWire : HttpWire
    {
        private string _cssElement;
        private int _cssTimeout;

        public SeleniumHttpWire(string url, string cssElement, int cssTimeout, IRuntime runtime, int line)
            : base(url, runtime, line)
        {
            _cssElement = cssElement;
            _cssTimeout = cssTimeout;
        }

        protected virtual object RunPostDownload(IWebDriver driver)
        {
            return null;
        }

        protected override object DownloadImpl()
        {
            object bytes = new byte[0];

            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.SuppressInitialDiagnosticInformation = true;
            driverService.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("no-sandbox");
            options.AddArgument("start-maximized");
            options.AddArgument("disable-extensions");

            if (Proxy != null)
            {
                options.AddArgument(string.Format("proxy-server={0}", Proxy));
            }

            ChromeDriver driver = null;
            try
            {
                driver = new ChromeDriver(driverService, options);

                driver.Navigate().GoToUrl(new Uri(Url));

                if (!String.IsNullOrEmpty(_cssElement))
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(_cssTimeout));
                    wait.Message = "Couldn't find element in page";
                    wait.Until(drv => drv.FindElement(By.CssSelector(_cssElement)));
                }

                var postObject = RunPostDownload(driver);

                if (postObject != null)
                {
                    bytes = postObject;
                }
                else
                {
                    string html = driver.PageSource;
                    bytes = Encoding.UTF8.GetBytes(html);
                }
            }
            finally
            {
                if (driver != null)
                    driver.Quit();
            }

            return bytes;
        }

    }
}
