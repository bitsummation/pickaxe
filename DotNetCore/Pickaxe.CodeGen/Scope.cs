﻿/* Copyright 2015 Brock Reeve
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
using System;
using System.Collections.Generic;

namespace Pickaxe.CodeDom
{
    internal class Scope : IDisposable
    {
        protected Dictionary<string, IScopeData> _scope;
        private Scope _parent;
        private CodeDomTypeDefinition _mainType;

        public static Scope Current { get; private set; }

        public string ScopeGuid { get; private set; }
        public string ScopeIdentifier { get; private set; }
        public CodeDomTypeDefinition Type { get; private set; }

        protected Scope(CodeDomTypeDefinition mainType)
        {
            _scope = new Dictionary<string, IScopeData>();
            ScopeGuid = Guid.NewGuid().ToString("N");
            ScopeIdentifier = "Scope_" +  ScopeGuid;
            if (mainType != null)
            {
                _mainType = mainType;
                CreateType(mainType);
            }

            JoinMembers = new List<MemberDeclarationSyntax>();
        }

        private void CreateType(CodeDomTypeDefinition mainType)
        {
            Type = new CodeDomTypeDefinition(ScopeIdentifier);
            Type.SetModifier(SyntaxKind.PrivateKeyword);

            mainType.AddMember(SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName(ScopeIdentifier))
                        .AddVariables(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier("_" + ScopeIdentifier))
                                )).
                                WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword))));

            mainType.ConstructorStatement(SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName("_" + ScopeIdentifier),
                    SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.IdentifierName(ScopeIdentifier))
                        .WithArgumentList(
                        SyntaxFactory.ArgumentList()))));
        }

        public static void Reset()
        {
            Current = null;
        }

        public static Scope Push(CodeDomTypeDefinition mainType)
        {
            var scope = new Scope(mainType);
            scope._parent = Current;
            Current = scope;
            return scope;
        }

        public static Scope PushSelect()
        {
            var scope = new SelectScope();
            scope._parent = Current;
            Current = scope;
            return scope;
        }

        private void Pop()
        {
            if (Type != null)
            {
                Type.AddMember(Type.GetConstructor());
                _mainType.AddMember(Type.GetClassDeclaration());
            }

            Current = Current._parent;
        }

        public static ScopeData<TableDescriptor> EmptyTableDescriptor = new ScopeData<TableDescriptor>() {Type = new TableDescriptor(null) };

        public void Register(string variable, IScopeData data)
        {
            _scope[variable] = data;
        }

        public void RegisterPrimitive(string variable, Type type, TypeSyntax typeSyntax)
        {
            _scope[variable] = new ScopeData<Type> { Type = type, TypeSyntax = typeSyntax};
        }

        public void RegisterTable(string variable, TableDescriptor descriptor, TypeSyntax typeSyntax)
        {
            _scope[variable] = new ScopeData<TableDescriptor> { Type = descriptor, TypeSyntax = typeSyntax};
        }

        private static Scope FindScope(Scope scope, string variable)
        {
            if (scope == null)
                return null;

            if (scope._scope.ContainsKey(variable))
                return scope;

            return FindScope(scope._parent, variable);
        }

        public static Scope FindScope(string variable)
        {
            return FindScope(Scope.Current, variable);
        }

        public bool IsCurrentScopeRegistered(string variable)
        {
            return _scope.ContainsKey(variable);
        }

        public bool IsRegistered(string variable)
        {
            var scope = FindScope(variable);
            return scope != null;
        }

        public bool IsPrimitiveRegistered(string variable)
        {
            var scope = FindScope(variable);
            if (scope != null && scope._scope.ContainsKey(variable))
                return (scope._scope[variable] is ScopeData<Type>);
            
            return false;
        }

        public bool IsTableRegistered(string variable)
        {
            var scope = FindScope(variable);
            if (scope != null && scope._scope.ContainsKey(variable))
                return (scope._scope[variable] is ScopeData<TableDescriptor> /*&& scope._scope[variable].CodeDomReference.TypeArguments.Count > 0*/);

            return false;
        }

        public bool IsTableRowRegistered(string variable)
        {
            var scope = FindScope(variable);
            if (scope != null && scope._scope.ContainsKey(variable))
                return (scope._scope[variable] is ScopeData<TableDescriptor> /*&& scope._scope[variable].CodeDomReference.TypeArguments.Count == 0*/ );

            return false;
        }

        public IScopeData GetScope(string variable)
        {
            var scope = FindScope(variable);
            return scope._scope[variable];
        }

        public ScopeData<TableDescriptor> GetTableDescriptor(string variable)
        {
            var scope = FindScope(variable);
            return scope._scope[variable] as ScopeData<TableDescriptor>;
        }

        public virtual bool IsSelect
        {
            get
            {
                return false;
            }
        }


        public IList<MemberDeclarationSyntax> JoinMembers { get; set; }

        public virtual SelectMatch[] FindTableVariable(string variable)
        {
            return new SelectMatch[0];
        }

        public virtual SelectMatch[] FindAll()
        {
            return new SelectMatch[0];
        }

        public virtual string[] AliasType<TType>()
        {
            return new string[0];
        }

        public virtual Type FindTypeWithAlias(string alias)
        {
            return null;
        }

        public virtual ExpressionSyntax CreateExpression(string variable)
        {
            return SyntaxFactory.IdentifierName("_" + ScopeIdentifier + "." + variable);
        }

        public void Dispose()
        {
            Pop();
        }
    }
}
