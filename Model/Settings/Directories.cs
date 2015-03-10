using System;
using System.IO;

namespace LeagueSharp.Loader.Model.Settings
{
    internal static class Directories
    {
        public static readonly string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string AppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LeagueSharp");
        public static readonly string RepositoryDir = Path.Combine(AppDataDirectory, "Repositories");
        public static readonly string AssembliesDir = Path.Combine(AppDataDirectory, "Assemblies");
        public static readonly string CoreDirectory = Path.Combine(CurrentDirectory, "System");
        public static readonly string NuGetDirectory = Path.Combine(CurrentDirectory, "NuGet");
        public static readonly string LogsDir = Path.Combine(CurrentDirectory, "Logs");
        public static readonly string LocalRepoDir = Path.Combine(CurrentDirectory, "LocalAssemblies");
        public static readonly string LoaderFilePath = Path.Combine(CurrentDirectory, "Leaguesharp.Loader.exe");
        public static readonly string ConfigFilePath = Path.Combine(CurrentDirectory, "config.xml");
        public static readonly string CoreFilePath = Path.Combine(CoreDirectory, "Leaguesharp.Core.dll");
        public static readonly string BootstrapFilePath = Path.Combine(CoreDirectory, "Leaguesharp.Bootstrap.dll");
    }
}