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
using Pickaxe.Sdk;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(Program program) //generate namespace/class definition
        {
            var mainNamespace = new CodeNamespace("");
            mainNamespace.Imports.Add(new CodeNamespaceImport("System"));
            mainNamespace.Imports.Add(new CodeNamespaceImport("HtmlAgilityPack"));
            mainNamespace.Imports.Add(new CodeNamespaceImport("Pickaxe.Runtime"));
            mainNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            mainNamespace.Types.Add(_mainType.Type);

            CodeMemberMethod method = new CodeMemberMethod();
            _runMethodStatements = method.Statements;
            method.Name = "Run";
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            _runMethodStatements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, "InitProxies")));
            
            _mainType.Type.Members.Add(method);
            _unit.Namespaces.Add(mainNamespace);

            foreach (var child in program.Children)
            {
                var arg = VisitChild(child);
                if (arg.ParentStatements.Count > 0)
                {
                    CreateStepMethod();
                    _stepMethodStatements.AddRange(arg.ParentStatements);
                    CallOnProgress(_stepMethodStatements);

                }
            }

            CreateStepMethod();
            CallOnProgressComplete(_stepMethodStatements);

            _mainType.Constructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(null, "TotalOperations"),
                    new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(null, "TotalOperations"),
                    CodeBinaryOperatorType.Add,
                    new CodePrimitiveExpression(_totalOperations))
                    ));
        }
    }
}
