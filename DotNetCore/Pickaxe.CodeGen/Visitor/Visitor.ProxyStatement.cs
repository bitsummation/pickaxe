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
        public void Visit(ProxyStatement statement)
        {
            var selectArgs = VisitChild(statement.Test);
            VisitChild(statement.List);

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "TestProxy";
            method.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            GenerateCallStatement(method.Statements, statement.Line.Line);

            ((Action)selectArgs.Tag)();

            method.Statements.Add(new CodeVariableDeclarationStatement(selectArgs.Scope.CodeDomReference,
                "resultRows",
                selectArgs.CodeExpression));

            method.Statements.Add(new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression("resultRows"), "RowCount"),
                CodeBinaryOperatorType.GreaterThan,
                new CodePrimitiveExpression(0))
                ));

            _mainType.Type.Members.Add(method);
        }
    }
}
