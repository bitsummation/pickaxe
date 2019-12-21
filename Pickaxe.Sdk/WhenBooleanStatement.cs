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

namespace Pickaxe.Sdk
{
    public class WhenBooleanStatement : AstNode
    {
        public BooleanExpression Expression
        {
            get
            {
                return Children.Where(x => x.GetType().IsSubclassOf(typeof(BooleanExpression))).Cast<BooleanExpression>().Single();
            }
        }
    
        public CaseExpression Then
        {
            get
            {
                return Children.Where(x => x.GetType() == typeof(CaseExpression)).Cast<CaseExpression>().Single();
            }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
