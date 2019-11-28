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

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pickaxe.CodeDom.Semantic;
using Pickaxe.Runtime;
using Pickaxe.Sdk;
using System;
using System.Collections.Generic;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        private void GenerateDownloadDeffered(DownloadPageExpression expression, ref GenericNameSyntax returnType, int line)
        {
            var statementDomArg = VisitChild(expression.Statement);
            //if in select context pick the lazy download type~
            var downloadType = Scope.Current.IsSelect ? "SelectDownloadTable" : "VariableDownloadTable";

            if (statementDomArg.Scope.TypeSyntax.GetText().ToString() == typeof(Table<>).Name)
            {
                if (statementDomArg.Tag != null)
                    ((Action)statementDomArg.Tag)(); //remove call to OnSelect
            }
            else if (statementDomArg.Scope.TypeSyntax.GetText().ToString() != typeof(string).Name.ToLower())
                Errors.Add(new DownloadRequireString(new Semantic.LineInfo(expression.Statement.Line.Line, expression.Statement.Line.CharacterPosition)));

            var method = SyntaxFactory.MethodDeclaration(
                returnType,
                 SyntaxFactory.Identifier("Download_" + statementDomArg.MethodIdentifier))
                 .WithModifiers(
                 SyntaxFactory.TokenList(
                     SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                     .WithBody(
                 SyntaxFactory.Block());

            var methodStatements = new List<StatementSyntax>();
            GenerateCallStatement(methodStatements, line);

            ExpressionSyntax argsExpression = null;

            int threadCount = 1;

            argsExpression = SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("LazyDownloadArgs"),
                                        SyntaxFactory.IdentifierName("CreateWebRequestArgs")))
                                .AddArgumentListArguments(
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.ThisExpression()),
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        SyntaxFactory.Literal(line))),
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        SyntaxFactory.Literal(threadCount))),
                                                SyntaxFactory.Argument(statementDomArg.CodeExpression));

            if (expression.ThreadHint != null)
            {
                threadCount = expression.ThreadHint.ThreadCount;

                argsExpression = SyntaxFactory.InvocationExpression(
                                     SyntaxFactory.MemberAccessExpression(
                                         SyntaxKind.SimpleMemberAccessExpression,
                                         SyntaxFactory.IdentifierName("LazyDownloadArgs"),
                                         SyntaxFactory.IdentifierName("CreateWebRequestArgs")))
                                 .AddArgumentListArguments(
                                                 SyntaxFactory.Argument(
                                                     SyntaxFactory.ThisExpression()),
                                                 SyntaxFactory.Argument(
                                                     SyntaxFactory.LiteralExpression(
                                                         SyntaxKind.NumericLiteralExpression,
                                                         SyntaxFactory.Literal(line))),
                                                 SyntaxFactory.Argument(
                                                     SyntaxFactory.LiteralExpression(
                                                         SyntaxKind.NumericLiteralExpression,
                                                         SyntaxFactory.Literal(threadCount))),
                                                 SyntaxFactory.Argument(statementDomArg.CodeExpression));
            }

            string cssWaitElement = null;
            int cssTimeout = 5;
            if(expression.JSTableHint != null)
            {
                cssWaitElement = expression.JSTableHint.CssWaitElement;
                cssTimeout = expression.JSTableHint.CssTimeoutSeconds;

                argsExpression = SyntaxFactory.InvocationExpression(
                                   SyntaxFactory.MemberAccessExpression(
                                       SyntaxKind.SimpleMemberAccessExpression,
                                       SyntaxFactory.IdentifierName("LazyDownloadArgs"),
                                       SyntaxFactory.IdentifierName("CreateSeleniumArgs")))
                               .AddArgumentListArguments(
                                               SyntaxFactory.Argument(
                                                   SyntaxFactory.ThisExpression()),
                                               SyntaxFactory.Argument(
                                                   SyntaxFactory.LiteralExpression(
                                                       SyntaxKind.NumericLiteralExpression,
                                                       SyntaxFactory.Literal(line))),
                                               SyntaxFactory.Argument(
                                                   SyntaxFactory.LiteralExpression(
                                                       SyntaxKind.NumericLiteralExpression,
                                                       SyntaxFactory.Literal(threadCount))),
                                                SyntaxFactory.Argument(
                                                   SyntaxFactory.LiteralExpression(
                                                       SyntaxKind.NumericLiteralExpression,
                                                       SyntaxFactory.Literal(cssWaitElement))),
                                                SyntaxFactory.Argument(
                                                   SyntaxFactory.LiteralExpression(
                                                       SyntaxKind.NumericLiteralExpression,
                                                       SyntaxFactory.Literal(cssTimeout))),
                                               SyntaxFactory.Argument(statementDomArg.CodeExpression));
            }

            if (expression.JavascriptCode != null)
            {
                argsExpression = SyntaxFactory.InvocationExpression(
                                  SyntaxFactory.MemberAccessExpression(
                                      SyntaxKind.SimpleMemberAccessExpression,
                                      SyntaxFactory.IdentifierName("LazyDownloadArgs"),
                                      SyntaxFactory.IdentifierName("CreateJavaScriptArgs")))
                              .AddArgumentListArguments(
                                              SyntaxFactory.Argument(
                                                  SyntaxFactory.ThisExpression()),
                                              SyntaxFactory.Argument(
                                                  SyntaxFactory.LiteralExpression(
                                                      SyntaxKind.NumericLiteralExpression,
                                                      SyntaxFactory.Literal(line))),
                                              SyntaxFactory.Argument(
                                                  SyntaxFactory.LiteralExpression(
                                                      SyntaxKind.NumericLiteralExpression,
                                                      SyntaxFactory.Literal(threadCount))),
                                               SyntaxFactory.Argument(
                                                  SyntaxFactory.LiteralExpression(
                                                      SyntaxKind.NumericLiteralExpression,
                                                      SyntaxFactory.Literal(cssWaitElement))),
                                               SyntaxFactory.Argument(
                                                  SyntaxFactory.LiteralExpression(
                                                      SyntaxKind.NumericLiteralExpression,
                                                      SyntaxFactory.Literal(cssTimeout))),
                                              SyntaxFactory.Argument(statementDomArg.CodeExpression),
                                                 SyntaxFactory.Argument(
                                                  SyntaxFactory.LiteralExpression(
                                                      SyntaxKind.NumericLiteralExpression,
                                                      SyntaxFactory.Literal(expression.JavascriptCode.Code))));

                downloadType = "DynamicObjectDownloadTable";
                returnType = SyntaxFactory.GenericName(
                      SyntaxFactory.Identifier("RuntimeTable"))
                      .AddTypeArgumentListArguments(SyntaxFactory.IdentifierName("DynamicObject"));

                method = method.WithReturnType(returnType);
            }

            methodStatements.Add(SyntaxFactory.ReturnStatement(
                                SyntaxFactory.ObjectCreationExpression(
                                    SyntaxFactory.IdentifierName(downloadType))
                                    .AddArgumentListArguments(SyntaxFactory.Argument(argsExpression)))
                                    );

            var methodcall = SyntaxFactory.InvocationExpression(
                       SyntaxFactory.IdentifierName(method.Identifier));

            method = method.WithBody(SyntaxFactory.Block(
                        methodStatements
                        ));

            _mainType.AddMember(method);
            _codeStack.Peek().CodeExpression = methodcall;
        }

        public void Visit(DownloadPageExpression expression)
        {
            var type = SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier("RuntimeTable"))
                        .AddTypeArgumentListArguments(SyntaxFactory.IdentifierName("DownloadPage"));

            GenerateDownloadDeffered(expression, ref type, expression.Line.Line);

            _codeStack.Peek().Scope = new ScopeData<TableDescriptor> { Type = DownloadPage.Columns, TypeSyntax = type};
        }
    }
}
