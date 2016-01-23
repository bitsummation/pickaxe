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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        private CodeMemberMethod CreateCopyMethod(CodeParameterDeclarationExpressionCollection methodParams, CodeStatementCollection statements)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Copy_" + Guid.NewGuid().ToString("N");
            method.Parameters.AddRange(methodParams);
            _mainType.Type.Members.Add(method);

            method.Statements.AddRange(statements);

            return method;
        }

        public void Visit(FromStatement statement)
        {
            var statementDomArg = VisitChild(statement.Statement);
            _joinMembers.Clear();

            var scope = Scope.Current.GetTableDescriptor(statementDomArg.Scope.CodeDomReference.TypeArguments[0].BaseType);
            if(scope != null)
                Scope.Current.Register(statementDomArg.Scope.CodeDomReference.TypeArguments[0].BaseType, new ScopeData<TableDescriptor> { Type = scope.Type, CodeDomReference = scope.CodeDomReference.TypeArguments[0] });
            
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "From_" + statementDomArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = statementDomArg.Scope.CodeDomReference;
            GenerateCallStatement(method.Statements, statement.Line.Line);
            _mainType.Type.Members.Add(method);

            method.Statements.Add(new CodeVariableDeclarationStatement(statementDomArg.Scope.CodeDomReference, "table", statementDomArg.CodeExpression));


            var anon = "anon_" + Guid.NewGuid().ToString("N");
            var bufferTable = new CodeTypeDeclaration(anon) { TypeAttributes = TypeAttributes.NestedPrivate };
            bufferTable.BaseTypes.Add(new CodeTypeReference("IRow"));
            _mainType.Type.Members.Add(bufferTable);

            var field = new CodeMemberField();
            field.Name = statementDomArg.Scope.CodeDomReference.TypeArguments[0].BaseType;
            field.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            field.Type = statementDomArg.Scope.CodeDomReference.TypeArguments[0];
            _joinMembers.Add(field);
            bufferTable.Members.AddRange(_joinMembers.ToArray());

            var codeStatements = new CodeStatementCollection();

            var anonType = new CodeTypeReference(anon);
            codeStatements.Add(new CodeVariableDeclarationStatement(anonType, "t",
               new CodeObjectCreateExpression(anonType)));

            codeStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("t"), field.Name),
                new CodeVariableReferenceExpression("o")));

            codeStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("t")));

            var codeParams = new CodeParameterDeclarationExpressionCollection();
            codeParams.Add(new CodeParameterDeclarationExpression(statementDomArg.Scope.CodeDomReference.TypeArguments[0], "o"));
            var copyMethod = CreateCopyMethod(codeParams, codeStatements);
            copyMethod.ReturnType = anonType;

            var anonExpression = new CodeSnippetExpression("o => {" + GenerateCode(
                new CodeMethodReturnStatement(
                    new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, copyMethod.Name),
                        new CodeVariableReferenceExpression("o")))) + "}");

            method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerable", anonType), "join",
                    new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression("table"), "Select"), anonExpression)));

            if(statement.Join != null)
            {
                var args = VisitChild(statement.Join, new CodeDomArg() {Scope = new ScopeData<Type> { Type = typeof(int), CodeDomReference = anonType} });

                method.Statements.Add(new CodeMethodReturnStatement(args.CodeExpression));
                method.ReturnType = args.Scope.CodeDomReference;
                _codeStack.Peek().Scope = args.Scope;
            }
            else
            {
                var tableType = new CodeTypeReference(statementDomArg.Scope.CodeDomReference.BaseType, anonType);
                method.ReturnType = tableType;
                _codeStack.Peek().Scope = new ScopeData<Type> { Type = typeof(int), CodeDomReference = tableType };

                method.Statements.Add(new CodeVariableDeclarationStatement(tableType, "newTable",
                    new CodeObjectCreateExpression(tableType)));

                method.Statements.Add(new CodeMethodInvokeExpression(
                    new CodeVariableReferenceExpression("newTable"), "SetRows",
                    new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("join"), "ToList")));

                method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("newTable")));
            }

            var methodcall = new CodeMethodInvokeExpression(
              new CodeMethodReferenceExpression(null, method.Name));

            _codeStack.Peek().CodeExpression = methodcall;
        }
    }
}
