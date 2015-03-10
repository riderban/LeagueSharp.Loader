using System;
using System.IO;

namespace LeagueSharp.Loader.Model.Settings
{
    internal static class Directories
    {
        static Directories()
        {
            Directory.CreateDirectory(AppDataDirectory);
            Directory.CreateDirectory(RepositoryDirectory);
            Directory.CreateDirectory(AssembliesDirectory);
            Directory.CreateDirectory(LibrariesDirectory);
            Directory.CreateDirectory(CoreDirectory);
            Directory.CreateDirectory(NuGetDirectory);
            Directory.CreateDirectory(LogsDirectory);
            Directory.CreateDirectory(LocalRepositoryDirectory);
        }

        public static readonly string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string AppDataDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LeagueSharp");

        public static readonly string RepositoryDirectory = Path.Combine(AppDataDirectory, "Repositories");
        public static readonly string AssembliesDirectory = Path.Combine(AppDataDirectory, "Assemblies");
        public static readonly string LibrariesDirectory = Path.Combine(AppDataDirectory, "Libraries");
        public static readonly string CoreDirectory = Path.Combine(AppDataDirectory, "Core");
        public static readonly string NuGetDirectory = Path.Combine(AppDataDirectory, "NuGet");
        public static readonly string LogsDirectory = Path.Combine(AppDataDirectory, "Logs");
        public static readonly string LocalRepositoryDirectory = Path.Combine(AppDataDirectory, "LocalAssemblies");
        public static readonly string LoaderFilePath = Path.Combine(CurrentDirectory, "Leaguesharp.Loader.exe");
        public static readonly string ConfigFilePath = Path.Combine(AppDataDirectory, "config.xml");
        public static readonly string CoreFilePath = Path.Combine(CoreDirectory, "Leaguesharp.Core.dll");
        public static readonly string BootstrapFilePath = Path.Combine(CoreDirectory, "Leaguesharp.Bootstrap.dll");
    }
}