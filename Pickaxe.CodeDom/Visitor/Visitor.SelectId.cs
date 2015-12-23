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
            ScopeData<TableDescriptor> descriptor = Scope.EmptyTableDescriptor;

            if(_codeStack.Peek().Scope != null)
            {
                var rowType = _codeStack.Peek().Scope.CodeDomReference.TypeArguments[0];

                if (Scope.Current.IsRegistered(rowType.BaseType))
                    descriptor = Scope.Current.GetTableDescriptor(rowType.BaseType);
            }

            _codeStack.Peek()
                   .ParentStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("result"),
                       "AddColumn",
                       new CodePrimitiveExpression(id.Id)));

            _codeStack.Peek().CodeExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("row"), id.Id);

            if (!descriptor.Type.Variables.Any(x => x.Variable == id.Id)) //variable is not in table
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
                {
                    //assign default type error condition
                    _codeStack.Peek().Scope = new ScopeData<Type> { Type = typeof(int), CodeDomReference = new CodeTypeReference(typeof(int)) };
                    Errors.Add(new UnknownSelectVariableException(new Semantic.LineInfo(id.Line.Line, id.Line.CharacterPosition), id.Id));
                }
            }
            else
            {
                var pair = descriptor.Type.Variables.Where(x => x.Variable == id.Id).Single();
                _codeStack.Peek().Scope = new ScopeData<Type> { Type = pair.Type.Type, CodeDomReference = new CodeTypeReference(pair.Type.Type) };
            }
        }
    }
}
