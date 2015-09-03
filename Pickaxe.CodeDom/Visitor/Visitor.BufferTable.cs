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
using Pickaxe.Runtime;
using Pickaxe.CodeDom.Semantic;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(BufferTable table)
        {
            var descriptor = new TableDescriptor();
            var bufferTable = new CodeTypeDeclaration(table.Variable) { TypeAttributes = TypeAttributes.NestedPrivate };
            bufferTable.BaseTypes.Add(new CodeTypeReference("IRow"));

            var bufferCodeDomType = new CodeTypeReference("CodeTable", new CodeTypeReference(table.Variable));
            Scope.Current.Type.Type.Members.Add(
                new CodeMemberField(bufferCodeDomType, table.Variable) { Attributes = MemberAttributes.Public | MemberAttributes.Final });

            Scope.Current.Type.Constructor.Statements.Add(new CodeAssignStatement(
                new CodeSnippetExpression(table.Variable),
                new CodeObjectCreateExpression(
                    new CodeTypeReference("BufferTable", new CodeTypeReference(table.Variable)))));

            foreach (var arg in table.Args)
            {
                var domArg = VisitChild(arg);
                bufferTable.Members.AddRange(domArg.ParentMemberDefinitions);
                descriptor.Variables.Add(new VariableTypePair { Variable = arg.Variable, Type = TablePrimitive.FromString(arg.Type) });
            }

            _mainType.Type.Members.Add(bufferTable);

            if (Scope.Current.IsCurrentScopeRegistered(table.Variable))
                Errors.Add(new VariableAlreadyExists(new Semantic.LineInfo(table.Line.Line, table.Line.CharacterPosition), table.Variable));

            Scope.Current.RegisterTable(table.Variable, descriptor, bufferCodeDomType);
        }
    }
}
