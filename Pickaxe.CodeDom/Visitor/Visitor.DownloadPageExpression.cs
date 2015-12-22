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

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        private CodeMemberMethod DownloadImpl(AstNode statement, string methodName, CodeTypeReference returnType, int line)
        {
            var statementDomArg = VisitChild(statement);

            if(statementDomArg.Scope.CodeDomReference.BaseType == typeof(Table<>).Name)
                ((Action)statementDomArg.Tag)(); //remove call to OnSelect
            else if( statementDomArg.Scope.CodeDomReference.BaseType != typeof(string).FullName)
                Errors.Add(new DownloadRequireString(new Semantic.LineInfo(statement.Line.Line, statement.Line.CharacterPosition)));

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Download_" + statementDomArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = returnType;
            GenerateCallStatement(method.Statements, line);

            method.Statements.Add(new CodeMethodReturnStatement(
             new CodeMethodInvokeExpression(
                 new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("Http"), methodName), new CodeThisReferenceExpression(),
                 statementDomArg.CodeExpression, new CodePrimitiveExpression(line))));

            _mainType.Type.Members.Add(method);

            var methodcall = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(null, method.Name));

            _codeStack.Peek().CodeExpression = methodcall;
            return method;
        }

        public void Visit(DownloadPageExpression expression)
        {
            var type = new CodeTypeReference("Table", new CodeTypeReference("DownloadPage"));
            DownloadImpl(expression.Statement, "DownloadPage", type, expression.Line.Line);

            _codeStack.Peek().Scope = new ScopeData<TableDescriptor> { Type = DownloadPage.Columns, CodeDomReference = type};
        }
    }
}
