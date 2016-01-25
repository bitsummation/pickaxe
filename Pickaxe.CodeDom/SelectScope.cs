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
using System.Linq;
using System.Text;

namespace Pickaxe.CodeDom
{
    internal class SelectScope : Scope
    {
        public SelectScope() : base(null)
        {
        }

        public override SelectMatch[] FindTableVariable(string variable)
        {
            var tableList = new List<SelectMatch>();
            foreach(var t in _scope)
            {
                var descriptor = GetTableDescriptor(t.Key);
                foreach(var var in descriptor.Type.Variables)
                {
                    if (var.Variable == variable) //we found the variable in a table
                        tableList.Add(new SelectMatch{TableAlias = t.Key, TableVariable = var});
                }
            }

            return tableList.ToArray();
        }

        public override SelectMatch[] FindAll()
        {
            var tableList = new List<SelectMatch>();
            foreach (var t in _scope)
            {
                var descriptor = GetTableDescriptor(t.Key);
                foreach(var var in descriptor.Type.Variables)
                    tableList.Add(new SelectMatch { TableAlias = t.Key, TableVariable = var });
            }

            return tableList.ToArray();
        }

        public override string[] AliasType<TType>()
        {
            var aliases = new List<string>();
            foreach(var t in _scope)
            {
                var descriptor = GetTableDescriptor(t.Key);
                if(descriptor.Type.RunTimeType != null && descriptor.Type.RunTimeType == typeof(TType))
                {
                    aliases.Add(t.Key);
                }
            }

            return aliases.ToArray();
        }

        public override CodeExpression CreateExpression(string variable)
        {
            return new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("row"), variable);
        }
    }
}
