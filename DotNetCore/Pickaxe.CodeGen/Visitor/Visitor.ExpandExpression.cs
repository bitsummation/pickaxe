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
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(ExpandExpression expression)
        {
            var fromDomArgs = VisitChild(expression.From);
            var toDomArgs = VisitChild(expression.To);
            var expressionArgs = VisitChild(expression.Expression);

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Expand_" + fromDomArgs.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = new CodeTypeReference("Table", new CodeTypeReference("Expand"));

            method.Statements.Add(new CodeVariableDeclarationStatement(method.ReturnType, "expandTable",
                new CodeObjectCreateExpression(new CodeTypeReference("RuntimeTable", new CodeTypeReference("Expand")))));
            
            method.Statements.Add(new CodeVariableDeclarationStatement(typeof(int?), "x"));

            var loop = new CodeIterationStatement();
            loop.InitStatement =  new CodeAssignStatement( new CodeVariableReferenceExpression("x"), fromDomArgs.CodeExpression);
            loop.TestExpression = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("x"),
                CodeBinaryOperatorType.LessThanOrEqual, toDomArgs.CodeExpression);
            loop.IncrementStatement = new CodeAssignStatement(new CodeVariableReferenceExpression("x"), new CodeBinaryOperatorExpression(
                new CodeVariableReferenceExpression("x"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));

            loop.Statements.Add(new CodeVariableDeclarationStatement("Expand", "expand", new CodeObjectCreateExpression("Expand")));
            loop.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("expand"), "value"),
                expressionArgs.CodeExpression));

            loop.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("expandTable"), "Add",
                new CodeVariableReferenceExpression("expand")));

            method.Statements.Add(loop);

            method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("expandTable")));

            _mainType.Type.Members.Add(method);
            var methodcall = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(null, method.Name));

            _codeStack.Peek().CodeExpression = methodcall;
            _codeStack.Peek().Scope = new ScopeData<TableDescriptor> { Type = Expand.Columns, CodeDomReference = method.ReturnType };
        }
    }
}
