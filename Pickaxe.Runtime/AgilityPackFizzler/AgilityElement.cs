using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Pickaxe.Runtime.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime.AgilityPackFizzler
{
    internal class AgilityElement : HtmlElement
    {
        internal AgilityElement(HtmlNode handle) : base(handle)
        {
        }

        internal HtmlNode Element
        {
            get
            {
                return Handle as HtmlNode;
            }
        }

        public override HtmlElement QuerySelector(string cssSelector)
        {
            var node = Element.QuerySelector(cssSelector);
            if (node != null)
                return new AgilityElement(node);

            return null;
        }

        public override HtmlElement[] QuerySelectorAll(string cssSelector)
        {
            var list = new List<HtmlElement>();

            foreach (var e in Element.QuerySelectorAll(cssSelector).ToArray())
                list.Add(new AgilityElement(e));

            return list.ToArray();
        }

        public override bool AttributeExists(string attr)
        {
            return Element.Attributes[attr] != null;
        }

        internal override string TakeAttribute(string attr)
        {
            if(AttributeExists(attr))
                return Element.Attributes[attr].Value;

            return null;
        }

        internal override string TakeHtml()
        {
            return Element.InnerHtml;
        }

        internal override string TakeText()
        {
            return Element.InnerText;
        }
    }
}
