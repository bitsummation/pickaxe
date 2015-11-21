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

using System.Linq;

namespace Pickaxe.Sdk
{
    public class FileTable : AstNode
    {
        public string Variable { get; set; }

        public TableColumnArg[] Args
        {
            get { return Children.Where(x => x.GetType() == typeof(TableColumnArg)).Cast<TableColumnArg>().ToArray(); }
        }

        public string FieldTerminator { get; set; }
        public string RowTerminator { get; set; }
        public AstNode Location
        {
            get { return Children.Where(x => x.GetType() != typeof(TableColumnArg)).Single(); }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
