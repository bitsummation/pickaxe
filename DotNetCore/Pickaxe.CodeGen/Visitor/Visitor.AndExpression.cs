/* Copyright 2015 Brock Reeve
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
            var rightArgs = VisitChild(expression.Right, new CodeDomArg() { Scope = _codeStack.Peek().Scope });

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
