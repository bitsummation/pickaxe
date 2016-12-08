using Pickaxe.Runtime.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime.AgilityPackFizzler
{
    internal class AgilityFactory : DomFactory
    {
        public override HtmlDoc Create()
        {
            return new AgilityDoc();
        }
    }
}
