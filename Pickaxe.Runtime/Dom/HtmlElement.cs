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

namespace Pickaxe.Runtime.Dom
{
    public abstract class HtmlElement
    {
        internal HtmlElement(object handle)
        {
            if (handle == null)
                throw new InvalidOperationException("handle should not be null");

            Cache = new Dictionary<string, CacheElement>();
            Handle = handle;
        }

        internal Dictionary<string, CacheElement> Cache { get; private set; } 

        internal object Handle { get; set; }

        public abstract HtmlElement[] QuerySelectorAll(string cssSelector);
        public abstract HtmlElement QuerySelector(string cssSelector);

        public abstract bool AttributeExists(string attr);
        internal abstract string TakeAttribute(string attr);
        internal abstract string TakeText();
        internal abstract string TakeHtml();

        internal abstract void Clear();
    }
}
