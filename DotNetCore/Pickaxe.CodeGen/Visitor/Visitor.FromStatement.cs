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
        private MethodDeclarationSyntax CreateFetch(AliasBase aliasBase, out TypeSyntax anonType)
        {
            var statementDomArg = VisitChild(aliasBase.Statement);
            if (Scope.Current.IsTableRegistered(statementDomArg.Scope.GenericTypeSyntax.TypeArgumentList.Arguments[0].GetText().ToString()))
            {
                var scope = Scope.Current.GetTableDescriptor(statementDomArg.Scope.GenericTypeSyntax.TypeArgumentList.Arguments[0].GetText().ToString());
                if (aliasBase.Alias == null)
                    aliasBase.Children.Add(new TableAlias { Id = statementDomArg.Scope.GenericTypeSyntax.TypeArgumentList.Arguments[0].GetText().ToString() });

                Scope.Current.Register(aliasBase.Alias.Id, new ScopeData<TableDescriptor> { Type = scope.Type, TypeSyntax = scope.TypeSyntax });
            }

            var method = SyntaxFactory.MethodDeclaration(
                  SyntaxFactory.PredefinedType(
                      SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                   SyntaxFactory.Identifier("Fetch_" + Guid.NewGuid().ToString("N")))
                   .WithModifiers(
                   SyntaxFactory.TokenList(
                       SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                       .WithBody(
                   SyntaxFactory.Block());

            var methodStatements = new List<StatementSyntax>();

            var anon = "anon_" + Guid.NewGuid().ToString("N");
            var bufferTable = SyntaxFactory.ClassDeclaration(anon).WithModifiers(
             SyntaxFactory.TokenList(
                 SyntaxFactory.Token(SyntaxKind.PrivateKeyword)
                 )
             );

            bufferTable = bufferTable.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("IRow")));
            var field = SyntaxFactory.FieldDeclaration(
                            SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.IdentifierName(statementDomArg.Scope.GenericTypeSyntax.TypeArgumentList.Arguments[0].GetText().ToString()))
                            .AddVariables(
                                    SyntaxFactory.VariableDeclarator(
                                        SyntaxFactory.Identifier(aliasBase.Alias == null ? string.Empty : aliasBase.Alias.Id))))
                        .WithModifiers(
                            SyntaxFactory.TokenList(
                                SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            bufferTable = bufferTable.AddMembers(field);

            anonType = SyntaxFactory.IdentifierName(anon);
            Scope.Current.JoinMembers.Add(field);

            methodStatements.Add(
                SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(statementDomArg.Scope.GenericTypeSyntax)
                    .AddVariables(
                        SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier("table"))
                            .WithInitializer(
                            SyntaxFactory.EqualsValueClause(
                                statementDomArg.CodeExpression))))
                                );
          

            var copyStatments = new List<StatementSyntax>();

            copyStatments.Add(
                SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(anonType)
                    .AddVariables(
                        SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier("t"))
                            .WithInitializer(
                            SyntaxFactory.EqualsValueClause(
                                SyntaxFactory.ObjectCreationExpression(anonType)
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList())))))
                                    );

            copyStatments.Add(
            SyntaxFactory.ExpressionStatement(
                                SyntaxFactory.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("t"),
                                        SyntaxFactory.IdentifierName(field.Declaration.Variables[0].Identifier.Text)),
                                    SyntaxFactory.IdentifierName("o")))
                                    );
            copyStatments.Add(
                SyntaxFactory.ReturnStatement(
                    SyntaxFactory.IdentifierName("t"))
                    );


            var codeParams = SyntaxFactory.ParameterList(
                SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                SyntaxFactory.Parameter(
                                    SyntaxFactory.Identifier("o"))
                                .WithType(
                                    SyntaxFactory.IdentifierName(statementDomArg.Scope.GenericTypeSyntax.TypeArgumentList.Arguments[0].GetText().ToString()))));

            var copyMethod = CreateCopyMethod(codeParams, copyStatments);

            methodStatements.Add(SyntaxFactory.ReturnStatement(
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("table"),
                                        SyntaxFactory.IdentifierName("Select")))
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                            SyntaxFactory.Argument(
                                                SyntaxFactory.SimpleLambdaExpression(
                                                    SyntaxFactory.Parameter(
                                                        SyntaxFactory.Identifier("o")),
                                                    SyntaxFactory.Block(
                                                        SyntaxFactory.SingletonList<StatementSyntax>(
                                                            SyntaxFactory.ReturnStatement(
                                                                SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.IdentifierName(copyMethod.Identifier.Text))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList(
                                                                        SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                                            SyntaxFactory.Argument(
                                                                                SyntaxFactory.IdentifierName("o"))))))))))))))
                                                                                );

            _mainType.AddMember(bufferTable);

            method = method.WithReturnType(SyntaxFactory.GenericName(
                      SyntaxFactory.Identifier("IEnumerable"))
                      .AddTypeArgumentListArguments(anonType));

            copyMethod = copyMethod.WithReturnType(anonType);

            method = method.WithBody(SyntaxFactory.Block(
                        methodStatements
                        ));

            _mainType.AddMember(copyMethod);
            _mainType.AddMember(method);
            return method;
        }

        private MethodDeclarationSyntax CreateCopyMethod(ParameterListSyntax methodParams, List<StatementSyntax> statements)
        {
            return SyntaxFactory.MethodDeclaration(
                           SyntaxFactory.PredefinedType(
                               SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                           SyntaxFactory.Identifier("Copy_" + Guid.NewGuid().ToString("N")))
                       .WithModifiers(
                           SyntaxFactory.TokenList(
                               SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                       .WithParameterList(methodParams)
                       .WithBody(
                        SyntaxFactory.Block(statements));
        }
        

        public void Visit(FromStatement statement)
        {
            var method = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(
                        SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    SyntaxFactory.Identifier("From_" + Guid.NewGuid().ToString("N")))
                    .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                        .WithBody(
                    SyntaxFactory.Block());

            var methodStatements = new List<StatementSyntax>();  
            GenerateCallStatement(methodStatements, statement.Line.Line);

            TypeSyntax anonType;
            var fetchMethod = CreateFetch(statement, out anonType);

            methodStatements.Add(SyntaxFactory.LocalDeclarationStatement(
                                SyntaxFactory.VariableDeclaration(SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("IEnumerable"))
                                    .AddTypeArgumentListArguments(
                                    anonType))
                                .AddVariables(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("join"))
                                .WithInitializer(SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(fetchMethod.Identifier)
                                                )))))
                                                );

            if(statement.Join != null)
            {
                var args = VisitChild(statement.Join, new CodeDomArg() {Scope = new ScopeData<Type> { Type = typeof(int), TypeSyntax = anonType} });

                methodStatements.Add(SyntaxFactory.ReturnStatement(args.CodeExpression));
                method = method.WithReturnType(args.Scope.TypeSyntax);

                _codeStack.Peek().Scope = args.Scope;
            }
            else
            {
                var tableType = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier("CodeTable"))
                    .AddTypeArgumentListArguments(anonType);

                method = method.WithReturnType(tableType);
                _codeStack.Peek().Scope = new ScopeData<Type> { Type = typeof(int), TypeSyntax = tableType };

                methodStatements.Add(SyntaxFactory.LocalDeclarationStatement(
                                SyntaxFactory.VariableDeclaration(tableType)
                                .AddVariables(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("newTable"))
                                .WithInitializer(SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.ObjectCreationExpression(tableType).AddArgumentListArguments()
                                                ))))
                                                );

                methodStatements.Add(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("newTable"),
                            SyntaxFactory.IdentifierName("SetRows")))
                            .AddArgumentListArguments(SyntaxFactory.Argument(
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("join"),
                                        SyntaxFactory.IdentifierName("ToList")))))
                                                        ));

                methodStatements.Add(SyntaxFactory.ReturnStatement(
                           SyntaxFactory.IdentifierName("newTable"))
                           );
            }

            var methodcall = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(method.Identifier));

            method = method.WithBody(SyntaxFactory.Block(
                           methodStatements
                           ));

            _mainType.AddMember(method);
            _codeStack.Peek().CodeExpression = methodcall;
        }
        
    }
}
