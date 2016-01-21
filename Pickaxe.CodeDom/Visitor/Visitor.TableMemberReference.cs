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
        public void Visit(TableMemberReference variable)
        {
            _codeStack.Peek().Scope = new ScopeData<Type> { Type = typeof(string), CodeDomReference = new CodeTypeReference(typeof(string)) };
            var rowArgs = VisitChild(variable.RowReference);

            if (Scope.Current.IsTableRowRegistered(variable.RowReference.Id))
            {
                var descriptor = Scope.Current.GetTableDescriptor(variable.RowReference.Id);
                if (!descriptor.Type.Variables.Any(x => x.Variable == variable.Member)) //error don't know member
                    Errors.Add(new NoTableMember(new Semantic.LineInfo(variable.Line.Line, variable.Line.CharacterPosition), variable.Member));
                else
                {
                    var member = descriptor.Type.Variables.Where(x => x.Variable == variable.Member).Single();
                    _codeStack.Peek().Scope = new ScopeData<Type> { Type = member.Type.Type, CodeDomReference = new CodeTypeReference(member.Type.Type) };
                }
            }

            _codeStack.Peek()
                   .ParentStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("result"),
                       "AddColumn",
                       new CodePrimitiveExpression(variable.Member)));

            var expression = new CodeFieldReferenceExpression(rowArgs.CodeExpression, variable.Member);
            _codeStack.Peek().CodeExpression = expression;
        }
    }
}
