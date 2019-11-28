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
using Pickaxe.Sdk;
using System.Collections.Generic;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(Program program) //generate namespace/class definition
        {
            _unit = _unit.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Pickaxe.Runtime")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Pickaxe.Runtime.Dom")),
               // SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Linq")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic"))
                );

            var mainNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(""));

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Run")
               .WithModifiers(
                SyntaxFactory.TokenList(
                   SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                   )
               );

            var runStatements = new List<StatementSyntax>();
            runStatements.Add(SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("InitProxies"))));

            _mainType.ConstructorAddParameters(
                SyntaxFactory.Parameter(
                    SyntaxFactory.Identifier("args"))
                    .WithType(
                    SyntaxFactory.ArrayType(
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                            .AddRankSpecifiers(
                        SyntaxFactory.ArrayRankSpecifier(
                            SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                SyntaxFactory.OmittedArraySizeExpression())))));

            _mainType.ConstructorAddBaseArgs(
                SyntaxFactory.ConstructorInitializer(
                    SyntaxKind.BaseConstructorInitializer,
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxFactory.IdentifierName("args"))))));

            foreach (var child in program.Children)
            {
                var arg = VisitChild(child);
                runStatements.AddRange(arg.ParentStatements);
            }

            var stepMethod = CreateStepMethod();
            runStatements.Add(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(stepMethod.Identifier))));

            stepMethod = stepMethod.AddBodyStatements(CallOnProgressComplete());

            _mainType.ConstructorStatement(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName("TotalOperations"),
                        SyntaxFactory.BinaryExpression(
                            SyntaxKind.AddExpression,
                            SyntaxFactory.IdentifierName("TotalOperations"),
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.NumericLiteralExpression,
                                SyntaxFactory.Literal(_totalOperations))
                                ))));

            method = method.WithBody(SyntaxFactory.Block(
                 runStatements
             ));

            _mainType.AddMember(stepMethod);
            _mainType.AddMember(method);
            _mainType.AddMember(_mainType.GetConstructor());
            mainNamespace = mainNamespace.AddMembers(_mainType.GetClassDeclaration());
            _unit = _unit.AddMembers(mainNamespace);
        }
    }
}
