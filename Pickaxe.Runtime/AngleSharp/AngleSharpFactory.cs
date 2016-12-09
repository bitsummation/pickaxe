using Pickaxe.Runtime.AgilityPackFizzler;
using Pickaxe.Runtime.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Runtime.AngleSharp
{
    internal class AngleSharpFactory : DomFactory
    {
        public override HtmlDoc Create()
        {
            return new AngleSharpDoc();
        }
    }
}
