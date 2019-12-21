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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Runtime
{
    public class DynamicObject : IRow
    {
        private Dictionary<string, string> _properties;

        public DynamicObject()
        {
            _properties = new Dictionary<string, string>();
        }

        public void Add(string property, string value)
        {
            _properties.Add(property, value);
        }

        public virtual string this[string prop]
        {
            get
            {
                string returnValue = null;
                if (_properties.ContainsKey(prop))
                    returnValue = _properties[prop];

                return returnValue;
            }
        }

        public static TableDescriptor Columns
        {
            get
            {
                var variablePair = new List<VariableTypePair>();
                return new TableDescriptor(typeof(DynamicObject)) { Variables = variablePair };
            }
        }

    }
}
