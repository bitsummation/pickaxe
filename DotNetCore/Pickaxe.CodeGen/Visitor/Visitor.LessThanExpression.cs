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

using Pickaxe.Runtime;
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

        private CodeExpression DoBoolean(CodeDomArg leftArgs, CodeDomArg rightArgs, CodeBinaryOperatorType operation)
        {
            Type leftType = null;
            if (leftArgs.Scope != null)
                leftType = leftArgs.Scope.CodeDomReference.GenerateType();

            Type rightType = null;
            if (rightArgs.Scope != null)
                rightType = rightArgs.Scope.CodeDomReference.GenerateType();

            if (leftType != null && rightType != null && leftType != rightType)
            {
                if (leftType == typeof(string) || leftType == typeof(object))
                {
                    var primitive = TablePrimitive.FromType(rightType);
                    leftArgs.CodeExpression = primitive.ToNative(leftArgs.CodeExpression);
                }
                else if (rightType == typeof(string))
                {
                    var primitive = TablePrimitive.FromType(leftType);
                    rightArgs.CodeExpression = primitive.ToNative(rightArgs.CodeExpression);
                }
            }

            if (leftArgs.Tag != null)
                _codeStack.Peek().Tag = leftArgs.Tag;
            if (rightArgs.Tag != null)
                _codeStack.Peek().Tag = rightArgs.Tag;

            return new CodeBinaryOperatorExpression(leftArgs.CodeExpression, operation, rightArgs.CodeExpression);
        }

        private void DoBoolean(BooleanExpression expression, CodeBinaryOperatorType operation)
        {
            var leftArgs = VisitChild(expression.Left, new CodeDomArg() { Scope = _codeStack.Peek().Scope });
            var rightArgs = VisitChild(expression.Right, new CodeDomArg() { Scope = _codeStack.Peek().Scope });

            _codeStack.Peek().CodeExpression = DoBoolean(leftArgs, rightArgs, operation);
        }

        public void Visit(LessThanExpression expression)
        {
            DoBoolean(expression, CodeBinaryOperatorType.LessThan);
        }
    }
}
