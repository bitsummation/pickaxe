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

using Pickaxe.Runtime;
using Pickaxe.Sdk;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        private BufferTable FetchBufferTable(NestedSelectStatement statement, IScopeData scope, IList<CodeAssignStatement> codeAssignments, out Boolean outerLoopNeeded)
        {
            outerLoopNeeded = false;
            var bufferTable = new BufferTable(){Variable = "b"};

            for (int x = 0; x < statement.Args.Length; x++) //select args
            {
                var domSelectArg = VisitChild(statement.Args[x], new CodeDomArg() { Scope = scope });
                if (domSelectArg.Tag != null)
                    outerLoopNeeded = true;

                var primitive = TablePrimitive.FromType(Type.GetType(domSelectArg.Scope.CodeDomReference.BaseType));
                bufferTable.Children.Add(new TableColumnArg() {Variable = "a" + x, Type = primitive.TypeString });

                var assignment = new CodeAssignStatement();
                assignment.Left = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression("resultRow"), "a" + x);
                assignment.Right = domSelectArg.CodeExpression;
                codeAssignments.Add(assignment);
            }

            return bufferTable;
        }

        public void Visit(NestedSelectStatement statement)
        {
            BufferTable bufferTable;
            CodeMemberMethod method = new CodeMemberMethod();
            var methodStatements = new CodeStatementCollection();
            var selectArgAssignments = new List<CodeAssignStatement>();

            using (Scope.PushSelect())
            {
                if (statement.From == null)
                {
                    GenerateSelectOnly(statement);
                    return;
                }

                var fromDomArg = VisitChild(statement.From);
                var rowType = fromDomArg.Scope.CodeDomReference.TypeArguments[0];

                method.Name = "Nested_Select_" + fromDomArg.MethodIdentifier;
                method.Attributes = MemberAttributes.Private;
                GenerateCallStatement(method.Statements, statement.Line.Line);

                _mainType.Type.Members.Add(method);

                //create type
                bool outerLoopNeeded;
                bufferTable = FetchBufferTable(statement, fromDomArg.Scope, selectArgAssignments, out outerLoopNeeded);

                methodStatements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("CodeTable", new CodeTypeReference("b")),
                       "result",
                       new CodeObjectCreateExpression(new CodeTypeReference("BufferTable", new CodeTypeReference("b")))));

                methodStatements.Add(new CodeVariableDeclarationStatement(fromDomArg.Scope.CodeDomReference,
                    "fromTable",
                    fromDomArg.CodeExpression));

                if (statement.Where != null)
                {
                    var domWhereArgs = VisitChild(statement.Where, new CodeDomArg() { Scope = fromDomArg.Scope });
                    methodStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("fromTable"), domWhereArgs.CodeExpression));
                }

                //outside iterator
                methodStatements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerator", rowType),
                    "x",
                new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("fromTable"), "GetEnumerator",
                        null))));


                //foreach loops

                var outerLoop = new CodeIterationStatement();
                outerLoop.InitStatement = new CodeSnippetStatement();
                outerLoop.IncrementStatement = new CodeSnippetStatement();
                outerLoop.TestExpression = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("x"), "MoveNext",
                    null));

                outerLoop.Statements.Add(new CodeVariableDeclarationStatement(rowType,
                "row",
                new CodePropertyReferenceExpression(new CodeTypeReferenceExpression("x"), "Current")));

                var codeLoop = outerLoop;
                if (outerLoopNeeded)
                {
                    var aliases = Scope.Current.AliasType<DownloadPage>();
                    CodeDomArg args = null;
                    if (aliases.Length == 0)
                    {
                        //register error we could't find a download table and we are trying to access pick statements.
                        //create fake codedomargs
                        args = new CodeDomArg() { CodeExpression = new CodePrimitiveExpression("DownloadTable") };
                    }
                    else
                    {
                        var reference = new TableMemberReference() { Member = "nodes", RowReference = new TableVariableRowReference() { Id = aliases[0] } };
                        args = VisitChild(reference);
                    }

                    //Needed only for DownloadRow
                    outerLoop.Statements.Add(
                        new CodeVariableDeclarationStatement(
                            new CodeTypeReference("IEnumerator", new CodeTypeReference("HtmlElement")),
                            "y",
                            new CodeMethodInvokeExpression(
                                new CodeMethodReferenceExpression(
                                        args.CodeExpression, "GetEnumerator", null))));


                    //Needed only for DownloadRow
                    codeLoop = new CodeIterationStatement();
                    codeLoop.InitStatement = new CodeSnippetStatement();
                    codeLoop.IncrementStatement = new CodeSnippetStatement();
                    codeLoop.TestExpression = new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("y"), "MoveNext",
                        null));

                    //Needed only for DownloadRow
                    codeLoop.Statements.Add(new CodeVariableDeclarationStatement(
                        new CodeTypeReference("HtmlElement"),
                        "node",
                        new CodePropertyReferenceExpression(new CodeTypeReferenceExpression("y"), "Current")));

                    outerLoop.Statements.Add(codeLoop);
                }

                //Needed for both.
                codeLoop.Statements.Add(new CodeVariableDeclarationStatement(
                    new CodeTypeReference("b"),
                    "resultRow",
                    new CodeObjectCreateExpression(new CodeTypeReference("b"))));

                codeLoop.Statements.AddRange(selectArgAssignments.ToArray());

                var addResults = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("result"), "Add"),
                    new CodeArgumentReferenceExpression("resultRow"));
                codeLoop.Statements.Add(addResults);


                methodStatements.Add(outerLoop);

                if (outerLoopNeeded)
                {
                    var aliases = Scope.Current.AliasType<DownloadPage>();
                    outerLoop.Statements.Add(new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("row"), aliases[0]), "Clear"));
                }

            }

            var scope = CreateBufferTable(bufferTable);
            method.ReturnType = scope.CodeDomReference;
            var methodcall = new CodeMethodInvokeExpression(
                   new CodeMethodReferenceExpression(null, method.Name));

            methodStatements.Add(new CodeMethodReturnStatement(new CodeTypeReferenceExpression("result")));
            method.Statements.AddRange(methodStatements);

            _codeStack.Peek().CodeExpression = methodcall;
            _codeStack.Peek().Scope = new ScopeData<TableDescriptor>() { CodeDomReference = method.ReturnType };

            //descriptor = Scope.Current.GetTableDescriptor(statement.Variable.Id);


                /*
            var methodStatements = new CodeStatementCollection();
            var selectArgAssignments = new List<CodeAssignStatement>();
             = false;

            var typePair = new List<VariableTypePair>();

            for (int x = 0; x < statement.Args.Length; x++) //select args
            {
                var domSelectArg = VisitChild(statement.Args[x], new CodeDomArg() { Scope = fromDomArg.Scope });
                if (domSelectArg.Tag != null)
                    outerLoopNeeded = true;

                var primitive = TablePrimitive.FromType(Type.GetType(domSelectArg.Scope.CodeDomReference.BaseType));
                typePair.Add(new VariableTypePair() { Variable = "a", Primitive = primitive });

                //var assignment = new CodeAssignStatement();
                //resultRow.a
                //resultRow.b
                //assignment.Left = new CodeIndexerExpression(new CodeTypeReferenceExpression("resultRow"), new CodeSnippetExpression(x.ToString()));
                //assignment.Right = domSelectArg.CodeExpression;

                //methodStatements.AddRange(domSelectArg.ParentStatements);

                //selectArgAssignments.Add(assignment);
            }

            //CreateBufferTable(new BufferTable(){Variable = "b", Args = })*/

        }

    }
}
