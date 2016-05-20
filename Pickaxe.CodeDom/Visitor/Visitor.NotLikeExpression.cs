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


using System.CodeDom;
using Pickaxe.Sdk;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(NotLikeExpression op)
        {
            var leftArgs = VisitChild(op.Left);
            var rightArgs = VisitChild(op.Right);

            _codeStack.Peek().CodeExpression = new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(leftArgs.CodeExpression, "Contains", rightArgs.CodeExpression), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false));
        }
    }
}
