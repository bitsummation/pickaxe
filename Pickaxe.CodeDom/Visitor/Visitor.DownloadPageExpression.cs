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
        private void GenerateDownloadDeffered(DownloadPageExpression expression, CodeTypeReference returnType, int line)
        {
            var statementDomArg = VisitChild(expression.Statement);

            if (statementDomArg.Scope.CodeDomReference.BaseType == typeof(Table<>).Name)
            {
                if (statementDomArg.Tag != null)
                    ((Action)statementDomArg.Tag)(); //remove call to OnSelect
            }
            else if (statementDomArg.Scope.CodeDomReference.BaseType != typeof(string).FullName)
                Errors.Add(new DownloadRequireString(new Semantic.LineInfo(expression.Statement.Line.Line, expression.Statement.Line.CharacterPosition)));

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Download_" + statementDomArg.MethodIdentifier;
            method.ReturnType = returnType;

            _mainType.Type.Members.Add(method);
            GenerateCallStatement(method.Statements, line);

            CodeExpression argsExpression = null;

            int threadCount = 1;
            argsExpression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(LazyDownloadArgs)), "CreateWebRequestArgs",
                    new CodeThisReferenceExpression(),
                    new CodePrimitiveExpression(line),
                    new CodePrimitiveExpression(threadCount),
                    statementDomArg.CodeExpression);

            if (expression.ThreadHint != null)
            {
                threadCount = expression.ThreadHint.ThreadCount;

                argsExpression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(LazyDownloadArgs)), "CreateWebRequestArgs",
                    new CodeThisReferenceExpression(),
                    new CodePrimitiveExpression(line),
                    new CodePrimitiveExpression(threadCount),
                    statementDomArg.CodeExpression);
            }

            if(expression.JSTableHint != null)
            {
                argsExpression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(LazyDownloadArgs)), "CreateSeleniumArgs",
                    new CodeThisReferenceExpression(),
                    new CodePrimitiveExpression(line),
                    new CodePrimitiveExpression(threadCount),
                    new CodePrimitiveExpression(expression.JSTableHint.CssWaitElement),
                    new CodePrimitiveExpression(expression.JSTableHint.CssTimeoutSeconds),
                    statementDomArg.CodeExpression);
            }

            //if in select context pick the lazy download type~
            var downloadType = Scope.Current.IsSelect ? "SelectDownloadTable" : "VariableDownloadTable";

            method.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(new CodeTypeReference(downloadType),
                argsExpression)));

            var methodcall = new CodeMethodInvokeExpression(
               new CodeMethodReferenceExpression(null, method.Name));

            _codeStack.Peek().CodeExpression = methodcall;
        }

        private CodeMemberMethod DownloadImpl(AstNode statement, string methodName, CodeTypeReference returnType, int line)
        {
            var statementDomArg = VisitChild(statement);

            if(statementDomArg.Scope.CodeDomReference.BaseType == typeof(Table<>).Name)
            {
                if (statementDomArg.Tag != null)
                    ((Action)statementDomArg.Tag)(); //remove call to OnSelect
            }
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
            var type = new CodeTypeReference("RuntimeTable", new CodeTypeReference("DownloadPage"));
            GenerateDownloadDeffered(expression, type, expression.Line.Line);

            _codeStack.Peek().Scope = new ScopeData<TableDescriptor> { Type = DownloadPage.Columns, CodeDomReference = type};
        }
    }
}
