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

using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Text.RegularExpressions;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pickaxe.Parser.Antlr;
using Pickaxe.Parser.Bridge;
using Pickaxe.Sdk;

namespace Pickaxe.Parser
{
    public class CodeParser
    {
        private string _code;

        public CodeParser(string code)
        {
            _code = code;
            Errors = new List<ParseException>();
        }

        public IList<ParseException> Errors { get; private set; }

        public Program Parse()
        {
            var lexer = new AntlrLexer(new ANTLRStringStream(_code));
            var tokens = new CommonTokenStream(lexer);
           
            var parser = new AntlrParser(tokens);
            parser.TreeAdaptor = new Adapter();
            var result = parser.program();

            lexer.Errors.ForEach(x => Errors.Add(x));
            parser.Errors.ForEach(x => Errors.Add(x));
            if (Errors.Any())
                return null;

            var bridge = new BridgeVisitor(result.Tree);
            return bridge.CreateAst();
        }

        private class Adapter : CommonTreeAdaptor
        {
            public override object Create(IToken payload)
            {
                if (payload != null)
                {
                    if (payload.Type == ScrapeParser.PROGRAM)
                        return new AntlrBridgeTree<Program>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.FILE_TABLE)
                        return new AntlrBridgeTree<FileTable>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.BUFFER_TABLE)
                        return new AntlrBridgeTree<BufferTable>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    if (payload.Type == ScrapeParser.INSERT_INTO)
                        return new AntlrBridgeTree<InsertIntoStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.INSERT_OVERWRITE)
                        return new AntlrBridgeTree<InsertOverwriteStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.INSERT_INTO_DIRECTORY)
                        return new AntlrBridgeTree<InsertIntoDirectoryStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.SELECT_ARG)
                        return new AntlrBridgeTree<SelectArg>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.SELECT_STATEMENT)
                        return new AntlrBridgeTree<SelectStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.PICK)
                        return new AntlrBridgeTree<PickStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.FROM)
                        return new AntlrBridgeTree<FromStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.WHERE)
                        return new AntlrBridgeTree<WhereStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    
                    if(payload.Type == ScrapeParser.PROXIES)
                        return new AntlrBridgeTree<ProxyStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.PROXY_LIST)
                        return new AntlrBridgeTree<ProxyList>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    if(payload.Type == ScrapeParser.EXPAND)
                        return new AntlrBridgeTree<ExpandExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.EXPAND_INTERATION_VARIABLE)
                        return new AntlrBridgeTree<ExpandIterationVariable>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.DOWNLOAD_PAGE)
                        return new AntlrBridgeTree<DownloadPageExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.DOWNLOAD_IMAGE)
                        return new AntlrBridgeTree<DownloadImageExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    if(payload.Type == ScrapeParser.EACH)
                        return new AntlrBridgeTree<EachStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.BLOCK)
                        return new AntlrBridgeTree<Block>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    if (payload.Type == ScrapeParser.VARIABLE_REFERENCE)
                        return new AntlrBridgeTree<VariableReferance>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.TABLE_MEMBER_REFERENCE)
                        return new AntlrBridgeTree<TableMemberReference>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.TABLE_VARIABLE_REFERENCE)
                        return new AntlrBridgeTree<TableVariableReference>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.INT)
                        return new AntlrBridgeTree<IntegerLiteral>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.STRING_LITERAL)
                        return new AntlrBridgeTree<StringLiteral>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.NULL)
                        return new AntlrBridgeTree<NullLiteral>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    if(payload.Type == ScrapeParser.SELECT_ID)
                        return new AntlrBridgeTree<SelectId>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.SELECT_ALL)
                        return new AntlrBridgeTree<SelectAll>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    if(payload.Type == ScrapeParser.TAKE_ATTRIBUTE)
                        return new AntlrBridgeTree<TakeAttributeStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.TAKE_TEXT)
                        return new AntlrBridgeTree<TakeTextStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.TAKE_HTML)
                        return new AntlrBridgeTree<TakeHtmlStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    if(payload.Type == ScrapeParser.VARIABLE_DECLARATION)
                        return new AntlrBridgeTree<VariableDeclarationStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    //case statement
                    if (payload.Type == ScrapeParser.CASE_VAR)
                        return new AntlrBridgeTree<CaseVariableStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.WHEN_LITERAL_STATEMENT)
                        return new AntlrBridgeTree<WhenLiteralStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.CASE_EXPRESSION)
                        return new AntlrBridgeTree<CaseExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.CASE_BOOL)
                        return new AntlrBridgeTree<CaseBooleanStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.WHEN_BOOL_STATEMENT)
                        return new AntlrBridgeTree<WhenBooleanStatement>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    
                    if(payload.Type == ScrapeParser.AND)
                        return new AntlrBridgeTree<AndExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.OR)
                        return new AntlrBridgeTree<OrExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.LESSTHAN)
                        return new AntlrBridgeTree<LessThanExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.LESSTHANEQUAL)
                        return new AntlrBridgeTree<LessThanEqualExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.GREATERTHAN)
                        return new AntlrBridgeTree<GreaterThanExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.GREATERTHANEQUAL)
                        return new AntlrBridgeTree<GreaterThanEqualExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.NOTEQUAL)
                        return new AntlrBridgeTree<NotEqualExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeParser.EQUALS)
                        return new AntlrBridgeTree<EqualsExpression>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    //Math operators
                    if(payload.Type == ScrapeParser.PLUS)
                        return new AntlrBridgeTree<AdditionOperator>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.ASTERISK)
                        return new AntlrBridgeTree<MultiplicatonOperator>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.MINIS)
                        return new AntlrBridgeTree<SubtrationOperator>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if (payload.Type == ScrapeParser.DIV)
                        return new AntlrBridgeTree<DivisionOperator>(payload, (node, tree, visitor) => visitor.Visit(node, tree));

                    if(payload.Type == ScrapeParser.PROCEDURE)
                        return new AntlrBridgeTree<ProcedureDefinition>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                    if(payload.Type == ScrapeLexer.EXEC)
                        return new AntlrBridgeTree<ProcedureCall>(payload, (node, tree, visitor) => visitor.Visit(node, tree));
                }

                return new CommonTree(payload);
            }
        }
        
    }
}
