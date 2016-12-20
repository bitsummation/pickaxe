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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;
using Pickaxe.Parser.Antlr;
using Pickaxe.Sdk;

namespace Pickaxe.Parser.Bridge
{
    internal class BridgeVisitor : IBridgeVisitor
    {
        private CommonTree _root;
        private Program _program;

        public BridgeVisitor(CommonTree root)
        {
            _root = root;
        }

        public Program CreateAst()
        {
            var bridgeRoot = _root as BridgeBase;
            bridgeRoot.Accept(this);

            return _program;
        }

        private void SetLine(AstNode node, ITree tree)
        {
            node.Line = new Sdk.LineInfo(tree.Line, tree.CharPositionInLine);
        }

        private static AstNode Parent(CommonTree tree)
        {
            var parent = (CommonTree)tree.Parent;
            while (parent as BridgeBase == null)
                return Parent((CommonTree)parent);

            var bridgeParent = parent as BridgeBase;
            return bridgeParent.Element;
        }

        private void VisitChildren(CommonTree tree)
        {
            Visit(tree.Children);
        }

        private void Visit(ITree tree)
        {
            var bridge = tree as BridgeBase;
            if (bridge != null)
                bridge.Accept(this);
        }

        private void Visit(IList<ITree> trees)
        {
            if (trees != null)
            {
                foreach (var child in trees)
                    Visit(child);
            }
        }

        public void Visit(Program node, CommonTree tree)
        {
            _program = node;
            VisitChildren(tree);
        }

        private static string ParseLiteral(string literal)
        {
            return Regex.Replace(literal, "'", "");
        }

        public void Visit(MsSqlTable table, CommonTree tree)
        {
            Parent(tree).Children.Add(table);
            SetLine(table, tree);

            table.Variable = tree.Children[0].Text;
            var args = tree.Children[1] as CommonTree;
            foreach (var arg in args.Children)
                table.Children.Add(new TableColumnArg() { Variable = arg.GetChild(0).Text, Type = arg.GetChild(1).Text });

            table.ConnectionString = ParseLiteral(tree.Children[2].GetChild(0).GetChild(0).Text);
            table.Table = ParseLiteral(tree.Children[2].GetChild(1).GetChild(0).Text);
        }

        public void Visit(FileTable table, CommonTree tree)
        {
            Parent(tree).Children.Add(table);
            SetLine(table, tree);

            Debug.Assert(tree.Children[0].Type == AntlrParser.ID);
            Debug.Assert(tree.Children[1].Type == AntlrParser.TABLE_COLUMN_ARGS);
            Debug.Assert(tree.Children[2].Type == AntlrParser.WITH);
            Debug.Assert(tree.Children[3].Type == AntlrParser.LOCATION);

            table.Variable = tree.Children[0].Text;
            var args = tree.Children[1] as CommonTree;
            foreach (var arg in args.Children)
                table.Children.Add(new TableColumnArg() { Variable = arg.GetChild(0).Text, Type = arg.GetChild(1).Text });

            Debug.Assert(tree.Children[2].GetChild(0).Type == AntlrParser.FIELD_TERMINATOR);
            Debug.Assert(tree.Children[2].GetChild(1).Type == AntlrParser.ROW_TERMINATOR);
            table.FieldTerminator = ParseLiteral(tree.Children[2].GetChild(0).GetChild(0).Text);
            table.RowTerminator = ParseLiteral(tree.Children[2].GetChild(1).GetChild(0).Text);

            Visit(tree.Children[3].GetChild(0));
        }

        public void Visit(BufferTable table, CommonTree tree)
        {
            Parent(tree).Children.Add(table);
            SetLine(table, tree);
            Debug.Assert(tree.Children[0].Type == AntlrParser.ID);

            table.Variable = tree.Children[0].Text;
            
            if (tree.Children.Count > 1)
            {
                var args = tree.Children[1] as CommonTree;
                foreach (var arg in args.Children)
                    table.Children.Add(new TableColumnArg() { Variable = arg.GetChild(0).Text, Type = arg.GetChild(1).Text });
            }
        }

        public void Visit(UpdateSetArgs arg, CommonTree tree)
        {
            Parent(tree).Children.Add(arg);
            SetLine(arg, tree);
            VisitChildren(tree);
        }

