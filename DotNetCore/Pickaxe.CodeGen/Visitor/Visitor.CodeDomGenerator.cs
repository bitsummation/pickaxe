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

using System;
using System.Collections.Generic;
using Pickaxe.Sdk;
using Pickaxe.Runtime;
using Pickaxe.CodeDom.Semantic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        private AstNode _program;
        private CompilationUnitSyntax _unit;
        private CodeDomTypeDefinition _mainType;

        private int _totalOperations;
        private Stack<CodeDomArg> _codeStack;

        public CodeDomGenerator(AstNode program)
        {
            _totalOperations = 0;

            Errors = new List<SemanticException>();
            _program = program;
            _codeStack = new Stack<CodeDomArg>();

            _unit = SyntaxFactory.CompilationUnit();
            _mainType = new CodeDomTypeDefinition("Code");
            _mainType.AddBaseType("RuntimeBase");

            Scope.Reset();
            Scope.Push(_mainType);
            InitScope();
        }

        private void InitScope() //add runtime types
        {
            //DownloadPage
            Scope.Current.RegisterTable("DownloadPage", DownloadPage.Columns);
            Scope.Current.RegisterTable("DownloadImage", DownloadImage.Columns);
            Scope.Current.RegisterTable("Expand", Expand.Columns);
            Scope.Current.RegisterTable("DynamicObject", DynamicObject.Columns);

            //Register @@identity
            Scope.Current.RegisterPrimitive("@@identity", typeof(int));

            Scope.Current.Type.AddMember(SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(SyntaxKind.IntKeyword)))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier("g_identity")))))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                );
        }

        public IList<SemanticException> Errors { get; private set; }

        public SyntaxTree Generate()
        { 
            _program.Accept(this);
            return _unit.SyntaxTree;
        }

        private void CallOnProgressComplete(List<StatementSyntax> statements)
        {
            statements.Add(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName("OnProgress"))
                    .AddArgumentListArguments(
                        SyntaxFactory.Argument(
                            SyntaxFactory.ObjectCreationExpression(
                                SyntaxFactory.IdentifierName("ProgressArgs"))
                            .AddArgumentListArguments(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.IdentifierName("TotalOperations")),
                                SyntaxFactory.Argument(
                                    SyntaxFactory.IdentifierName("TotalOperations"))))))
                );
        }

        private void CallOnProgress(List<StatementSyntax> statements, bool increaseTotal = true)
        {
            if(increaseTotal)
                _totalOperations++;

            statements.Add(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName("OnProgress"))
                    ));
        }

        private void GenerateCallStatement(List<StatementSyntax> statements, int line)
        {
            statements.Add(SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("Call"))
                .AddArgumentListArguments(
                    SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(line)))))
                );
        }

        private CodeDomArg VisitChild(AstNode node, CodeDomArg arg)
        {
            _codeStack.Push(arg);
            node.Accept(this);
            return _codeStack.Pop();
        }

        private CodeDomArg VisitChild(AstNode node)
        {
            return VisitChild(node, new CodeDomArg());
        }


        private MethodDeclarationSyntax CreateStepMethod()
        {
            var method = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                SyntaxFactory.Identifier("Step_" + Guid.NewGuid().ToString("N")))
                .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithBody(
                        SyntaxFactory.Block());

            _mainType.AddMember(method);
            return method;
        }

        private MethodDeclarationSyntax CreateBlockMethod()
        {
            var method = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(
                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                SyntaxFactory.Identifier("Block_" + Guid.NewGuid().ToString("N")))
                .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithBody(
                        SyntaxFactory.Block());

            _mainType.AddMember(method);
            return method;
        }
    }
}
