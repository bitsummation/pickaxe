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
using Pickaxe.Runtime.Dom;
using System;
using Fizzler.Systems.HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime.AgilityPackFizzler
{
    internal class AgilityDoc : HtmlDoc
    {
        private HtmlDocument _doc;
     
        public override HtmlElement FirstElement
        {
            get
            {
                return new AgilityElement(_doc.DocumentNode);
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return (string.IsNullOrEmpty(_doc.DocumentNode.InnerText)); //no nodes in root
            }
        }

        public override bool ValidateCss(string cssSelector)
        {
            bool valid = true;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<html></html>");
            try
            {
                doc.DocumentNode.QuerySelector(cssSelector);
            }
            catch (Exception)
            {
                valid = false;
            }

            return valid;
        }

        public override void Load(string html)
        {
            _doc = new HtmlDocument();
            _doc.LoadHtml(html);
        }
    }
}
