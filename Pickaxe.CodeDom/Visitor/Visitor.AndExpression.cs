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
        private void DoBooleanAggregate(BooleanExpression expression, CodeBinaryOperatorType operation)
        {
            var leftArgs = VisitChild(expression.Left, new CodeDomArg() { Scope = _codeStack.Peek().Scope });
            var rightArgs = VisitChild(expression.Rigth, new CodeDomArg() { Scope = _codeStack.Peek().Scope });

            if (leftArgs.Tag != null)
                _codeStack.Peek().Tag = leftArgs.Tag;
            if (rightArgs.Tag != null)
                _codeStack.Peek().Tag = rightArgs.Tag;

            _codeStack.Peek().CodeExpression = new CodeBinaryOperatorExpression(leftArgs.CodeExpression, operation, rightArgs.CodeExpression);
        }

        public void Visit(AndExpression expression)
        {
            DoBooleanAggregate(expression, CodeBinaryOperatorType.BooleanAnd);
        }
    }
}
