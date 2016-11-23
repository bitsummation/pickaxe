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
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;

namespace Pickaxe.CodeDom
{
    internal class Scope : IDisposable
    {
        protected Dictionary<string, IScopeData> _scope;
        private Scope _parent;

        public static Scope Current { get; private set; }

        protected Scope(CodeDomTypeDefinition mainType)
        {
            _scope = new Dictionary<string, IScopeData>();
            ScopeIdentifier = "Scope_" +  Guid.NewGuid().ToString("N");
            if(mainType != null)
                CreateType(mainType);

            JoinMembers = new List<CodeMemberField>();
        }

        private void CreateType(CodeDomTypeDefinition mainType)
        {
            Type = new CodeDomTypeDefinition(ScopeIdentifier);
            Type.Type.TypeAttributes = TypeAttributes.NestedPrivate;

            var classType = new CodeTypeReference(ScopeIdentifier);
            mainType.Type.Members.Add(
                new CodeMemberField(classType, "_" + ScopeIdentifier));

            mainType.Constructor.Statements.Add(new CodeAssignStatement(
              new CodeSnippetExpression("_" + ScopeIdentifier),
              new CodeObjectCreateExpression(classType)));

            mainType.Type.Members.Add(Type.Type);
        }

        public string ScopeIdentifier { get; private set; }
        public CodeDomTypeDefinition Type { get; private set; }

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
            Current = Current._parent;
        }

        public static ScopeData<TableDescriptor> EmptyTableDescriptor = new ScopeData<TableDescriptor>() {Type = new TableDescriptor(null) };

        public void Register(string variable, IScopeData data)
        {
            _scope[variable] = data;
        }

        public void RegisterPrimitive(string variable, Type type, CodeTypeReference codeDomType)
        {
            _scope[variable] = new ScopeData<Type> { Type = type, CodeDomReference = codeDomType };
        }

        public void RegisterTable(string variable, TableDescriptor descriptor, CodeTypeReference codeDomType)
        {
            _scope[variable] = new ScopeData<TableDescriptor> { Type = descriptor, CodeDomReference = codeDomType };
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
                return (scope._scope[variable] is ScopeData<TableDescriptor> && scope._scope[variable].CodeDomReference.TypeArguments.Count > 0);

            return false;
        }

        public bool IsTableRowRegistered(string variable)
        {
            var scope = FindScope(variable);
            if (scope != null && scope._scope.ContainsKey(variable))
                return (scope._scope[variable] is ScopeData<TableDescriptor> && scope._scope[variable].CodeDomReference.TypeArguments.Count == 0);

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


        public IList<CodeMemberField> JoinMembers { get; set; }

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

        public virtual CodeExpression CreateExpression(string variable)
        {
            return new CodeVariableReferenceExpression("_" + ScopeIdentifier + "." + variable);
        }

        public void Dispose()
        {
            Pop();
        }
    }
}