        public void Visit(UpdateStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(SelectStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(NestedSelectStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(SelectArg arg, CommonTree tree)
        {
            Parent(tree).Children.Add(arg);
            VisitChildren(tree);
        }

        public void Visit(AsExpression expression, CommonTree tree)
        {
            Parent(tree).Children.Add(expression);
            SetLine(expression, tree);
            VisitChildren(tree);

            expression.Alias = tree.GetChild(0).Text;
        }

        public void Visit(PickStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(ReplaceExpression expression, CommonTree tree)
        {
            Parent(tree).Children.Add(expression);
            SetLine(expression, tree);
            VisitChildren(tree);
        }

        public void Visit(MatchExpression expression, CommonTree tree)
        {
            Parent(tree).Children.Add(expression);
            SetLine(expression, tree);
            VisitChildren(tree);
        }

        public void Visit(FromStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(InnerJoinStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(WhereStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(StringLiteral literal, CommonTree tree)
        {
            Parent(tree).Children.Add(literal);
            SetLine(literal, tree);
            literal.Value = ParseLiteral(tree.Text);
        }

        public void Visit(NullLiteral literal, CommonTree tree)
        {
            Parent(tree).Children.Add(literal);
            SetLine(literal, tree);
        }

        public void Visit(IntegerLiteral literal, CommonTree tree)
        {
            Parent(tree).Children.Add(literal);
            SetLine(literal, tree);
            literal.Value = int.Parse(tree.Text);
        }

        public void Visit(FloatLiteral literal, CommonTree tree)
        {
            Parent(tree).Children.Add(literal);
            SetLine(literal, tree);
            literal.Value = float.Parse(tree.Text);
        }

        public void Visit(IdentityVariable identity, CommonTree tree)
        {
            Parent(tree).Children.Add(identity);
            SetLine(identity, tree);

            identity.Id = tree.Text;
        }


        public void Visit(CommandLineVariable variable, CommonTree tree)
        {
            Parent(tree).Children.Add(variable);
            SetLine(variable, tree);

            variable.Id = tree.Text;
        }

        public void Visit(VariableReferance variable, CommonTree tree)
        {
            Parent(tree).Children.Add(variable);
            SetLine(variable, tree);
            
            variable.Id = tree.Text;
        }

        public void Visit(TableVariableReference variable, CommonTree tree)
        {
            Parent(tree).Children.Add(variable);
            SetLine(variable, tree);

            variable.Id = tree.Text;
        }

        public void Visit(TableAlias alias, CommonTree tree)
        {
            Parent(tree).Children.Add(alias);
            SetLine(alias, tree);

            alias.Id = tree.GetChild(0).Text;
        }

        public void Visit(ExpandExpression expression, CommonTree tree)
        {
            Parent(tree).Children.Add(expression);
            SetLine(expression, tree);
            VisitChildren(tree);

            if (tree.ChildCount < 3) //if no expand expression we just add the default iteration variable
            {
                var iterationVariable = new ExpandIterationVariable();
                expression.Children.Add(iterationVariable);
            }
        }

        public void Visit(ExpandIterationVariable variable, CommonTree tree)
        {
            Parent(tree).Children.Add(variable);
        }

        public void Visit(DownloadPageExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(DownloadImageExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(TakeTextStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
        }

        public void Visit(TakeHtmlStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
        }

        public void Visit(TakeAttributeStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            statement.Attriute = ParseLiteral(tree.GetChild(0).Text);
        }

        public void Visit(VariableAssignmentStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(VariableDeclarationStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);   
            statement.Variable = tree.GetChild(0).Text;
            Visit(tree.GetChild(1));
        }

        public void Visit(TruncateTableStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(InsertIntoStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            Visit(tree.GetChild(0));
            Visit(tree.GetChild(1));
        }

        public void Visit(InsertOverwriteStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            Visit(tree.GetChild(0));
            Visit(tree.GetChild(1));
        }

        public void Visit(InsertIntoDirectoryStatement statement, CommonTree tree)
        {
            Parent(tree).Children.Add(statement);
            SetLine(statement, tree);
            VisitChildren(tree);
        }

        public void Visit(SelectId id, CommonTree tree)
        {
            Parent(tree).Children.Add(id);
            id.Id = tree.Text;
            SetLine(id, tree);
        }

        public void Visit(SelectAll all, CommonTree tree)
        {
            Parent(tree).Children.Add(all);
        }

        public void Visit(WhileStatement whileStatement, CommonTree tree)
        {
            Parent(tree).Children.Add(whileStatement);
            SetLine(whileStatement, tree);

            VisitChildren(tree);
        }

        public void Visit(EachStatement eachStatement, CommonTree tree)
        {
            Parent(tree).Children.Add(eachStatement);
            SetLine(eachStatement, tree);

            eachStatement.IterationVariable = new VariableDeclarationStatement() { Variable = tree.GetChild(0).Text };
            SetLine(eachStatement.IterationVariable, tree.GetChild(0));

            var rowGetter = new TableVariableRowGetter() { Id = tree.GetChild(1).Text };
            SetLine(rowGetter, tree.GetChild(1));
            eachStatement.IterationVariable.Children.Add(rowGetter);
            
            eachStatement.TableReference = new TableVariableReference() { Id = tree.GetChild(1).Text };
            SetLine(eachStatement.TableReference, tree.GetChild(1));

            Visit(tree.GetChild(2));
        }

        public void Visit(Block block, CommonTree tree)
        {
            Parent(tree).Children.Add(block);
            VisitChildren(tree);
        }

        public void Visit(TableMemberReference variable, CommonTree tree)
        {
            Parent(tree).Children.Add(variable);
            variable.RowReference = new TableVariableRowReference() { Id = tree.GetChild(0).Text, Parent = variable };
            SetLine(variable.RowReference, tree.GetChild(0));
            variable.Member = tree.GetChild(1).Text;
            SetLine(variable, tree.GetChild(1));
        }

        public void Visit(AdditionOperator op, CommonTree tree)
        {
            Parent(tree).Children.Add(op);
            VisitChildren(tree);
        }

        public void Visit(SubtrationOperator op, CommonTree tree)
        {
            Parent(tree).Children.Add(op);
            VisitChildren(tree);
        }

        public void Visit(MultiplicatonOperator op, CommonTree tree)
        {
            Parent(tree).Children.Add(op);
            VisitChildren(tree);
        }

        public void Visit(DivisionOperator op, CommonTree tree)
        {
            Parent(tree).Children.Add(op);
            VisitChildren(tree);
        }

        public void Visit(ProxyStatement statement, CommonTree tree)
        {
            SetLine(statement, tree);
            Parent(tree).Children.Add(statement);
            VisitChildren(tree);
        }

        public void Visit(ProxyList list, CommonTree tree)
        {
            SetLine(list, tree);
            Parent(tree).Children.Add(list);
            VisitChildren(tree);
        }

        public void Visit(CaseVariableStatement statement, CommonTree tree)
        {
            SetLine(statement, tree);
            Parent(tree).Children.Add(statement);
            VisitChildren(tree);
        }

        public void Visit(WhenLiteralStatement statement, CommonTree tree)
        {
            SetLine(statement, tree);
            Parent(tree).Children.Add(statement);
            VisitChildren(tree);
        }

        public void Visit(CaseExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(CaseBooleanStatement statement, CommonTree tree)
        {
            SetLine(statement, tree);
            Parent(tree).Children.Add(statement);
            VisitChildren(tree);
        }

        public void Visit(NullOperator op, CommonTree tree)
        {
            SetLine(op, tree);
            Parent(tree).Children.Add(op);
            VisitChildren(tree);
        }

        public void Visit(JSTableHint hint, CommonTree tree)
        {
            SetLine(hint, tree);
            Parent(tree).Children.Add(hint);
            VisitChildren(tree);
        }

        public void Visit(ThreadTableHint hint, CommonTree tree)
        {
            SetLine(hint, tree);
            Parent(tree).Children.Add(hint);
            hint.ThreadCount = int.Parse(tree.GetChild(0).Text);
        }

        public void Visit(WhenBooleanStatement statement, CommonTree tree)
        {
            SetLine(statement, tree);
            Parent(tree).Children.Add(statement);
            VisitChildren(tree);
        }

        public void Visit(NodesBooleanExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(NotLikeExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(LikeExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(AndExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(OrExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(LessThanExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(LessThanEqualExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(GreaterThanExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(GreaterThanEqualExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(NotEqualExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(EqualsExpression expression, CommonTree tree)
        {
            SetLine(expression, tree);
            Parent(tree).Children.Add(expression);
            VisitChildren(tree);
        }

        public void Visit(ProcedureDefinition definition, CommonTree tree)
        {
            Parent(tree).Children.Add(definition);
            SetLine(definition, tree);

            definition.Name = tree.Children[0].Text;
            Visit(tree.Children[1]);

            if (tree.Children.Count > 2)
            {
                var args = tree.Children[2] as CommonTree;
                foreach (var arg in args.Children)
                    definition.Children.Add(new TableColumnArg() { Variable = arg.GetChild(0).Text, Type = arg.GetChild(1).Text });
            }
        }

        public void Visit(ProcedureCall call, CommonTree tree)
        {
            Parent(tree).Children.Add(call);
            SetLine(call, tree);

            call.Name = tree.Children[0].Text;
            for(int x = 1; x < tree.ChildCount; x++)
                Visit(tree.Children[x]);
        }

        public void Visit(GetDatePrimitive primitive, CommonTree tree)
        {
            Parent(tree).Children.Add(primitive);
            SetLine(primitive, tree);
        }
    }
}
