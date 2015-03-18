using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Settings;
using LibGit2Sharp;

namespace LeagueSharp.Loader.Core
{
    internal class GitUpdater
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal static event TransferHandler OnTransferProgress;

        internal static bool Update(LeagueSharpAssembly assembly)
        {
            if (Repository.IsValid(assembly.PathToRepository))
            {
                try
                {
                    Fetch(assembly.PathToRepository, "origin");
                }
                catch (Exception e)
                {
                    Log.WarnFormat("Download failed {0} - {1}\n{2}", assembly.Name, assembly.Location, e);
                    return false;
                }

                try
                {
                    Checkout(assembly.PathToRepository, "origin/master");
                }
                catch (Exception e)
                {
                    Log.Warn(e);
                }

                return true;
            }

            return false;
        }

        [Obsolete("Use Update(LeagueSharpAssembly assembly) instead")]
        internal static string Update(string url, string directory)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Log.WarnFormat("Wrong Url specified - {0}", url);
            }
            else
            {
                try
                {
                    var dir = Path.Combine(directory, url.GetHashCode().ToString("X"));
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
                    Log.WarnFormat("{0} - {1}", ex.Message, url);
                }
            }
            return string.Empty;
        }

        internal static void ClearUnusedRepos(List<LeagueSharpAssembly> assemblyList)
        {
            try
            {
                var usedRepos = assemblyList.Select(assembly => assembly.Location.GetHashCode().ToString("X")).ToList();
                var dirs = new List<string>(Directory.EnumerateDirectories(Directories.RepositoryDirectory));

                foreach (var dir in dirs.Where(dir => !usedRepos.Contains(Path.GetFileName(dir))))
                {
                    Utility.ClearDirectory(dir);
                    Directory.Delete(dir);
                }
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        internal delegate void TransferHandler(string repository, TransferProgress progress);

        #region Git Commands

        internal static void Clone(string url, string path, bool overrideExisting = true)
        {
            try
            {
                Log.InfoFormat("Clone {0} into {1} override:{2}", url, path, overrideExisting);

                if (Directory.Exists(path) && overrideExisting)
                {
                    Utility.ClearDirectory(path);
                    Directory.Delete(path, true);
                }

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
                Log.Warn(e);
            }
        }

        internal static void Fetch(string repository, string remote)
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
                        Log.WarnFormat("Remote[{0}] not found in {1}", remote, repository);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        internal static void Checkout(string repository, string branch, CheckoutModifiers mods = CheckoutModifiers.Force)
        {
            try
            {
                using (var repo = new Repository(repository))
                {
                    if (IsValidBranch(repo.Branches[branch]))
                    {
                        repo.Merge(repo.Branches[branch],
                            new Signature(Config.Instance.Username, Config.Instance.Username + "@joduska.me",
                                DateTimeOffset.Now),
                            new MergeOptions {FileConflictStrategy = CheckoutFileConflictStrategy.Theirs});

                        repo.Checkout(branch,
                            new CheckoutOptions {CheckoutModifiers = mods});
                    }
                    else
                    {
                        Log.WarnFormat("Branch[{0}] not found in {1}", branch, repository);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        internal static TreeChanges Diff(string repository)
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
                Log.Warn(e);
            }

            return null;
        }

        internal static TreeChanges Diff(string repository, string branch1, string branch2)
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
                    Log.Warn("Branch not found");
                }
            }
            catch (Exception e)
            {
                Log.Warn(e);
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

        internal static bool IsValidRepository(string path)
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

        internal static bool IsValidBranch(Branch branch)
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
                Log.Warn(e);
            }
        }

        internal static void CleanRepository(string repository)
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
                Log.Warn(e);
            }
        }

        #endregion
    }
}