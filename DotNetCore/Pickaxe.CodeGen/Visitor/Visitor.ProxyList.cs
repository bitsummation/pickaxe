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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(ProxyList statement)
        {
            var property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Override | MemberAttributes.Family;
            property.Name = "Proxies";
            property.HasSet = false;
            property.Type = new CodeTypeReference(typeof(string[]));

            property.GetStatements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(string[])), "proxies",
                new CodeArrayCreateExpression(new CodeTypeReference(typeof(string[])), new CodePrimitiveExpression(statement.Proxies.Length)))
                );

            for (int x = 0; x < statement.Proxies.Length; x++ )
            {
                if (!Proxy.TryParse(statement.Proxies[x].Value))
                    Errors.Add(new BadProxyFormat(new Semantic.LineInfo(statement.Proxies[x].Line.Line, statement.Proxies[x].Line.CharacterPosition)));
                
               property.GetStatements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("proxies"),
                    new CodePrimitiveExpression(x)), new CodePrimitiveExpression(statement.Proxies[x].Value))
                    );
            }

            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("proxies")));

            _mainType.Type.Members.Add(property);
        }
    }
}
