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
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 *  private Table<ResultRow> Select_0ae5ca8202dd40f7851f8403512e901e()
    {
        Call(15);
        RuntimeTable<ResultRow> result = new RuntimeTable<ResultRow>();
        result.AddColumn("video");
        result.AddColumn("link");
        result.AddColumn("title");
        result.AddColumn("processed");
        CodeTable<anon_0d0c5f775d4f4103a5dbec37fd679db0> fromTable = From_6784c5a98b024a119a124c471a8b5ad1();
        fromTable = Where_f7d2dc7858b04dfcb427c5d85dc9c35a(fromTable);
        IEnumerator<anon_0d0c5f775d4f4103a5dbec37fd679db0> x = fromTable.GetEnumerator();
        for (
        ; x.MoveNext(); 
        )
        {
            anon_0d0c5f775d4f4103a5dbec37fd679db0 row = x.Current;
            ResultRow resultRow = new ResultRow(4);
            resultRow[0] = row.videos.video;
            resultRow[1] = row.videos.link;
            resultRow[2] = row.videos.title;
            resultRow[3] = row.videos.processed;
            if (((((resultRow[0] != null) 
                        && (resultRow[1] != null)) 
                        && (resultRow[2] != null)) 
                        && (resultRow[3] != null)))
            {
                result.Add(resultRow);
            }
        }
        OnSelect(result);
        return result;
    }
 * */

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(UpdateStatement statement)
        {
            using (Scope.PushSelect())
            {
                CodeMemberMethod method = new CodeMemberMethod();
                method.Name = "Update_" + Guid.NewGuid().ToString("N");
                method.Attributes = MemberAttributes.Private;
                GenerateCallStatement(method.Statements, statement.Line.Line);

                _mainType.Type.Members.Add(method);

                FromStatement fromStatement = null;
                if (statement.From == null) //alias must be a table ref
                {
                    var tableReference = new TableVariableReference() { Id = statement.Alias.Id, Line = statement.Alias.Line };
                    fromStatement = new FromStatement() { Line = tableReference.Line };
                    fromStatement.Children.Add(tableReference);

                }
                else
                    fromStatement = statement.From;


                var fromDomArg = VisitChild(fromStatement);
                var rowType = fromDomArg.Scope.CodeDomReference.TypeArguments[0];

                method.Statements.Add(new CodeVariableDeclarationStatement(fromDomArg.Scope.CodeDomReference,
                   "fromTable",
                   fromDomArg.CodeExpression));

                if (statement.Where != null)
                {
                    var domWhereArgs = VisitChild(statement.Where, new CodeDomArg() { Scope = fromDomArg.Scope });
                    method.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("fromTable"), domWhereArgs.CodeExpression));
                }

                //outside iterator
                method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerator", rowType),
                    "x",
                new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("fromTable"), "GetEnumerator",
                        null))));


                var loop = new CodeIterationStatement();
                loop.InitStatement = new CodeSnippetStatement();
                loop.IncrementStatement = new CodeSnippetStatement();
                loop.TestExpression = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("x"), "MoveNext",
                    null));

                loop.Statements.Add(new CodeVariableDeclarationStatement(rowType,
                    "row",
                    new CodePropertyReferenceExpression(new CodeTypeReferenceExpression("x"), "Current")));


                foreach (var a in statement.SetArgs.AssignStatements)
                {
                    var assignmentArg = VisitChild(a);
                    loop.Statements.AddRange(assignmentArg.ParentStatements);
                }

                method.Statements.Add(loop);

                var methodcall = new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(null, method.Name));
                _codeStack.Peek().CodeExpression = methodcall;
                _codeStack.Peek().ParentStatements.Add(methodcall);
            }
        }
    }
}
