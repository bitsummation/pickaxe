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
        public void Visit(TableMemberReference variable)
        {
            /*
            var selectInfo = new SelectArgsInfo();

            _codeStack.Peek().Scope = new ScopeData<Type> { Type = typeof(string), CodeDomReference = new CodeTypeReference(typeof(string)) };
            var rowArgs = VisitChild(variable.RowReference);

            var expression = new CodeFieldReferenceExpression(rowArgs.CodeExpression, variable.Member);
            _codeStack.Peek().CodeExpression = expression;

            if (Scope.Current.IsTableRowRegistered(variable.RowReference.Id))
            {
                
                var descriptor = Scope.Current.GetTableDescriptor(variable.RowReference.Id);
                var dynamicAlias = Scope.Current.AliasType<DynamicObject>();
                if (dynamicAlias.Length > 0) //alias is a dynamic type so we get it from there
                {
                    _codeStack.Peek().CodeExpression = new CodeIndexerExpression(rowArgs.CodeExpression, new CodePrimitiveExpression(variable.Member));
                    _codeStack.Peek().Scope = new ScopeData<Type> { Type = typeof(string), CodeDomReference = new CodeTypeReference(typeof(string)) };
                   
                }
                else if (!descriptor.Type.Variables.Any(x => x.Variable == variable.Member)) //error don't know member
                {
                    Errors.Add(new NoTableMember(new Semantic.LineInfo(variable.Line.Line, variable.Line.CharacterPosition), variable.Member));
                }
                else
                {
                    var members = descriptor.Type.Variables.Where(x => x.Variable == variable.Member).ToList();

                    if (members.Count > 1)
                    {
                        var selectMatches = members.Select(x => new SelectMatch() { TableVariable = x }).ToArray();
                        Errors.Add(new AmbiguousSelectVariable(selectMatches, new Semantic.LineInfo(variable.Line.Line, variable.Line.CharacterPosition)));
                    }
                    else
                    {
                        var member = members.Single();
                        _codeStack.Peek().Scope = new ScopeData<Type> { Type = member.Primitive.Type, CodeDomReference = new CodeTypeReference(member.Primitive.Type) };
                    }
                }
            }

            selectInfo.ColumnName = variable.Member;
            _codeStack.Peek().Tag = selectInfo;
            */
            
        }
    }
}
