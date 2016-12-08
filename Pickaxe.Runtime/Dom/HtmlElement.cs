using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime.Dom
{
    public abstract class HtmlElement
    {
        internal HtmlElement(object handle)
        {
            if (handle == null)
                throw new InvalidOperationException("handle should not be null");

            Handle = handle;
        }

        internal object Handle { get; set; }

        public abstract HtmlElement[] QuerySelectorAll(string cssSelector);
        public abstract HtmlElement QuerySelector(string cssSelector);

        public abstract bool AttributeExists(string attr);
        internal abstract string TakeAttribute(string attr);
        internal abstract string TakeText();
        internal abstract string TakeHtml();
    }
}
