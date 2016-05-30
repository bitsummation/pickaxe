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
    public class JSTableHint : AstNode
    {
        public string CssWaitElement
        {
            get
            {
                string cssWaitElement = null;
                var literal = Children.Where(x => x.GetType() == typeof(StringLiteral)).Cast<StringLiteral>().SingleOrDefault();
                if (literal != null)
                    cssWaitElement = literal.Value;

                return cssWaitElement;
            }
        }

        public int CssTimeoutSeconds
        {
            get
            {
                int cssTimeout = 5;
                var literal = Children.Where(x => x.GetType() == typeof(IntegerLiteral)).Cast<IntegerLiteral>().SingleOrDefault();
                if (literal != null)
                    cssTimeout = literal.Value;

                return cssTimeout;
            }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
