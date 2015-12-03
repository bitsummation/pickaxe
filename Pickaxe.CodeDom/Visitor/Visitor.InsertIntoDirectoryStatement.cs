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
        public void Visit(InsertIntoDirectoryStatement statement)
        {
            if (statement.Select.Args.Length != 2) //can only select one thing into a directory
            {
                Errors.Add(new OnlyTwoSelectParamForDirectory(new Semantic.LineInfo(statement.Line.Line, statement.Line.CharacterPosition)));
                return;
            }

            var domArg = VisitChild(statement.Select);

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Insert_" + domArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;

            ((Action)domArg.Tag)();

            //needs to foreach around the returned table from select like the other inserts

            method.Statements.Add(new CodeVariableDeclarationStatement(domArg.Scope.CodeDomReference,
                "resultRows",
                domArg.CodeExpression));

            var cast = new CodeCastExpression(typeof(string),
                new CodeIndexerExpression(new CodeIndexerExpression(new CodeVariableReferenceExpression("resultRows"),
                    new CodePrimitiveExpression(0)), new CodePrimitiveExpression(0)));

            method.Statements.Add(new CodeVariableDeclarationStatement(typeof(string), "filename", cast));

            cast = new CodeCastExpression(typeof(byte[]),
                new CodeIndexerExpression(new CodeIndexerExpression(new CodeVariableReferenceExpression("resultRows"),
                    new CodePrimitiveExpression(0)), new CodePrimitiveExpression(1))); 

            method.Statements.Add(new CodeVariableDeclarationStatement(typeof(byte[]), "bytes", cast));

            var directoryArgs = VisitChild(statement.Directory);

            method.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bytes"), "WriteFile",
                directoryArgs.CodeExpression, new CodeVariableReferenceExpression("filename")));

            _mainType.Type.Members.Add(method);

            var methodcall = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(null, method.Name));

            _codeStack.Peek().ParentStatements.Add(methodcall);
            _codeStack.Peek().CodeExpression = methodcall;
        }
    }
}
