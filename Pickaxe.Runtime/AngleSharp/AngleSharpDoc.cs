using Pickaxe.Runtime.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime.AngleSharp
{
    internal class AngleSharpDoc : HtmlDoc
    {
        public override HtmlElement FirstElement
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsEmpty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Load(string html)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateCss(string cssSelector)
        {
            throw new NotImplementedException();
        }
    }
}
