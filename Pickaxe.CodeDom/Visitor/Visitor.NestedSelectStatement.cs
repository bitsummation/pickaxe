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
        private BufferTable FetchBufferTable(NestedSelectStatement statement, IScopeData scope, out Boolean outerLoopNeeded)
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
            }

            return bufferTable;
        }

        public void Visit(NestedSelectStatement statement)
        {
            var fromDomArg = VisitChild(statement.From);

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Nested_Select" + fromDomArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            GenerateCallStatement(method.Statements, statement.Line.Line);

            _mainType.Type.Members.Add(method);
            var methodStatements = new CodeStatementCollection();

            //create type
            bool outerLoopNeeded;
            var bufferTable = FetchBufferTable(statement, fromDomArg.Scope, out outerLoopNeeded);
            var scope = CreateBufferTable(bufferTable);

            methodStatements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("CodeTable", new CodeTypeReference("b")),
                   "result",
                   new CodeObjectCreateExpression(new CodeTypeReference("BufferTable", new CodeTypeReference("b")))));


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
