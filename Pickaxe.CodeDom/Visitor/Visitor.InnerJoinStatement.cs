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

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(InnerJoinStatement statement)
        {
            var statementDomArg = VisitChild(statement.Statement);
            var scope = Scope.Current.GetTableDescriptor(statementDomArg.Scope.CodeDomReference.TypeArguments[0].BaseType);
            Scope.Current.Register(statementDomArg.Scope.CodeDomReference.TypeArguments[0].BaseType, new ScopeData<TableDescriptor> { Type = scope.Type, CodeDomReference = scope.CodeDomReference.TypeArguments[0] });

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Join_" + statementDomArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("IEnumerable", _codeStack.Peek().Scope.CodeDomReference), "outer"));
            GenerateCallStatement(method.Statements, statement.Line.Line);
            _mainType.Type.Members.Add(method);

            method.Statements.Add(new CodeVariableDeclarationStatement(statementDomArg.Scope.CodeDomReference, "table", statementDomArg.CodeExpression));

            //create anon type

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

            //Do Join
            var anonType = new CodeTypeReference(anon);

            //inner
            //copy


            method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IList", anonType), "join",
                new CodeObjectCreateExpression(new CodeTypeReference("List", anonType)))); 

            method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerator", _codeStack.Peek().Scope.CodeDomReference), "o",
                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("outer"), "GetEnumerator")));

            var outerLoop = new CodeIterationStatement();
            outerLoop.InitStatement = new CodeSnippetStatement();
            outerLoop.IncrementStatement = new CodeSnippetStatement();
            outerLoop.TestExpression = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("o"), "MoveNext");

            outerLoop.Statements.Add(new CodeVariableDeclarationStatement(_codeStack.Peek().Scope.CodeDomReference, "oc",
                new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("o"), "Current")));

            outerLoop.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerator", statementDomArg.Scope.CodeDomReference.TypeArguments[0]), "i",
                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("table"), "GetEnumerator")));


            var innerLoop = new CodeIterationStatement();
            outerLoop.Statements.Add(innerLoop);
            innerLoop.InitStatement = new CodeSnippetStatement();
            innerLoop.IncrementStatement = new CodeSnippetStatement();
            innerLoop.TestExpression = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("i"), "MoveNext");

            innerLoop.Statements.Add(new CodeVariableDeclarationStatement(statementDomArg.Scope.CodeDomReference.TypeArguments[0], "ic",
                new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("i"), "Current")));

            var booleanArgs = VisitChild(statement.Expression);
            innerLoop.Statements.Add(new CodeConditionStatement(booleanArgs.CodeExpression));

            method.Statements.Add(outerLoop);

            if(statement.Join != null)
            {
                var args = VisitChild(statement.Join, new CodeDomArg() { Scope = new ScopeData<Type> { Type = typeof(int), CodeDomReference = anonType } });

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

            _codeStack.Peek().CodeExpression = new CodeMethodInvokeExpression(
               new CodeMethodReferenceExpression(null, method.Name), new CodeVariableReferenceExpression("join"));
        }
    }
}
