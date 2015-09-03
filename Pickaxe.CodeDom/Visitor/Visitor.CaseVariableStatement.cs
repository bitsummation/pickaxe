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
        public void Visit(CaseVariableStatement statement)
        {
            var domArg = new CodeDomArg();

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Case_" + domArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = new CodeTypeReference(typeof(object));
            GenerateCallStatement(method.Statements, statement.Line.Line);

            var caseArgs = VisitChild(statement.Case, new CodeDomArg() { Scope = _codeStack.Peek().Scope });
            if (caseArgs.Tag != null)
                _codeStack.Peek().Tag = caseArgs.Tag;

            method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(object)), "var", caseArgs.CodeExpression));

            foreach (var childArg in statement.BooleanStatements)
            {
                domArg = VisitChild(childArg, new CodeDomArg() { Scope = _codeStack.Peek().Scope });
                if (domArg.Tag != null)
                    _codeStack.Peek().Tag = domArg.Tag;

                method.Statements.AddRange(domArg.ParentStatements);
            }

            if (statement.ElseStatement == null)
                method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));
            else
            {
                domArg = VisitChild(statement.ElseStatement, new CodeDomArg() { Scope = _codeStack.Peek().Scope });
                if (domArg.Tag != null)
                    _codeStack.Peek().Tag = domArg.Tag;

                method.Statements.Add(new CodeMethodReturnStatement(domArg.CodeExpression));
            }

            _mainType.Type.Members.Add(method);

            var methodcall = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, method.Name));
            var rowParam = new CodeParameterDeclarationExpression(_codeStack.Peek().Scope.CodeDomReference.TypeArguments[0], "row");
            method.Parameters.Add(rowParam);
            methodcall.Parameters.Add(new CodeVariableReferenceExpression("row"));

            if(_codeStack.Peek().Tag != null) //pick statement
            {
                var htmlNodeParam = new CodeParameterDeclarationExpression(new CodeTypeReference("HtmlNode"), "node");
                methodcall.Parameters.Add(new CodeVariableReferenceExpression("node"));
                method.Parameters.Add(htmlNodeParam);
            }

            _codeStack.Peek()
                  .ParentStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("result"),
                      "AddColumn",
                      new CodePrimitiveExpression("(No column name)")));

            _codeStack.Peek().CodeExpression = methodcall;

        }
    }
}
