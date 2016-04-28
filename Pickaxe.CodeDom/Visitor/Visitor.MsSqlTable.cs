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
using System.Reflection;
using System.Text;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(MsSqlTable table)
        {
            var descriptor = new TableDescriptor(typeof(MsSqlTable<>));
            var sqlTable = new CodeTypeDeclaration(table.Variable) { TypeAttributes = TypeAttributes.NestedPrivate };
            sqlTable.BaseTypes.Add(new CodeTypeReference("IRow"));

            var sqlTableCodeDomType = new CodeTypeReference("MsSqlTable", new CodeTypeReference(table.Variable));
            Scope.Current.Type.Type.Members.Add(
                new CodeMemberField(sqlTableCodeDomType, table.Variable) { Attributes = MemberAttributes.Public | MemberAttributes.Final });

            Scope.Current.Type.Constructor.Statements.Add(new CodeAssignStatement(
               new CodeSnippetExpression(table.Variable),
               new CodeObjectCreateExpression(
                   new CodeTypeReference("MsSqlTable", new CodeTypeReference(table.Variable)))));

            var codeExpressions = new List<CodeExpression>();
            foreach (var arg in table.Args)
            {
                var domArg = VisitChild(arg);
                sqlTable.Members.AddRange(domArg.ParentMemberDefinitions);
                codeExpressions.Add(new CodePrimitiveExpression(arg.Variable));
                descriptor.Variables.Add(new VariableTypePair { Variable = arg.Variable, Primitive = TablePrimitive.FromString(arg.Type) });
            }

            _mainType.Type.Members.Add(sqlTable);

            if (Scope.Current.IsCurrentScopeRegistered(table.Variable))
                Errors.Add(new VariableAlreadyExists(new Semantic.LineInfo(table.Line.Line, table.Line.CharacterPosition), table.Variable));

            Scope.Current.RegisterTable(table.Variable, descriptor, sqlTableCodeDomType);

            //Init Code
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Init_" + sqlTable.Name;
            method.Attributes = MemberAttributes.Private;

            _mainType.Type.Members.Add(method);

            var assignment = new CodeAssignStatement(new CodeVariableReferenceExpression("_" + Scope.Current.ScopeIdentifier + "." + table.Variable + ".ConnectionString"), new CodePrimitiveExpression(table.ConnectionString));
            method.Statements.Add(assignment);

            assignment = new CodeAssignStatement(new CodeVariableReferenceExpression("_" + Scope.Current.ScopeIdentifier + "." + table.Variable + ".Table"), new CodePrimitiveExpression(table.Table));
            method.Statements.Add(assignment);

            assignment = new CodeAssignStatement(new CodeVariableReferenceExpression("_" + Scope.Current.ScopeIdentifier + "." + table.Variable + ".FieldNames"),
                new CodeArrayCreateExpression(new CodeTypeReference(typeof(string)), codeExpressions.ToArray()));
            method.Statements.Add(assignment);

            var methodcall = new CodeMethodInvokeExpression(
               new CodeMethodReferenceExpression(null, method.Name));

            _codeStack.Peek().ParentStatements.Add(methodcall);
            _codeStack.Peek().CodeExpression = methodcall;
        }
    }
}
