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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Runtime
{
    public class ResultRow : IEnumerable<object>, IRow
    {
        private IList<object> _columns;

        public ResultRow(int columnSize)
        {
            _columns = new object[columnSize];
        }

        public object this[int index]
        {
            get
            {
                return _columns[index];
            }
            set
            {
                _columns[index] = value;
            }
        }

        public int ColumnCount
        {
            get
            {
                return _columns.Count;
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
