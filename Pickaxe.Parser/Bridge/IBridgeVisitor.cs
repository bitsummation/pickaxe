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

using Antlr.Runtime.Tree;
using Pickaxe.Sdk;

namespace Pickaxe.Parser.Bridge
{
    internal interface IBridgeVisitor
    {
        void Visit(Program node, CommonTree tree);
        void Visit(FileTable table, CommonTree tree);
        void Visit(BufferTable table, CommonTree tree);

        void Visit(InsertIntoStatement statement, CommonTree tree);
        void Visit(InsertOverwriteStatement statement, CommonTree tree);
        void Visit(InsertIntoDirectoryStatement statement, CommonTree tree);
        void Visit(SelectArg arg, CommonTree tree);
        void Visit(SelectStatement statement, CommonTree tree);
        void Visit(FromStatement statement, CommonTree tree);
        void Visit(WhereStatement statement, CommonTree tree);
        void Visit(PickStatement statement, CommonTree tree);
        void Visit(SelectId id, CommonTree tree);
        void Visit(SelectAll all, CommonTree tree);

        void Visit(ProxyStatement statement, CommonTree tree);
        void Visit(ProxyList list, CommonTree tree);

        void Visit(ExpandExpression expression, CommonTree tree);
        void Visit(ExpandIterationVariable variable, CommonTree tree);
        void Visit(DownloadPageExpression expression, CommonTree tree);
        void Visit(DownloadImageExpression expression, CommonTree tree);

        void Visit(EachStatement eachStatement, CommonTree tree);
        void Visit(Block block, CommonTree tree);

        void Visit(TableMemberReference variable, CommonTree tree);
        void Visit(VariableReferance variable, CommonTree tree);
        void Visit(TableVariableReference variable, CommonTree tree);
        void Visit(StringLiteral literal, CommonTree tree);
        void Visit(IntegerLiteral literal, CommonTree tree);
        void Visit(NullLiteral literal, CommonTree tree);

        void Visit(TakeTextStatement statement, CommonTree tree);
        void Visit(TakeHtmlStatement statement, CommonTree tree);
        void Visit(TakeAttributeStatement statement, CommonTree tree);

        void Visit(VariableDeclarationStatement statement, CommonTree tree);

        void Visit(AdditionOperator op, CommonTree tree);
        void Visit(SubtrationOperator op, CommonTree tree);
        void Visit(MultiplicatonOperator op, CommonTree tree);
        void Visit(DivisionOperator op, CommonTree tree);

        void Visit(CaseVariableStatement statement, CommonTree tree);
        void Visit(WhenLiteralStatement statement, CommonTree tree);
        void Visit(CaseExpression expression, CommonTree tree);

        void Visit(CaseBooleanStatement statement, CommonTree tree);
        void Visit(WhenBooleanStatement statement, CommonTree tree);

        void Visit(AndExpression expression, CommonTree tree);
        void Visit(OrExpression expression, CommonTree tree);
        void Visit(LessThanExpression expression, CommonTree tree);
        void Visit(LessThanEqualExpression expression, CommonTree tree);
        void Visit(GreaterThanExpression expression, CommonTree tree);
        void Visit(GreaterThanEqualExpression expression, CommonTree tree);
        void Visit(NotEqualExpression expression, CommonTree tree);
        void Visit(EqualsExpression expression, CommonTree tree);

        void Visit(ProcedureDefinition definition, CommonTree tree);
        void Visit(ProcedureCall call, CommonTree tree);
    }
}
