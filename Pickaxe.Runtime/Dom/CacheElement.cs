using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Runtime.Dom
{
    public class CacheElement : HtmlElement
    {
        internal CacheElement() : base(new object())
        {
            AttrCache = new Dictionary<string, string>();
        }

        public string Text { get; set; }
        public Dictionary<string, string> AttrCache { get; private set; }
        public string Html { get; set; }

        public override HtmlElement[] QuerySelectorAll(string cssSelector)
        {
            throw new NotImplementedException();
        }

        public override HtmlElement QuerySelector(string cssSelector)
        {
            throw new NotImplementedException();
        }

        public override bool AttributeExists(string attr)
        {
            return AttrCache.ContainsKey(attr);
        }

        internal override string TakeAttribute(string attr)
        {
            if(AttributeExists(attr))
                return AttrCache[attr];

            return null;
        }

        internal override string TakeText()
        {
            return Text;
        }

        internal override string TakeHtml()
        {
            return Html;
        }

        internal override void Clear()
        {
        }
    }
}
