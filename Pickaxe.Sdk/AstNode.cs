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

namespace Pickaxe.Sdk
{
    public abstract class AstNode
    {
        protected AstNode()
        {
            Children = new ChildCollection(this);
        }

        public LineInfo Line { get; set; }
        public AstNode Parent { get; set; }
        public ChildCollection Children { get; private set; } 
        public abstract void Accept(IAstVisitor visitor);
    }
}
