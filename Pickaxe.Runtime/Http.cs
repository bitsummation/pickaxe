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

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pickaxe.Runtime
{
    public static class Http
    {
        private static IHttpRequest CreateRequest(IHttpRequestFactory factory, string url)
        {
            return factory.Create(url);
        }

        private static DownloadImage GetImage(IHttpRequestFactory factory, string url)
        {
            var request = CreateRequest(factory, url);
            var bytes = request.Download();

            string extension = Path.GetExtension(url);
            string fileName = Guid.NewGuid().ToString("N") + extension;
            if (bytes.Length == 0)
                fileName = "";

            return new DownloadImage() { date = DateTime.Now, image = bytes, size = bytes.Length, url = url, filename = fileName };
        }

        private static HtmlDocument GetDocument(IHttpRequestFactory factory, string url, out int length)
        {
            var request = CreateRequest(factory, url);
            var bytes = request.Download();

            string html = string.Empty;
            length = bytes.Length;
            html = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        private static RuntimeTable<DownloadPage> DownloadPage(IRuntime runtime, string[] urls)
        {
            var table = new RuntimeTable<DownloadPage>();
            foreach (var url in urls)
            {
                int contentlength;
                var doc = GetDocument(runtime.RequestFactory, url, out contentlength);
                table.Add(new DownloadPage() { url = url, nodes = new[] { doc.DocumentNode }, date = DateTime.Now, size = contentlength });
            }

            return table;
        }

        public static RuntimeTable<DownloadPage> DownloadPage(IRuntime runtime, string url, int line)
        {
            return DownloadPage(runtime, new string[] { url });
        }

        public static RuntimeTable<DownloadImage> DownloadImage(IRuntime runtime, string url, int line)
        {
            var table = new RuntimeTable<DownloadImage>();

            var image = GetImage(runtime.RequestFactory, url);
            
            table.Add(image);
            return table;
        }
    }
}
