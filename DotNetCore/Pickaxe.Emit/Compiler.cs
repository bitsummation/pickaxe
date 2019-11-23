using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pickaxe.CodeDom.Visitor;
using Pickaxe.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pickaxe.Emit
{
    public class Compiler
    {
        private string[] _sources;

        public Compiler(string source)
            : this(new string[] { source })
        {
        }

        public Compiler(string[] sources)
        {
            _sources = sources;
            Errors = new List<Exception>();
        }

        public List<Exception> Errors { get; private set; }

        private SyntaxTree[] CodeGenCore()
        {
            var treeList = new List<SyntaxTree>();
            foreach (var source in _sources)
            {
                var parser = new CodeParser(source);
                var ast = parser.Parse();
                if (parser.Errors.Any()) //antlr parse errors
                    Errors.AddRange(parser.Errors);

                if (!Errors.Any())
                {
                    var generator = new CodeDomGenerator(ast);
                    treeList.Add(generator.Generate());
                    if (generator.Errors.Any()) //Semantic erros
                        Errors.AddRange(generator.Errors);
                }
            }

            return treeList.ToArray();
        }

        public string[] ToCode() //generate source code.
        {
            var source = new List<string>();
            var compileUnits = CodeGenCore();
            if (!Errors.Any())
            {
                foreach (var unit in compileUnits)
                    source.Add(unit.GetRoot().NormalizeWhitespace().ToFullString());
            }

            return source.ToArray();
        }

        public Assembly ToAssembly()
        {
            Assembly generatedAssembly = null;
            var trees = CodeGenCore();
            if (!Errors.Any())
            {
                var persist = new AssemblyGenerator(trees);
                generatedAssembly = persist.ToAssembly();
                if (persist.Errors.Any()) //c# compile errors
                    Errors.AddRange(persist.Errors.Select(x => new Exception(x)));
            }

            return generatedAssembly;
        }
    }
}
