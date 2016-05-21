using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Sdk
{
    public class NotLikeExpression : BooleanExpression
    {
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
