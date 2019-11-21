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
        public void Visit(Block block)
        {
            var method = CreateBlockMethod();

            using (Scope.Push(_mainType))
            {
                foreach (var child in block.Children)
                {
                    var arg = VisitChild(child);
                    if (arg.ParentStatements.Count > 0)
                    {
                        var stepMethod = CreateStepMethod();
                        stepMethod.Statements.AddRange(arg.ParentStatements);
                        if (_codeStack.Peek().Tag == null)
                            CallOnProgress(stepMethod.Statements);
                        
                        method.Statements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, stepMethod.Name)));
                    }
                }
            }

            _codeStack.Peek().ParentStatements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, method.Name)));
        }
    }
}
