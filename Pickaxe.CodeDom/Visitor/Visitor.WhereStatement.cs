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

using Microsoft.CSharp;
using Pickaxe.Sdk;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Visitor
{
    /*
        * 
        *   private CodeTable<temp> Where_37a99efb340440479119342f9a9481b2(CodeTable<temp> table)
   {
       var rows = table.Where(row => row.id == 2).ToArray();
       return fromTable.Where(""); //nodes
   }
        */ 

    public partial class CodeDomGenerator : IAstVisitor
    {
        public void Visit(WhereStatement statement)
        {
            var statementDomArg = VisitChild(statement.BooleanExpression, new CodeDomArg() { Scope = _codeStack.Peek().Scope });

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "Where_" + statementDomArg.MethodIdentifier;
            method.Attributes = MemberAttributes.Private;
            method.Parameters.Add(new CodeParameterDeclarationExpression(_codeStack.Peek().Scope.CodeDomReference, "table"));
            GenerateCallStatement(method.Statements, statement.Line.Line);

            _mainType.Type.Members.Add(method);


            //hack to get anonymous delegate to work. CodeDom doesn't not directly support this
            CSharpCodeProvider csc = new CSharpCodeProvider();
            StringWriter sw = new StringWriter();
            csc.GenerateCodeFromStatement(new CodeMethodReturnStatement(statementDomArg.CodeExpression), sw, new CodeGeneratorOptions());

            var boolean = new CodeSnippetExpression("row => {" + sw + "}");

            var rowType = new CodeTypeReference("IList", new CodeTypeReference(_codeStack.Peek().Scope.CodeDomReference.TypeArguments[0].BaseType));

            method.Statements.Add(new CodeVariableDeclarationStatement(rowType, "rows",
                new CodeMethodInvokeExpression(
                    new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression("table"), "Where"), boolean), "ToList"))
                );

            method.Statements.Add(new CodeMethodInvokeExpression(
                new CodeVariableReferenceExpression("table"), "SetRows", new CodeVariableReferenceExpression("rows")));

            var methodcall = new CodeMethodInvokeExpression(
             new CodeMethodReferenceExpression(null, method.Name), new CodeArgumentReferenceExpression("fromTable"));

            _codeStack.Peek().CodeExpression = methodcall;




            /*VerifyCssSelector(statement.Selector, new Semantic.LineInfo(statement.Line.Line, statement.Line.CharacterPosition));

            var state = new CodeAssignStatement(
                    new CodeSnippetExpression("fromTable"),
                     new CodeMethodInvokeExpression(
                         new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("fromTable"), "Where"),
                         new CodePrimitiveExpression(statement.Selector)));

            _codeStack.Peek().ParentStatements.Add(state);*/
        }
    }
}
