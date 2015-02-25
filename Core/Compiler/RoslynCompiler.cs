using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

namespace LeagueSharp.Loader.Core.Compiler
{
    internal class RoslynCompiler
    {
        private static IEnumerable<MetadataReference> LeagueSharpSystemReferences
        {
            get
            {
                return Directory.EnumerateFiles(Directories.CoreDirectory, "*.dll").Select(f => MetadataReference.CreateFromFile(f));
            }
        }

        public static bool Compile(LeagueSharpAssembly assembly)
        {
            try
            {
                if (!File.Exists(assembly.Project))
                {
                    return false;
                }

                //using (var workspace = MSBuildWorkspace.Create())
                //{
                //    var project = workspace.OpenProjectAsync(assembly.Project).Result;

                //    var options = project.CompilationOptions
                //        .WithPlatform(Platform.X86)
                //        // TODO: .WithOutputKind(assembly.OutputKind) 
                //        .WithOptimizationLevel(assembly.Optimization);

                //    Console.WriteLine("Project References:");
                //    foreach (var reference in project.MetadataReferences)
                //    {
                //        Console.WriteLine(reference.Display);
                //    }

                //    var compilation = project
                //        .WithAssemblyName(assembly.Name)
                //        .WithCompilationOptions(options)
                //        //.WithMetadataReferences(LeagueSharpSystemReferences)
                //        .GetCompilationAsync().Result;

                //    var diag = compilation.GetDiagnostics();

                //    Console.WriteLine("References:");
                //    foreach (var reference in compilation.References)
                //    {
                //        Console.WriteLine(reference.Display);
                //    }

                //    Console.WriteLine("Diagnostics:");
                //    Console.WriteLine("Analyzed: {0} Error Count: {1}", Path.GetFileName(assembly.Project),
                //        diag.Count(d => d.Severity == DiagnosticSeverity.Error));

                //    var outputFile = "";
                //    switch (assembly.OutputKind)
                //    {
                //            case OutputKind.ConsoleApplication:
                //            outputFile = assembly.Name + ".exe";
                //            break;

                //            case OutputKind.DynamicallyLinkedLibrary:
                //            outputFile = assembly.Name + ".dll";
                //            break;

                //        default:
                //            throw new NotSupportedException(assembly.OutputKind.ToString());
                //    }

                //    compilation.Emit(Path.Combine(Directories.AssembliesDir, outputFile), Directories.AssembliesDir);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }
    }
}