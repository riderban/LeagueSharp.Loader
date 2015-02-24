using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace LeagueSharp.Loader.Core.Compiler
{
    internal class RoslynCompiler
    {
        private static IEnumerable<MetadataReference> LeagueSharpSystemReferences
        {
            get
            {
                return
                    Directory.EnumerateFiles(Directories.CoreDirectory, ".dll")
                        .Select(f => MetadataReference.CreateFromFile(f));
            }
        }

        public static void Compile(LeagueSharpAssembly assembly)
        {
            try
            {
                if (!File.Exists(assembly.Project))
                {
                    return;
                }

                using (var workspace = MSBuildWorkspace.Create())
                {
                    var project = workspace.OpenProjectAsync(assembly.Project).Result;

                    var options = project.CompilationOptions
                        .WithPlatform(Platform.X86)
                        .WithOutputKind(assembly.OutputKind)
                        .WithOptimizationLevel(assembly.Optimization);

                    var compilation = project
                        .WithCompilationOptions(options)
                        .WithMetadataReferences(LeagueSharpSystemReferences)
                        .GetCompilationAsync().Result;

                    var diag = compilation.GetDiagnostics();

                    Console.WriteLine("References:");
                    foreach (var reference in project.MetadataReferences)
                    {
                        Console.WriteLine(reference.Display);
                    }

                    Console.WriteLine("Diagnostics:");
                    if (diag.Any(d => d.Severity == DiagnosticSeverity.Error))
                    {
                        Console.WriteLine("Analyzed: {0} Error Count: {1}", Path.GetFileName(assembly.Project),
                            diag.Count(d => d.Severity == DiagnosticSeverity.Error));
                    }

                    foreach (var diagnostic in diag.Where(d => d.Severity == DiagnosticSeverity.Error))
                    {
                        Console.WriteLine(diagnostic);
                    }

                    Console.WriteLine("Emit:");
                    var result = compilation.Emit(Path.Combine(@"C:\Users\h3h3\AppData\Roaming\LeagueSharp\Assemblies", project.AssemblyName + ".dll"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}