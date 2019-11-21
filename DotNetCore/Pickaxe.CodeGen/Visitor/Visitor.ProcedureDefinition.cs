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
        public void Visit(ProcedureDefinition statement)
        {
            var argList = new List<CodeParameterDeclarationExpression>();
            foreach (var arg in statement.Args)
            {
                var variableType = TablePrimitive.FromString(arg.Type).Type;
                var codeParam = new CodeParameterDeclarationExpression(variableType, arg.Variable);
                Scope.Current.RegisterPrimitive(codeParam.Name, variableType, codeParam.Type);
                Scope.Current.Type.Type.Members.Add(new CodeMemberField() { Name = codeParam.Name, Type = codeParam.Type, Attributes = MemberAttributes.Public | MemberAttributes.Final });

                var assignment = new CodeAssignStatement(new CodeVariableReferenceExpression("_" + Scope.Current.ScopeIdentifier + "." + codeParam.Name), new CodeVariableReferenceExpression(codeParam.Name));
                _mainType.Constructor.Statements.Add(assignment);

                argList.Add(codeParam);
            }

            _mainType.Type.Name = statement.Name;
            _mainType.Constructor.Parameters.Clear();
            _mainType.Constructor.BaseConstructorArgs.Clear();
            _mainType.Constructor.Parameters.AddRange(argList.ToArray());
            _mainType.Constructor.BaseConstructorArgs.Add(new CodeArrayCreateExpression(new CodeTypeReference(typeof(string[])), 0));

            //visit block
            var blockArgs = VisitChild(statement.Block);
            _codeStack.Peek().ParentStatements.AddRange(blockArgs.ParentStatements);
        }
    }
}
