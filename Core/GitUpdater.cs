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
        internal delegate void TransferHandler(string repository, TransferProgress progress);
        internal static event TransferHandler OnTransferProgress;

        #region Git Commands

        private static void Clone(string url, string path, bool overrideExisting = true)
        {
            try
            {
                if (Directory.Exists(path) && overrideExisting)
                    Directory.Delete(path, true);

                Repository.Clone(url, path,
                    new CloneOptions
                    {
                        Checkout = true,
                        OnTransferProgress = progress =>
                        {
                            RaiseOnTransferProgress(url, progress);
                            return true;
                        }
                    });

                FixRepositoryConfig(path);
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning, e.Message);
            }
        }

        private static void Fetch(string repository, string remote)
        {
            try
            {
                using (var repo = new Repository(repository))
                {
                    if (repo.Network.Remotes.Any(r => r.Name == remote))
                    {
                        var url = repo.Network.Remotes[remote].Url;

                        repo.Fetch(remote,
                            new FetchOptions
                            {
                                OnTransferProgress = progress =>
                                {
                                    RaiseOnTransferProgress(url, progress);
                                    return true;
                                }
                            });
                    }
                    else
                    {
                        Utility.Log(LogLevel.Warning, string.Format("Remote[{0}] not found in {1}", remote, repository));
                    }
                }
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning, e.Message);
            }
        }

        private static void Checkout(string repository, string branch, CheckoutModifiers mods = CheckoutModifiers.Force)
        {
            try
            {
                using (var repo = new Repository(repository))
                {
                    if (IsValidBranch(repo.Branches[branch]))
                    {
                        repo.Merge(repo.Branches[branch],
                            new Signature(Config.Instance.Username, Config.Instance.Username + "@joduska.me", DateTimeOffset.Now),
                            new MergeOptions { FileConflictStrategy = CheckoutFileConflictStrategy.Theirs });

                        repo.Checkout("head",
                            new CheckoutOptions { CheckoutModifiers = mods });
                    }
                    else
                    {
                        Utility.Log(LogLevel.Warning, string.Format("Branch[{0}] not found in {1}", branch, repository));
                    }
                }
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning, e.Message);
            }
        }

        private static TreeChanges Diff(string repository)
        {
            try
            {
                using (var repo = new Repository(repository))
                {
                    return repo.Diff.Compare<TreeChanges>();
                }
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning, e.Message);
            }

            return null;
        }

        private static TreeChanges Diff(string repository, string branch1, string branch2)
        {
            try
            {
                using (var repo = new Repository(repository))
                {
                    if (IsValidBranch(repo.Branches[branch1]) && IsValidBranch(repo.Branches[branch2]))
                    {
                        var tree1 = repo.Branches[branch1].Tip.Tree;
                        var tree2 = repo.Branches[branch2].Tip.Tree;

                        return repo.Diff.Compare<TreeChanges>(tree1, tree2);
                    }
                    else
                    {
                        Utility.Log(LogLevel.Warning, "Branch not found");
                    }
                }
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning, e.Message);
            }

            return null;
        }

        #endregion

        #region Utility

        private static void RaiseOnTransferProgress(string repository, TransferProgress progress)
        {
            if (OnTransferProgress != null)
                OnTransferProgress(repository, progress);
        }

        private static bool IsValidRepository(string path)
        {
            try
            {
                if (Repository.IsValid(path))
                {
                    using (var repo = new Repository(path))
                    {
                        if (repo.Head.Tip != null && repo.Head.Tip.Tree != null)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private static bool IsValidBranch(Branch branch)
        {
            return branch != null && branch.Tip != null && branch.Tip.Tree != null;
        }

        private static void FixRepositoryConfig(string repository)
        {
            try
            {
                if (IsValidRepository(repository))
                {
                    using (var repo = new Repository(repository))
                    {
                        repo.Config.Set("user.name", Config.Instance.Username);
                        repo.Config.Set("user.email", Config.Instance.Username + "@joduska.me");
                    }
                }
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning, e.Message);
            }
        }

        private static void CleanRepository(string repository)
        {
            try
            {
                if (IsValidRepository(repository))
                {
                    using (var repo = new Repository(repository))
                    {
                        repo.RemoveUntrackedFiles();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning, e.Message);
            }
        }

        #endregion

        internal static bool Update(LeagueSharpAssembly assembly)
        {
            assembly.State = AssemblyState.Downloading;
            assembly.State = AssemblyState.Ready;
            return false;
        }

        [Obsolete("Use Update(LeagueSharpAssembly assembly) instead")]
        internal static string Update(string url, Log log, string directory)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Utility.Log(LogLevel.Warning, string.Format("Wrong Url specified - {0}", url));
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

        internal static void ClearUnusedRepos(List<LeagueSharpAssembly> assemblyList)
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
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning, e.Message);
            }
        }
    }
}