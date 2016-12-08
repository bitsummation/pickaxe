using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime.Dom
{
    public abstract class HtmlDoc
    {
        public abstract void Load(string html);
        public abstract bool ValidateCss(string cssSelector);
        public abstract HtmlElement FirstElement { get; }
        public abstract bool IsEmpty { get; }
    }
}
