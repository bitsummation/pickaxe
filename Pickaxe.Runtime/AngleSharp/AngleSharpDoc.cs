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
using AngleSharp.Parser.Html;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp.Dom.Html;

namespace Pickaxe.Runtime.AngleSharp
{
    internal class AngleSharpDoc : HtmlDoc
    {
        private IHtmlDocument _doc;

        public override HtmlElement FirstElement
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
            _doc = parser.Parse(html);
        }

        public override bool ValidateCss(string cssSelector)
        {
            bool valid = true;
            var parser = new HtmlParser();
            var doc = parser.Parse("<html></html>");

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
