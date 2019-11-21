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
    public class DynamicObjectWrapper : DynamicObject
    {
        private ThreadedDownloadTable<DynamicObject> _parent;
        private DynamicObject _inner;
        
        public DynamicObjectWrapper(ThreadedDownloadTable<DynamicObject> parent)
        {
            _parent = parent;
        }

        protected DynamicObject Inner
        {
            get
            {
                _parent.Process();
                if (_inner == null)
                    _inner = _parent.GetResult();

                return _inner;
            }
            set
            {
                _inner = null;
            }
        }

        public override string this[string prop]
        {
            get
            {
                return Inner[prop];
            }
        }

    }
}
