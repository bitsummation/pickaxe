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

using System.Collections.Generic;
using System.Linq;

namespace Pickaxe.Sdk
{
    public class PickStatement : AstNode
    {
        public string Selector
        {
            get
            {
                return Children.Where(x => x.GetType() == typeof(StringLiteral)).Cast<StringLiteral>().Single().Value;
            }
        }

        public MatchExpression[] Matches
        {
            get
            {
                return Children.Where(x => x.GetType() == typeof(MatchExpression)).Cast<MatchExpression>().ToArray();
            }
        }

        public AstNode TakeStatement
        {
            get
            {
                var takeCount = Children.Where(x => x.GetType() != typeof(MatchExpression)
                    && x.GetType() != typeof(StringLiteral)).Count();

                if (takeCount == 0) //we default to taking text if nothing specified
                    Children.Add(new TakeTextStatement());

                return Children.Where(x => x.GetType() != typeof(MatchExpression)
                    && x.GetType() != typeof(StringLiteral)).Single();
            }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
