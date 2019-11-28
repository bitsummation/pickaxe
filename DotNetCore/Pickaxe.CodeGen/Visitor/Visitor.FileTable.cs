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

using System.IO;
using Pickaxe.Runtime;
using Pickaxe.CodeDom.Semantic;
using Pickaxe.Sdk;
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
        private void BuildIRowReaderImplementation(CodeTypeDeclaration type, TableColumnArg[] tableArgs)
        {
            var method = new CodeMemberMethod();
            method.Name = "Load";
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string[]), "columns"));

            for (int x = 0; x < tableArgs.Length; x++)
            {
                var varType = TablePrimitive.FromString(tableArgs[x].Type);
                var left = new CodeFieldReferenceExpression(null, tableArgs[x].Variable);
                CodeExpression right = new CodeIndexerExpression(new CodeTypeReferenceExpression("columns"), new CodeSnippetExpression(x.ToString()));
                right = varType.ToNative(right);

                method.Statements.Add(new CodeAssignStatement(left, right));
            }

            type.Members.Add(method);
        }

        private void BuildIRowWriterImplementation(CodeTypeDeclaration type, TableColumnArg[] tableArgs)
        {
            var method = new CodeMemberMethod();
            method.Name = "Line";
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.ReturnType = new CodeTypeReference(typeof(string[]));

            var arrayDecl = new CodeVariableDeclarationStatement(typeof(string[]), "print", new CodeArrayCreateExpression(typeof(string[]), tableArgs.Length));
            method.Statements.Add(arrayDecl);
            for (int x = 0; x < tableArgs.Length; x++)
            {
                var left = new CodeIndexerExpression(new CodeTypeReferenceExpression("print"), new CodeSnippetExpression(x.ToString()));
                var right = new CodeMethodInvokeExpression(
                     new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Format"),
                         new CodeSnippetExpression("\"{0}\""), new CodeVariableReferenceExpression(tableArgs[x].Variable));

                method.Statements.Add(new CodeAssignStatement(left, right));
            }

            method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("print")));
            type.Members.Add(method);
        }

        public void Visit(FileTable table)
        {
            var descriptor = new TableDescriptor(typeof(FileTable<>));
            var fileTable = new CodeTypeDeclaration(table.Variable) { TypeAttributes = TypeAttributes.NestedPrivate };
            fileTable.BaseTypes.Add(new CodeTypeReference("IRowWriter"));
            fileTable.BaseTypes.Add(new CodeTypeReference("IRowReader"));

            //field
            var fileTableCodeDomType = new CodeTypeReference("FileTable", new CodeTypeReference(table.Variable));
            Scope.Current.Type.Type.Members.Add(
                new CodeMemberField(fileTableCodeDomType, table.Variable) { Attributes = MemberAttributes.Public | MemberAttributes.Final });

            var locationArg = VisitChild(table.Location);

            //constructor
            Scope.Current.Type.Constructor.Statements.Add(new CodeAssignStatement(
                new CodeSnippetExpression(table.Variable),
                new CodeObjectCreateExpression(
                    new CodeTypeReference("FileTable", new CodeTypeReference(table.Variable)))));

            BuildIRowWriterImplementation(fileTable, table.Args);
            BuildIRowReaderImplementation(fileTable, table.Args);

            foreach (var arg in table.Args)
            {
                var domArg = VisitChild(arg);
                fileTable.Members.AddRange(domArg.ParentMemberDefinitions);
                descriptor.Variables.Add(new VariableTypePair { Variable = arg.Variable, Primitive= TablePrimitive.FromString(arg.Type)});
            }

            _mainType.Type.Members.Add(fileTable);

            if (Scope.Current.IsCurrentScopeRegistered(table.Variable))
                Errors.Add(new VariableAlreadyExists(new Semantic.LineInfo(table.Line.Line, table.Line.CharacterPosition), table.Variable));

            Scope.Current.RegisterTable(table.Variable, descriptor, fileTableCodeDomType);

            //Init Code
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Init_" + fileTable.Name;
            method.Attributes = MemberAttributes.Private;

            _mainType.Type.Members.Add(method);
            var assignment = new CodeAssignStatement(new CodeVariableReferenceExpression("_" + Scope.Current.ScopeIdentifier + "." + table.Variable + ".Location"), locationArg.CodeExpression);
            method.Statements.Add(assignment);

            assignment = new CodeAssignStatement(new CodeVariableReferenceExpression("_" + Scope.Current.ScopeIdentifier + "." + table.Variable + ".FieldTerminator"), new CodePrimitiveExpression(table.FieldTerminator));
            method.Statements.Add(assignment);

            assignment = new CodeAssignStatement(new CodeVariableReferenceExpression("_" + Scope.Current.ScopeIdentifier + "." + table.Variable + ".RowTerminator"), new CodePrimitiveExpression(table.RowTerminator));
            method.Statements.Add(assignment);

            method.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("_" + Scope.Current.ScopeIdentifier + "." + table.Variable), "Load"));

            var methodcall = new CodeMethodInvokeExpression(
               new CodeMethodReferenceExpression(null, method.Name));

            _codeStack.Peek().ParentStatements.Add(methodcall);
            _codeStack.Peek().CodeExpression = methodcall;
        }

    }
}
