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
        public void Visit(SelectArg arg)
        {
            var selectInfo = new SelectArgsInfo();

            var childDomArgs = new List<CodeDomArg>();
            foreach (var childArg in arg.Args)
            {
                var domArgs = VisitChild(childArg, new CodeDomArg() { Scope = _codeStack.Peek().Scope });
                childDomArgs.Add(domArgs);
            }
            
            var expression = childDomArgs[0].CodeExpression;
            var scope = childDomArgs[0].Scope;
            for (int x = 1; x < childDomArgs.Count; x++)
            {
                expression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Helper)), "NullConcat", expression, childDomArgs[x].CodeExpression);
                scope = childDomArgs[x].Scope;
            }

            if (arg.Args.Length > 1) //more than one statement (arg + arg)
                selectInfo.ColumnName = null;
            else if (arg.Args.Length == 1 && childDomArgs[0].Tag != null) //only add column name
                selectInfo.ColumnName = ((SelectArgsInfo)childDomArgs[0].Tag).ColumnName;
            
            foreach (var childDomArg in childDomArgs)
            {
                if (childDomArg.Tag != null && ((SelectArgsInfo)childDomArg.Tag).IsPickStatement)
                    selectInfo.IsPickStatement = true;
            }

            if (arg.As != null)
                selectInfo.ColumnName = arg.As.Alias;

            _codeStack.Peek().Tag = selectInfo;
            _codeStack.Peek().Scope = scope;
            _codeStack.Peek().CodeExpression = expression;
        }
    }
}
