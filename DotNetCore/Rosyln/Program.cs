using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Pickaxe.Emit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Rosyln
{
    class Program
    {

        private static SyntaxTree BuildTree()
        {
            var unit = SyntaxFactory.CompilationUnit();

            unit = unit.WithUsings(SyntaxFactory.SingletonList(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"))));

            var mainNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("RoslynCompileSample"));

            var mainType = SyntaxFactory.ClassDeclaration("Writer").WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                    )
                );

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Write")
                .WithModifiers(
                 SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                    )
                );


            var invodeExpression = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("Console"), SyntaxFactory.IdentifierName("WriteLine"))
                ).WithArgumentList(
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(
                            SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("what")
                            )))));


            method = method.WithBody(SyntaxFactory.Block(
                SyntaxFactory.SingletonList(
                    SyntaxFactory.ExpressionStatement(invodeExpression))
                ));

            mainType = mainType.AddMembers(method);
            mainNamespace = mainNamespace.AddMembers(mainType);
            unit = unit.AddMembers(mainNamespace);

            return unit.SyntaxTree;
        }
        private static void GenTest()
        {
            SyntaxTree syntaxTree = BuildTree();
            var code = syntaxTree.GetRoot().NormalizeWhitespace().ToFullString();

            //SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);

            string assemblyName = Path.GetRandomFileName();
            var refPaths = new[] {
                typeof(System.Object).GetTypeInfo().Assembly.Location,
                typeof(Console).GetTypeInfo().Assembly.Location,
                Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")
            };

            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(OptimizationLevel.Release));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return;
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                var type = assembly.GetType("RoslynCompileSample.Writer");
                var instance = assembly.CreateInstance("RoslynCompileSample.Writer");
                var meth = type.GetMember("Write").First() as MethodInfo;
                meth.Invoke(instance, new[] { "joel" });
            }
        }

        static void Main(string[] args)
        {
            var input = @"

 select *
from download page 'https://www.faa.gov/air_traffic/weather/asos/?state=TX'
where nodes = 'table.asos tbody tr'

";
            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
        }
    }
}
