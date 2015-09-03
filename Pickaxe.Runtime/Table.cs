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
    public abstract class Table<TRow> : IEnumerable<TRow> where TRow : IRow
    {
        public Table()
        {
            Rows = new List<TRow>();
        }

        protected IList<TRow> Rows { get; private set; }
        
        public virtual void Add(TRow row)
        {
            Rows.Add(row);
        }

        protected void Clear()
        {
            Rows.Clear();
        }

        public int RowCount
        {
            get { return Rows.Count; }
        }

        public TRow this[int index]
        {
            get
            {
                return Rows[index];
            }
            set
            {
                Rows[index] = value;
            }
        }

        public IEnumerator<TRow> GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
