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
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(EachStatement eachStatement)
        {
            using (Scope.Push(_mainType))
            {
                var iterationArg = VisitChild(eachStatement.IterationVariable);
                var tableArg = VisitChild(eachStatement.TableReference);
                var rowType = iterationArg.Scope.CodeDomReference;

                CodeMemberMethod method = new CodeMemberMethod();
                method.Name = "Each_" + iterationArg.MethodIdentifier;
                method.Attributes = MemberAttributes.Private;
                GenerateCallStatement(method.Statements, eachStatement.Line.Line);

                var progressInc = new CodeAssignStatement(new CodePropertyReferenceExpression(null, "TotalOperations"),
                    new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(null, "TotalOperations"),
                    CodeBinaryOperatorType.Add,
                    new CodePropertyReferenceExpression(tableArg.CodeExpression, "RowCount"))
                    );

                method.Statements.Add(progressInc);
                method.Statements.Add(
                new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerator", rowType),
                    "x",
                new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(tableArg.CodeExpression, "GetEnumerator",
                        null)))
                        );

                var loop = new CodeIterationStatement();
                loop.InitStatement = new CodeSnippetStatement();
                loop.IncrementStatement = new CodeSnippetStatement();
                loop.TestExpression = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("x"), "MoveNext",
                    null));

                var rowReference = VisitChild(new TableVariableRowReference() { Id = eachStatement.IterationVariable.Variable });
                loop.Statements.Add(new CodeAssignStatement(rowReference.CodeExpression, new CodePropertyReferenceExpression(new CodeTypeReferenceExpression("x"), "Current")));

                var blockArgs = VisitChild(eachStatement.Block, new CodeDomArg() { Tag = true });
                loop.Statements.AddRange(blockArgs.ParentStatements);
                CallOnProgress(loop.Statements, false);

                method.Statements.Add(loop);
                var methodcall = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(null, method.Name));

                _mainType.Type.Members.Add(method);
                _codeStack.Peek().ParentStatements.Add(methodcall);
                _codeStack.Peek().CodeExpression = methodcall;
            }
        }
    }
}
