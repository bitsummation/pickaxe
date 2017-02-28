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
        public void Visit(SelectId id)
        {
            var selectInfo = new SelectArgsInfo();

            _codeStack.Peek().Scope = new ScopeData<Type> { Type = typeof(int?), CodeDomReference = new CodeTypeReference(typeof(int?)) };
            _codeStack.Peek().CodeExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("row"), id.Id);
            selectInfo.ColumnName = id.Id;
            _codeStack.Peek().Tag = selectInfo;

            //1. Here we need to look through the select scope to get the variable.
            //If there, put correct table prefix.
            //if more than one we need to throw error
            //if we can't find we need to dig up the scope

            var tableMatches = Scope.Current.FindTableVariable(id.Id);

            if(tableMatches.Length > 0)
            {
                if (tableMatches.Length == 1) //we only found one
                {
                    _codeStack.Peek().CodeExpression = new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("row"), tableMatches[0].TableAlias), id.Id);
                    _codeStack.Peek().Scope = new ScopeData<Type> { Type = tableMatches[0].TableVariable.Primitive.Type, CodeDomReference = new CodeTypeReference(tableMatches[0].TableVariable.Primitive.Type) };
                }
                else //error we found more than 1
                    Errors.Add(new AmbiguousSelectVariable(tableMatches, new Semantic.LineInfo(id.Line.Line, id.Line.CharacterPosition)));
            }
            else //not found in the table variable so look up scope
            {
                //Need to check in the Scope to see if variable is defined there. If in select statmenet it is valid to put variables in it.
                if (Scope.Current.IsRegistered(id.Id))
                {
                    var variable = new VariableReferance() { Id = id.Id, Line = id.Line };
                    var variableArgs = VisitChild(variable);
                    _codeStack.Peek().CodeExpression = variableArgs.CodeExpression;
                    _codeStack.Peek().Scope = variableArgs.Scope;
                }
                else
                    Errors.Add(new UnknownSelectVariableException(new Semantic.LineInfo(id.Line.Line, id.Line.CharacterPosition), id.Id));
            }
        }
    }
}
