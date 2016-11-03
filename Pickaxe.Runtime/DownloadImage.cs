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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Runtime
{
    public class DownloadImage : IRow
    {
        public string url { get; set; }
        public DateTime? date { get; set; }
        public byte[] image { get; set; }
        public int? size { get; set; }
        public string filename { get; set; }

        public static TableDescriptor Columns
        {
            get
            {
                var propertyInfos = typeof(DownloadImage).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var variablePair = propertyInfos.Select(x => new VariableTypePair() { Variable = x.Name, Primitive = TablePrimitive.FromType(x.PropertyType) }).ToList();
                return new TableDescriptor(typeof(DownloadImage)) { Variables = variablePair };
            }
        }
    }
}
