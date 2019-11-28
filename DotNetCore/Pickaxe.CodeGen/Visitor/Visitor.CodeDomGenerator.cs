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
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using Pickaxe.Sdk;
using Pickaxe.Runtime;
using Pickaxe.CodeDom.Semantic;
using System.Linq;

namespace Pickaxe.CodeDom.Visitor
{
    public partial class CodeDomGenerator : IAstVisitor
    {
        private AstNode _program;
        private CodeCompileUnit _unit;
        private CodeDomTypeDefinition _mainType;

        private int _totalOperations;
        private Stack<CodeDomArg> _codeStack;

        public CodeDomGenerator(AstNode program)
        {
            _totalOperations = 0;

            Errors = new List<SemanticException>();
            _program = program;
            _codeStack = new Stack<CodeDomArg>();

            _unit = new CodeCompileUnit();
            _mainType = new CodeDomTypeDefinition("Code");
            _mainType.Type.BaseTypes.Add("RuntimeBase");

            Scope.Reset();
            Scope.Push(_mainType);
            InitScope();
        }

        private void InitScope() //add runtime types
        {
            //DownloadPage
            Scope.Current.RegisterTable("DownloadPage", DownloadPage.Columns, new CodeTypeReference("Table", new CodeTypeReference("DownloadPage")));
            Scope.Current.RegisterTable("DownloadImage", DownloadImage.Columns, new CodeTypeReference("Table", new CodeTypeReference("DownloadImage")));
            Scope.Current.RegisterTable("Expand", Expand.Columns, new CodeTypeReference("Table", new CodeTypeReference("Expand")));
            Scope.Current.RegisterTable("DynamicObject", DynamicObject.Columns, new CodeTypeReference("Table", new CodeTypeReference("DynamicObject")));

            //Register @@identity
            Scope.Current.RegisterPrimitive("@@identity", typeof(int), new CodeTypeReference(typeof(int)));
            Scope.Current.Type.Type.Members.Add(
               new CodeMemberField() { Name = "g_identity", Type =  new CodeTypeReference(typeof(int)), Attributes = MemberAttributes.Public | MemberAttributes.Final });
        }

        public IList<SemanticException> Errors { get; private set; }

        public CodeCompileUnit Generate()
        { 
            _program.Accept(this);
            return _unit;
        }

        private void CallOnProgressComplete(CodeStatementCollection statements)
        {
            var progressArgs = new CodeObjectCreateExpression(new CodeTypeReference("ProgressArgs"),
                new CodePropertyReferenceExpression(null, "TotalOperations"),
                new CodePropertyReferenceExpression(null, "TotalOperations"));

            statements.Add(new CodeMethodInvokeExpression(null, "OnProgress", progressArgs));
        }

        private void CallOnProgress(CodeStatementCollection statements, bool increaseTotal = true)
        {
            if(increaseTotal)
                _totalOperations++;

            var methodcall = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(null, "OnProgress"));

            statements.Add(methodcall);
        }

        private void GenerateCallStatement(CodeStatementCollection statements, int line)
        {
            statements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, "Call"), new CodePrimitiveExpression(line)));
        }

        private CodeDomArg VisitChild(AstNode node, CodeDomArg arg)
        {
            _codeStack.Push(arg);
            node.Accept(this);
            return _codeStack.Pop();
        }

        private CodeDomArg VisitChild(AstNode node)
        {
            return VisitChild(node, new CodeDomArg());
        }


        private CodeMemberMethod CreateStepMethod()
        {
            var method = new CodeMemberMethod();

            method.Name = "Step_" + Guid.NewGuid().ToString("N");
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            _mainType.Type.Members.Add(method);
            return method;
        }

        private CodeMemberMethod CreateBlockMethod()
        {
            var method = new CodeMemberMethod();

            method.Name = "Block_" + Guid.NewGuid().ToString("N");
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            _mainType.Type.Members.Add(method);
            return method;
        }
    }
}
