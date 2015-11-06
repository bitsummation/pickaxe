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
        public void Visit(ProcedureCall call)
        {
            var domArg = new CodeDomArg();

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Exec_" + domArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            GenerateCallStatement(method.Statements, call.Line.Line);
            _mainType.Type.Members.Add(method);

            var methodStatements = new CodeStatementCollection();

            var argList = new List<CodeExpression>();
            foreach(var arg in call.Args)
            {
                domArg = VisitChild(arg);
                argList.Add(domArg.CodeExpression);
            }

            methodStatements.Add(
                new CodeVariableDeclarationStatement(call.Name, "r",
                    new CodeObjectCreateExpression(call.Name, argList.ToArray()))
                    );

            methodStatements.Add(
            new CodeAssignStatement(
                new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("r"), "RequestFactory"),
                new CodePropertyReferenceExpression(null, "RequestFactory"))
                );

            methodStatements.Add(
                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("r"), "Run")
            );

            method.Statements.AddRange(methodStatements);

            var methodcall = new CodeMethodInvokeExpression(
              new CodeMethodReferenceExpression(null, method.Name));
            _codeStack.Peek().ParentStatements.Add(methodcall);
        }
    }
}
