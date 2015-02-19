using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Log;
using LeagueSharp.Loader.Model.Settings;
using LibGit2Sharp;
using LogLevel = LeagueSharp.Loader.Model.Log.LogLevel;

namespace LeagueSharp.Loader.Core
{
    internal class GitUpdater
    {
        public static string Update(string url, Log log, string directory)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Logs.Main.Items.Add(new LogItem
                {
                    Level = LogLevel.Warning,
                    Source = "GitUpdater",
                    Message = string.Format("No Url specified - {0}", url)
                });
            }
            else
            {
                try
                {
                    var dir = Path.Combine(directory, url.GetHashCode().ToString("X"), "trunk");
                    if (Repository.IsValid(dir))
                    {
                        using (var repo = new Repository(dir))
                        {
                            repo.Config.Set("user.name", Config.Instance.Username);
                            repo.Config.Set("user.email", Config.Instance.Username + "@joduska.me");
                            repo.Fetch("origin");
                            repo.Checkout("origin/master",
                                new CheckoutOptions {CheckoutModifiers = CheckoutModifiers.Force});
                        }
                    }
                    else
                    {
                        var oldPath = Path.Combine(directory, url.GetHashCode().ToString("X"));
                        if (Directory.Exists(oldPath))
                        {
                            Directory.Delete(oldPath, true);
                        }
                        Repository.Clone(url, dir, new CloneOptions {Checkout = true});
                        using (var repo = new Repository(dir))
                        {
                            repo.Config.Set("user.name", Config.Instance.Username);
                            repo.Config.Set("user.email", Config.Instance.Username + "@joduska.me");
                        }
                    }
                    return dir;
                }
                catch (Exception ex)
                {
                    Utility.Log(LogLevel.Error, string.Format("{0} - {1}", ex.Message, url));
                }
            }
            return string.Empty;
        }

        public static void ClearUnusedRepos(List<LeagueSharpAssembly> assemblyList)
        {
            try
            {
                var usedRepos = assemblyList.Select(assembly => assembly.Location.GetHashCode().ToString("X")).ToList();
                var dirs = new List<string>(Directory.EnumerateDirectories(Directories.RepositoryDir));

                foreach (var dir in dirs.Where(dir => !usedRepos.Contains(Path.GetFileName(dir))))
                {
                    Utility.ClearDirectory(dir);
                    Directory.Delete(dir);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}