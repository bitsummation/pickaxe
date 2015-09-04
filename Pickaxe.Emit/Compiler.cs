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

using Pickaxe.CodeDom.Visitor;
using Pickaxe.Parser;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pickaxe.Emit
{
    public class Compiler
    {
        private string[] _sources;

        public Compiler(string source) : this(new string[]{source})
        {
        }

        public Compiler(string[] sources)
        {
            _sources = sources;
            Errors = new List<Exception>();
        }

        public List<Exception> Errors { get; private set; }

        private CodeCompileUnit[] GetCompileUnits()
        {
            var compileUnits = new List<CodeCompileUnit>();
            foreach (var source in _sources)
            {
                var parser = new CodeParser(source);
                var ast = parser.Parse();
                if (parser.Errors.Any()) //antlr parse errors
                    Errors.AddRange(parser.Errors);

                if (!Errors.Any())
                {
                    var generator = new CodeDomGenerator(ast);
                    compileUnits.Add(generator.Generate());
                    if (generator.Errors.Any()) //Semantic erros
                        Errors.AddRange(generator.Errors);
                }
            }

            return compileUnits.ToArray();
        }

        public string[] ToCode() //generate source code.
        {
            var source = new List<string>();
            var compileUnits = new List<CodeCompileUnit>();
            foreach(var unit in compileUnits)
                source.Add(Persist.ToCSharpSource(unit));

            return source.ToArray();
        }

        public Assembly ToAssembly()
        {
            var compileUnits = GetCompileUnits();
            Assembly generatedAssembly = null;
            if (!Errors.Any())
            {
                var persist = new PersistAssembly(compileUnits);
                generatedAssembly = persist.ToAssembly();
                if (persist.Errors.Any()) //c# compile errors
                    Errors.AddRange(persist.Errors.Select(x => new Exception(x)));
            }

            return generatedAssembly;
        }
    }
}
