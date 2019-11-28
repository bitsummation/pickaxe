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

namespace Pickaxe.CodeDom
{
    internal class CodeDomTypeDefinition
    {
        public CodeDomTypeDefinition(string typeName)
        {
            var type = SyntaxFactory.ClassDeclaration(typeName).WithModifiers(
              SyntaxFactory.TokenList(
                  SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                  )
              );

            Constructor = SyntaxFactory.ConstructorDeclaration(typeName)
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                  )).WithBody(SyntaxFactory.Block());


            Type = type;
        }

        public void AddBaseType(string baseType)
        {
            Type = Type.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(baseType)));
        }

        public void SetModifier(SyntaxKind kind)
        {
            Type = Type.WithModifiers(SyntaxFactory.TokenList(
                        SyntaxFactory.Token(kind)));
        }

        public void AddMember(MemberDeclarationSyntax member)
        {
            Type = Type.AddMembers(member);
        }

        public void ConstructorAddParameters(ParameterSyntax parameter)
        {
            Constructor = Constructor.AddParameterListParameters(parameter);   
        }

        public void ConstructorAddBaseArgs(ConstructorInitializerSyntax initializer)
        {
            Constructor = Constructor.WithInitializer(initializer);
        }

        public void ConstructorStatement(StatementSyntax statement)
        {
            Constructor = Constructor.AddBodyStatements(statement);
        }

        public ClassDeclarationSyntax GetClassDeclaration()
        {
            return Type;
        }

        public ConstructorDeclarationSyntax GetConstructor()
        {
            return Constructor;
        }

        private ClassDeclarationSyntax Type { get; set; }
        private ConstructorDeclarationSyntax Constructor { get; set; }
    }
}
