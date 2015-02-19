using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace LeagueSharp.Loader.Core.Compiler
{
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

        public static void Compile(string csproj)
        {
            try
            {
                if (!File.Exists(csproj))
                {
                    return;
                }

                using (var workspace = MSBuildWorkspace.Create())
                {
                    var project = workspace.OpenProjectAsync(csproj).Result;
                }


                //var options = project.CompilationOptions
                //    .WithPlatform(Platform.X86)
                //    .WithOptimizationLevel(OptimizationLevel.Release);

                //var compilation = project
                //    .WithCompilationOptions(options)
                //    .WithMetadataReferences(LeagueSharpSystemReferences)
                //    .GetCompilationAsync().Result;

                //var diag = compilation.GetDiagnostics();

                //Console.WriteLine("References:");
                //foreach (var reference in project.MetadataReferences)
                //{
                //    Console.WriteLine(reference.Display);
                //}

                //Console.WriteLine("Diagnostics:");
                //if (diag.Any(d => d.Severity == DiagnosticSeverity.Error))
                //{
                //    Console.WriteLine("Analyzed: {0} Error Count: {1}", Path.GetFileName(csproj),
                //        diag.Count(d => d.Severity == DiagnosticSeverity.Error));
                //}

                //foreach (var diagnostic in diag.Where(d => d.Severity == DiagnosticSeverity.Error))
                //{
                //    Console.WriteLine(diagnostic);
                //}

                //Console.WriteLine("Emit:");
                //var result = compilation.Emit(Path.Combine(@"C:\Users\h3h3\AppData\Roaming\LeagueSharp\Assemblies", project.AssemblyName + ".dll"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}