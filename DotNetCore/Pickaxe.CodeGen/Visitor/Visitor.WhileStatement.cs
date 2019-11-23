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
        public void Visit(WhileStatement whileStatement)
        {
            /*using (Scope.Push(_mainType))
            {
                var tableArg = VisitChild(whileStatement.TableReference);

                CodeMemberMethod method = new CodeMemberMethod();
                method.Name = "While_" + tableArg.MethodIdentifier;
                method.Attributes = MemberAttributes.Private;
                GenerateCallStatement(method.Statements, whileStatement.Line.Line);

                var loop = new CodeIterationStatement();
                loop.InitStatement = new CodeSnippetStatement();
                loop.IncrementStatement = new CodeSnippetStatement();
                loop.TestExpression = new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(tableArg.CodeExpression, "RowCount"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0));

                method.Statements.Add(loop);

                var blockArgs = VisitChild(whileStatement.Block, new CodeDomArg() { Tag = true });
                loop.Statements.AddRange(blockArgs.ParentStatements);

                var methodcall = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(null, method.Name));

                _mainType.Type.Members.Add(method);
                _codeStack.Peek().ParentStatements.Add(methodcall);
                _codeStack.Peek().CodeExpression = methodcall;
            }*/
        }
    }
}
