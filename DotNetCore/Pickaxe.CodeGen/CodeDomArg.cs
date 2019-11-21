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

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Pickaxe.CodeDom
{
    internal class CodeDomArg
    {
        public CodeDomArg() : this(null, null)
        {
        }

        public CodeDomArg(ExpressionSyntax codeExpression)
            : this(codeExpression, null)
        {
        }

        public CodeDomArg(ExpressionSyntax codeExpression, IScopeData scope)
        {
            MethodIdentifier = Guid.NewGuid().ToString("N");
            CodeExpression = codeExpression;
            Scope = scope;

            ParentMemberDefinitions = new List<MemberDeclarationSyntax>();
            ParentStatements = new List<StatementSyntax>();
        }

        public object Tag { get; set; }

        public List<MemberDeclarationSyntax> ParentMemberDefinitions { get; set; }
        public List<StatementSyntax> ParentStatements { get; set; }

        public string MethodIdentifier { get; private set; }
        public ExpressionSyntax CodeExpression { get; set; }
        public IScopeData Scope { get; set; }

    }
}
