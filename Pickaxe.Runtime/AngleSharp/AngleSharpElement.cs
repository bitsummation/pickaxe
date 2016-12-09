using AngleSharp.Dom;
using Pickaxe.Runtime.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime.AngleSharp
{
    internal class AngleSharpElement : HtmlElement
    {
        internal AngleSharpElement(IElement element) : base(element)
        { }

        internal IElement Element
        {
            get
            {
                return Handle as IElement;
            }
        }

        public override bool AttributeExists(string attr)
        {
            return Element.Attributes[attr] != null;
        }

        public override HtmlElement QuerySelector(string cssSelector)
        {
            var node = Element.QuerySelector(cssSelector);
            if (node != null)
                return new AngleSharpElement(node);

            return null;
        }

        public override HtmlElement[] QuerySelectorAll(string cssSelector)
        {
            var list = new List<HtmlElement>();

            foreach (var e in Element.QuerySelectorAll(cssSelector).ToArray())
                list.Add(new AngleSharpElement(e));

            return list.ToArray();
        }

        internal override string TakeAttribute(string attr)
        {
            if (AttributeExists(attr))
                return Element.Attributes[attr].Value;

            return null;
        }

        internal override string TakeHtml()
        {
            return Element.InnerHtml;
        }

        internal override string TakeText()
        {
            return Element.TextContent;
        }
    }
}
