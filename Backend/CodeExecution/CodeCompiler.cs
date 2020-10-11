using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backend.CodeExecution
{
    internal static class CodeCompiler
    {
        private static readonly CSharpParseOptions parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8);

        private static readonly CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            optimizationLevel: OptimizationLevel.Release,
            assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default,
            checkOverflow: true
        );

        private static readonly AssemblyMetadata ownAssembly = AssemblyMetadata.CreateFromFile(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SystemDll.dll"));

        private static readonly MetadataReference[] references = new MetadataReference[]
        {
            AssemblyMetadata.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location).GetReference(),
            ownAssembly.GetReference()
        };

        static CodeCompiler()
        {
            Directory.CreateDirectory("./PlayerDLLs");
        }

        public static bool Compile(CSharpCompilation compilation)
        {
            var result = compilation.Emit(Path.Combine("./PlayerDLLs", compilation.AssemblyName + ".dll"));

            if (!result.Success)
            {
                Console.WriteLine("Compilation done with errors:");

                var failures = result.Diagnostics
                    .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
                {
                    Console.WriteLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                }

                return false;
            }

            Console.WriteLine("Compilation done without any errors.");

            return true;
        }

        public static CSharpCompilation Parse(Guid playerId, IEnumerable<string> sourceCode)
        {
            var parses = sourceCode
                .Select(code => SourceText.From(code))
                .Select(source => SyntaxFactory.ParseSyntaxTree(source, parseOptions));

            return CSharpCompilation.Create($"Player{playerId}Assembly",
                parses,
                references: references,
                options: compilationOptions);
        }
    }
}
