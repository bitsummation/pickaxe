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

using HtmlAgilityPack;
using Pickaxe.Sdk;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using Pickaxe.CodeDom.Semantic;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        private void VerifyCssSelector(string selector, Semantic.LineInfo lineInfo)
        {
            if (string.IsNullOrEmpty(selector))
                return;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<html></html>");
            try
            {
                doc.DocumentNode.QuerySelector(selector);
            }
            catch(Exception e)
            {
                Errors.Add(new BadCssSelector(e.Message, lineInfo));
            }
        }

        public void Visit(PickStatement statement)
        {
            VerifyCssSelector(statement.Selector, new Semantic.LineInfo(statement.Line.Line, statement.Line.CharacterPosition));

            _codeStack.Peek()
                .ParentStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("result"),
                    "AddColumn",
                    new CodePrimitiveExpression(statement.Selector)));

            var expression = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("node"),
                "Pick",
                new CodePrimitiveExpression(statement.Selector)
                );


            var takeDomArg = VisitChild(statement.TakeStatement);
            var takeExpression = takeDomArg.CodeExpression as CodeMethodInvokeExpression;
            takeExpression.Method.TargetObject = expression;

            if (statement.Match != null && statement.Match.Replace != null)
            {
                takeExpression = new CodeMethodInvokeExpression(takeExpression, "MatchReplace", new CodePrimitiveExpression(statement.Match.Value), new CodePrimitiveExpression(statement.Match.Replace.Value));
            }
            else if (statement.Match != null)
            {
                takeExpression = new CodeMethodInvokeExpression(takeExpression, "Match", new CodePrimitiveExpression(statement.Match.Value));
            }

            _codeStack.Peek().Tag = true;
            _codeStack.Peek().CodeExpression = takeExpression;
            _codeStack.Peek().Scope = new ScopeData<Type> { Type = typeof(string), CodeDomReference = new CodeTypeReference(typeof(string)) };
        }
    }
}
