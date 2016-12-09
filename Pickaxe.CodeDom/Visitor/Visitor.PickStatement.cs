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

using Pickaxe.Sdk;
using System;
using System.CodeDom;
using Pickaxe.CodeDom.Semantic;
using Pickaxe.Runtime;
using Pickaxe.Runtime.Dom;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        private void VerifyCssSelector(string selector, Semantic.LineInfo lineInfo)
        {
            if (string.IsNullOrEmpty(selector))
                return;

            HtmlDoc doc = Config.DomFactory.Create();
            bool valid = doc.ValidateCss(selector);
            if(!valid)
                Errors.Add(new BadCssSelector(selector, lineInfo));
        }

        public void Visit(PickStatement statement)
        {
            VerifyCssSelector(statement.Selector, new Semantic.LineInfo(statement.Line.Line, statement.Line.CharacterPosition));

            if (!string.IsNullOrEmpty(statement.Selector))
            {
                _codeStack.Peek()
                    .ParentStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("result"),
                        "AddColumn",
                        new CodePrimitiveExpression(statement.Selector)));
            }

            var expression = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("node"),
                "Pick",
                new CodePrimitiveExpression(statement.Selector)
                );


            var takeDomArg = VisitChild(statement.TakeStatement);
            var takeExpression = takeDomArg.CodeExpression as CodeMethodInvokeExpression;
            takeExpression.Method.TargetObject = expression;

            var aggExpression = takeExpression;
            foreach (var match in statement.Matches)
            {
                if (match.Replace != null)
                {
                    aggExpression = new CodeMethodInvokeExpression(aggExpression, "MatchReplace", new CodePrimitiveExpression(match.Value), new CodePrimitiveExpression(match.Replace.Value));
                }
                else // no replace
                {
                    aggExpression = new CodeMethodInvokeExpression(aggExpression, "Match", new CodePrimitiveExpression(match.Value));
                }
            }

            _codeStack.Peek().Tag = true;
            _codeStack.Peek().CodeExpression = aggExpression;
            _codeStack.Peek().Scope = new ScopeData<Type> { Type = typeof(string), CodeDomReference = new CodeTypeReference(typeof(string)) };
        }
    }
}
