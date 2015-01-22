using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace LeagueSharp.Loader.Core.Compiler
{
    public class RoslynCompiler
    {
        public RoslynCompiler()
        {
            Compile(@"C:\Git\LeagueSharp.Common\LeagueSharp.Common\LeagueSharp.Common\LeagueSharp.Common.csproj");
        }

        public static async void Compile(string csproj)
        {
            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(csproj).Result;

            var compilation = await project.GetCompilationAsync();
            var result = compilation.GetDiagnostics();

            Console.WriteLine("References:");
            foreach (var reference in project.MetadataReferences)
            {
                Console.WriteLine(reference.Display);
            }

            Console.WriteLine("Diagnostics:");
            foreach (var diagnostic in result.Where(d => d.Severity >= DiagnosticSeverity.Error))
            {
                Console.WriteLine(diagnostic);
            }

            Console.WriteLine("Emit:");
            if (result.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                Console.WriteLine("Skiped: " + Path.GetFileName(csproj));
            }
            else
            {
                compilation.Emit("LeagueSharp.Common.dll");
            }
        }
    }
}
