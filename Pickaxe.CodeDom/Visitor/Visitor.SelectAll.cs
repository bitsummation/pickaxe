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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(SelectAll all)
        {
            var rowType = _codeStack.Peek().Scope.CodeDomReference.TypeArguments[0];

            ScopeData<TableDescriptor> descriptor = Scope.EmptyTableDescriptor;
            if (Scope.Current.IsRegistered(rowType.BaseType))
                descriptor = Scope.Current.GetTableDescriptor(rowType.BaseType);

            all.Parent.Parent.Children.Remove(all.Parent);
            foreach (var var in descriptor.Type.Variables)
            {
                var arg = new SelectArg();
                arg.Children.Add(new SelectId() { Id = var.Variable });
                all.Parent.Parent.Children.Add(arg);
            }
        }

    }
}
