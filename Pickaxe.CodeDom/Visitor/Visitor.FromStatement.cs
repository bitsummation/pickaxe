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
        public void Visit(FromStatement statement)
        {
            var statementDomArg = VisitChild(statement.Statement);

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "From_" + statementDomArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = statementDomArg.Scope.CodeDomReference;
            GenerateCallStatement(method.Statements, statement.Line.Line);
            _mainType.Type.Members.Add(method);

            method.Statements.Add(new CodeVariableDeclarationStatement(statementDomArg.Scope.CodeDomReference, "table", statementDomArg.CodeExpression));

            if(statement.Join != null)
            {
                var anon = "anon_" + Guid.NewGuid().ToString("N");
                var bufferTable = new CodeTypeDeclaration(anon) { TypeAttributes = TypeAttributes.NestedPrivate };
                _mainType.Type.Members.Add(bufferTable);

                var field = new CodeMemberField();
                field.Name = statementDomArg.Scope.CodeDomReference.TypeArguments[0].BaseType;
                field.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                field.Type = statementDomArg.Scope.CodeDomReference.TypeArguments[0];
                bufferTable.Members.Add(field);

                var boolean = new CodeSnippetExpression("O => {" + GenerateCode(new CodeMethodReturnStatement(statementDomArg.CodeExpression)) + "}");

                method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerable", statementDomArg.Scope.CodeDomReference.TypeArguments[0]), "join",
                    new CodeMethodInvokeExpression(
                        new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(
                            new CodeTypeReferenceExpression("table"), "Where"), boolean), "ToList")));

                /*using (Scope.Push(_mainType))
                {
                    Scope.Current.RegisterPrimitive(field.Name,  )
                }*/
                
                //create type
                //generate select
                //Visit Join.
                //return the statement from the join
            }
            else
            {
                method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("table")));
            }

            var methodcall = new CodeMethodInvokeExpression(
              new CodeMethodReferenceExpression(null, method.Name));

            _codeStack.Peek().CodeExpression = methodcall;
            _codeStack.Peek().Scope = statementDomArg.Scope;
        }
    }
}
