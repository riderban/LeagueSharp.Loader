#region LICENSE

// Copyright 2015-2015 LeagueSharp.Loader
// RoslynCompiler.cs is part of LeagueSharp.Loader.
// 
// LeagueSharp.Loader is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Loader is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Loader. If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace LeagueSharp.Loader.Core.Compiler
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.MSBuild;

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