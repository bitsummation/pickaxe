using Pickaxe.Sdk;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(OrExpression expression)
        {
            DoBooleanAggregate(expression, CodeBinaryOperatorType.BooleanOr);
        }
    }
}
