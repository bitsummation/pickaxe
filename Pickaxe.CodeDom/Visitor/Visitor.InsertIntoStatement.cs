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
using Pickaxe.CodeDom.Semantic;
using Pickaxe.Sdk;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(InsertIntoStatement statement)
        {
            DoInsert(statement);
        }

        private void DoInsert(InsertIntoStatement statement)
        {
            var variableArgs = VisitChild(statement.Variable);
            var domArg = VisitChild(statement.Select);

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Insert_" + domArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            GenerateCallStatement(method.Statements, statement.Line.Line);

            ((Action)domArg.Tag)(); //remove call to OnSelect

            method.Statements.Add(new CodeVariableDeclarationStatement(domArg.Scope.CodeDomReference,
                "resultRows",
                domArg.CodeExpression));

            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(variableArgs.CodeExpression, "BeforeInsert"), new CodePrimitiveExpression(statement.Overwrite)));

            var identityArgs = VisitChild(new VariableReferance() { Id = "@@identity" });

            method.Statements.Add(new CodeAssignStatement(identityArgs.CodeExpression,
                    new CodePropertyReferenceExpression(variableArgs.CodeExpression, "RowCount")
                   ));

            method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("IEnumerator", new CodeTypeReference("ResultRow")),
            "x",
            new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("resultRows"), "GetEnumerator",
                    null))));


            var loop = new CodeIterationStatement();
            loop.InitStatement = new CodeSnippetStatement();
            loop.IncrementStatement = new CodeSnippetStatement();
            loop.TestExpression = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("x"), "MoveNext",
                null));


            loop.Statements.Add(new CodeAssignStatement(identityArgs.CodeExpression,
                new CodeBinaryOperatorExpression(identityArgs.CodeExpression,
                    CodeBinaryOperatorType.Add,
                    new CodePrimitiveExpression(1))
                    ));


            loop.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("ResultRow"),
                "row",
                new CodePropertyReferenceExpression(new CodeTypeReferenceExpression("x"), "Current")));

            loop.Statements.Add(new CodeVariableDeclarationStatement(
                new CodeTypeReference(statement.Variable.Id),
                "tableRow",
                new CodeObjectCreateExpression(new CodeTypeReference(statement.Variable.Id))));

            ScopeData<TableDescriptor> descriptor = Scope.EmptyTableDescriptor;
            if (Scope.Current.IsTableRegistered(statement.Variable.Id))
                descriptor = Scope.Current.GetTableDescriptor(statement.Variable.Id);

            int insertCount = descriptor.Type.Variables.Where(x => !x.Primitive.IsIdentity).Count();
            if (insertCount != statement.Select.Args.Length) //lengths don't match for insert. Need to remove identities
                Errors.Add(new InsertSelectArgsNotEqual(new Semantic.LineInfo(statement.Line.Line, statement.Line.CharacterPosition)));

            int indexer = 0;
            for (int x = 0; x < descriptor.Type.Variables.Count; x++)
            {
                var left = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("tableRow"), descriptor.Type.Variables[x].Variable);
                CodeExpression right = null;
                if(descriptor.Type.Variables[x].Primitive.IsIdentity)
                {
                    right = identityArgs.CodeExpression;
                }
                else
                {
                    right = new CodeIndexerExpression(new CodeTypeReferenceExpression("row"), new CodeSnippetExpression(indexer.ToString()));
                    right = descriptor.Type.Variables[x].Primitive.ToNative(right);
                    indexer++;
                }

                loop.Statements.Add(new CodeAssignStatement(left, right));
            }
         

            loop.Statements.Add(new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(variableArgs.CodeExpression, "Add"),
                new CodeVariableReferenceExpression("tableRow")));



            method.Statements.Add(loop);
            _mainType.Type.Members.Add(method);

            var methodcall = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(null, method.Name));

            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(variableArgs.CodeExpression, "AfterInsert")));

            _codeStack.Peek().ParentStatements.Add(methodcall);
            _codeStack.Peek().CodeExpression = methodcall;
        }
    }
}
