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
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using System.Collections.Generic;
using HtmlAgilityPack;
using Pickaxe.Runtime;

namespace Pickaxe.Emit
{
    public class Persist
    {
        private CodeCompileUnit _unit;

        public Persist(CodeCompileUnit unit)
        {
            _unit = unit;
            Errors = new List<string>();
        }

        public IList<string> Errors { get; private set; }

        public void ToFile(string path)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(path))
            {
                provider.GenerateCodeFromCompileUnit(
                    _unit, sourceWriter, options);
            }
        }

        public string ToCSharpSource()
        {
            string code = string.Empty;
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StringWriter writer = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(
                  _unit, writer, options);
                code = writer.ToString();
            }

            return code;
        }

        public Assembly ToAssembly()
        {
            Assembly generatedAssembly = null;

            String[] assemblyNames = { 
                                         "System.dll",
                                         "System.Core.dll",
                                         "System.Xml.dll",
                                         typeof(FileTable<>).Assembly.Location,
                                         typeof(HtmlNode).Assembly.Location
                                     };

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters(assemblyNames);
           
            cp.GenerateExecutable = false;
            //cp.OutputAssembly = "test";
            cp.GenerateInMemory = true;

            CompilerResults cr = provider.CompileAssemblyFromDom(cp, _unit);
            foreach (CompilerError compilerError in cr.Errors)
                Errors.Add(compilerError +  Environment.NewLine);

            if(!Errors.Any())
                generatedAssembly = cr.CompiledAssembly;

            return generatedAssembly;
        }
    }
}
