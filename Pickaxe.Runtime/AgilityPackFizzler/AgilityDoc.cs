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
