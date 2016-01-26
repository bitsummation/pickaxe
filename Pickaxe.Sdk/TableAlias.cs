using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Sdk
{
    public class TableAlias : VariableReferance
    {
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
