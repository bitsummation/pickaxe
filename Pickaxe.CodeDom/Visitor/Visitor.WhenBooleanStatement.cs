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

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(WhenBooleanStatement statement)
        {
            var arg = VisitChild(statement.Expression, new CodeDomArg() { Scope = _codeStack.Peek().Scope });
            if (arg.Tag != null)
                _codeStack.Peek().Tag = arg.Tag;

            var condition = new CodeConditionStatement();
            condition.Condition = arg.CodeExpression;

            var then = VisitChild(statement.Then, new CodeDomArg() { Scope = _codeStack.Peek().Scope });
            condition.TrueStatements.Add(new CodeMethodReturnStatement(then.CodeExpression));
            if (arg.Tag != null)
                _codeStack.Peek().Tag = arg.Tag;

            _codeStack.Peek().ParentStatements.Add(condition);
        }
    }
}
