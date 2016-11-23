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
using System.Text.RegularExpressions;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {

        private string ReplaceBooleanStatement(AliasBase aliasBAse, string statement)
        {
            //the inner is the new join (ic). Everything else is outer (oc)

            string innerMatch = @"row\.(" + aliasBAse.Alias.Id + @")";
            string outerMatch = @"row\.([^" + aliasBAse.Alias.Id + @"])";
            statement = Regex.Replace(statement, innerMatch, "ic.$1");
            statement = Regex.Replace(statement, outerMatch, "oc.$1");

            return statement;
        }

        public void Visit(InnerJoinStatement statement)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Join_" + Guid.NewGuid().ToString("N");
            method.Attributes = MemberAttributes.Private;
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("IEnumerable", _codeStack.Peek().Scope.CodeDomReference), "outer"));
            GenerateCallStatement(method.Statements, statement.Line.Line);
            _mainType.Type.Members.Add(method);


            CodeTypeReference fetchAnonType;
            var fetchMethod = CreateFetch(statement, out fetchAnonType);
            method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerable", fetchAnonType), "table",
                   new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, fetchMethod.Name))));


            //create combined anon type
            var anon = "anon_" + Guid.NewGuid().ToString("N");
            var bufferTable = new CodeTypeDeclaration(anon) { TypeAttributes = TypeAttributes.NestedPrivate };
            bufferTable.BaseTypes.Add(new CodeTypeReference("IRow"));
            _mainType.Type.Members.Add(bufferTable);
            bufferTable.Members.AddRange(Scope.Current.JoinMembers.ToArray());

            //Do Join
            var anonType = new CodeTypeReference(anon);

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

            outerLoop.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerator", fetchAnonType), "i",
                new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("table"), "GetEnumerator")));


            var innerLoop = new CodeIterationStatement();
            outerLoop.Statements.Add(innerLoop);
            innerLoop.InitStatement = new CodeSnippetStatement();
            innerLoop.IncrementStatement = new CodeSnippetStatement();
            innerLoop.TestExpression = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("i"), "MoveNext");

            innerLoop.Statements.Add(new CodeVariableDeclarationStatement(fetchAnonType, "ic",
                new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("i"), "Current")));

            var booleanArgs = VisitChild(statement.Expression);
            string booleanString = ReplaceBooleanStatement(statement, GenerateCodeFromExpression(booleanArgs.CodeExpression));

            var joinIf = new CodeConditionStatement(new CodeSnippetExpression(booleanString));
            innerLoop.Statements.Add(joinIf);

            joinIf.TrueStatements.Add(new CodeVariableDeclarationStatement(anonType, "t", new CodeObjectCreateExpression(anonType)));
            
            for(int x = 0; x < Scope.Current.JoinMembers.Count - 1; x++)
            {
                joinIf.TrueStatements.Add(new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        new CodeVariableReferenceExpression("t"), Scope.Current.JoinMembers[x].Name),
                    new CodePropertyReferenceExpression(
                        new CodeVariableReferenceExpression("oc"), Scope.Current.JoinMembers[x].Name)));
            }

            joinIf.TrueStatements.Add(new CodeAssignStatement(
                    new CodePropertyReferenceExpression(
                        new CodeVariableReferenceExpression("t"), Scope.Current.JoinMembers[Scope.Current.JoinMembers.Count-1].Name),
                    new CodePropertyReferenceExpression(
                        new CodeVariableReferenceExpression("ic"), Scope.Current.JoinMembers[Scope.Current.JoinMembers.Count - 1].Name)));


            joinIf.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("join"), "Add", new CodeVariableReferenceExpression("t")));
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
                var tableType = new CodeTypeReference("CodeTable", anonType);
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
