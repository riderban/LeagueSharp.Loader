#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

#endregion

namespace LeagueSharp.Loader.Core.Compiler
{

    #region

    #endregion

    public class RoslynCompiler
    {
        private static IEnumerable<MetadataReference> LeagueSharpSystemReferences
        {
            get
            {
                return
                    Directory.EnumerateFiles(@"C:\Program Files (x86)\LeagueSharp\System", ".dll")
                        .Select(f => MetadataReference.CreateFromFile(f));
            }
        }

        public static async void Compile(string csproj)
        {
            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(csproj).Result.AddMetadataReferences(LeagueSharpSystemReferences);
            var compilation = await project.GetCompilationAsync();
            var diag = compilation.GetDiagnostics();

            Console.WriteLine("References:");
            foreach (var reference in project.MetadataReferences)
            {
                Console.WriteLine(reference.Display);
            }

            //Console.WriteLine("Diagnostics:");
            //foreach (var diagnostic in diag.Where(d => d.Severity >= DiagnosticSeverity.Error))
            //{
            //    Console.WriteLine(diagnostic);
            //}

            Console.WriteLine("Emit:");
            if (diag.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                Console.WriteLine(
                    "Skiped: " + Path.GetFileName(csproj) + " Error Count: " +
                    diag.Count(d => d.Severity >= DiagnosticSeverity.Error));
            }
            else
            {
                compilation.Emit("LeagueSharp.Common.dll");
            }
        }
    }
}