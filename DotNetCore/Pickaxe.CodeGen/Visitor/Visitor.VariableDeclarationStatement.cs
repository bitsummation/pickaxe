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

using Pickaxe.CodeDom.Semantic;
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
        public void Visit(VariableDeclarationStatement statement)
        {
            var statementDomArg = VisitChild(statement.Expression);

            if (Scope.Current.IsCurrentScopeRegistered(statement.Variable)) //if variable in the same scope is already registered we add an error
                Errors.Add(new VariableAlreadyExists(new Semantic.LineInfo(statement.Line.Line, statement.Line.CharacterPosition), statement.Variable));

            Scope.Current.Register(statement.Variable, statementDomArg.Scope);

            Scope.Current.Type.Type.Members.Add( 
               new CodeMemberField() { Name = statement.Variable, Type = statementDomArg.Scope.CodeDomReference, Attributes = MemberAttributes.Public | MemberAttributes.Final });

            var assignment = new CodeAssignStatement(new CodeVariableReferenceExpression("_" + Scope.Current.ScopeIdentifier + "." + statement.Variable), statementDomArg.CodeExpression);

            _codeStack.Peek().Scope = statementDomArg.Scope;
            _codeStack.Peek().ParentStatements.Add(assignment);
        } 
    }
}
