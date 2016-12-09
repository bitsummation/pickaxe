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
                return new AngleSharpElement(_doc.DocumentElement);
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
