using System;
using System.IO;
using System.Windows.Forms;
using LeagueSharp.Loader.Core;

namespace LeagueSharp.Loader.Model.Settings
{
    internal static class Directories
    {
        public static string AssembliesDirectory
        {
            get { return Path.Combine(DataDirectory, "Assemblies"); }
        }

        public static string BootstrapFilePath
        {
            get { return Path.Combine(CoreDirectory, "Leaguesharp.Bootstrap.dll"); }
        }

        public static string ConfigFilePath
        {
            get { return Path.Combine(CurrentDirectory, "config.json"); }
        }

        public static string CoreDirectory
        {
            get { return Path.Combine(DataDirectory, "Core"); }
        }

        public static string CoreFilePath
        {
            get { return Path.Combine(CoreDirectory, "Leaguesharp.Core.dll"); }
        }

        public static string CurrentDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string DataDirectory
        {
            get { return Config.Instance.DataDirectory; }
        }

        public static string LibrariesDirectory
        {
            get { return Path.Combine(DataDirectory, "Libraries"); }
        }

        public static string LoaderFilePath
        {
            get { return Path.Combine(CurrentDirectory, "LeagueSharp.Loader.exe"); }
        }

        public static string LocalRepositoryDirectory
        {
            get { return Path.Combine(DataDirectory, "LocalAssemblies"); }
        }

        public static string LogsDirectory
        {
            get { return Path.Combine(DataDirectory, "Logs"); }
        }

        public static string NuGetDirectory
        {
            get { return Path.Combine(DataDirectory, "NuGet"); }
        }

        public static string RepositoryDirectory
        {
            get { return Path.Combine(DataDirectory, "Repositories"); }
        }

        public static void Initialize()
        {
            if (string.IsNullOrEmpty(Config.Instance.DataDirectory))
            {
                var dialog = new FolderBrowserDialog
                {
                    Description = Utility.GetMultiLanguageText("DataDirectoryDialog")
                };

                while (dialog.ShowDialog() != DialogResult.OK)
                {
                    var result = MessageBox.Show(Utility.GetMultiLanguageText("DataDirectoryDialogError"),
                        "Data Directory", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

                    if (result == DialogResult.Cancel)
                    {
                        Environment.Exit(1);
                    }
                }

                Config.Instance.DataDirectory = dialog.SelectedPath;
            }

            Directory.CreateDirectory(Config.Instance.DataDirectory);
            Directory.CreateDirectory(RepositoryDirectory);
            Directory.CreateDirectory(AssembliesDirectory);
            Directory.CreateDirectory(LibrariesDirectory);
            Directory.CreateDirectory(CoreDirectory);
            Directory.CreateDirectory(NuGetDirectory);
            Directory.CreateDirectory(LogsDirectory);
            Directory.CreateDirectory(LocalRepositoryDirectory);
        }
    }
}