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


using Pickaxe.Runtime.Dom;
using System;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Pickaxe.Runtime.AngleSharp
{
    internal class AngleSharpDoc : HtmlDoc
    {
        private IHtmlDocument _doc;

        public override Dom.HtmlElement FirstElement
        {
            get
            {
                return new AngleSharpElement(_doc.DocumentElement, null);
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(_doc.DocumentElement.TextContent);
            }
        }

        public override void Load(string html)
        {
            var parser = new HtmlParser();
            _doc = parser.ParseDocument(html);
        }

        public override bool ValidateCss(string cssSelector)
        {
            bool valid = true;
            var parser = new HtmlParser();
            var doc = parser.ParseDocument("<html></html>");

            try
            {
                doc.DocumentElement.QuerySelector(cssSelector);
            }
            catch (Exception)
            {
                valid = false;
            }

            return valid;
        }
    }
}
