using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Pickaxe.CodeDom.Visitor;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
                var parser = new Parser.CodeParser(source);
                var ast = parser.Parse();
                if (parser.Errors.Any()) //antlr parse errors
                    Errors.AddRange(parser.Errors);

                if (!Errors.Any())
                {
                    var generator = new CodeDomGenerator(ast);
                    var unit = generator.Generate();

                    if (generator.Errors.Any()) //Semantic erros
                        Errors.AddRange(generator.Errors);
                    else
                    {
                        SyntaxTree tree = CSharpSyntaxTree.ParseText(ToCSharpSource(unit));
                        treeList.Add(tree);
                    }
                }
            }

            return treeList.ToArray();
        }

        public static string ToCSharpSource(CodeCompileUnit unit)
        {
            string code = string.Empty;
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StringWriter writer = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(
                  unit, writer, options);
                code = writer.ToString();
            }

            return code;
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
