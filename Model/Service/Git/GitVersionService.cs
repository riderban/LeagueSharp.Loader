using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Settings;
using LeagueSharp.Loader.ViewModel;
using LibGit2Sharp;
using Microsoft.Practices.ServiceLocation;

namespace LeagueSharp.Loader.Model.Service.Git
{
    internal class GitVersionService : IGitVersionService
    {
        private readonly List<LeagueSharpAssembly> _cache = new List<LeagueSharpAssembly>();

        public void GetAssemblyData(Action<ObservableCollection<LeagueSharpAssembly>> callback, bool forceUpdate = false)
        {
            if (_cache.Count > 0 && !forceUpdate)
            {
                callback(new ObservableCollection<LeagueSharpAssembly>(_cache));
                return;
            }

            Task.Factory.StartNew(() =>
            {
                var progress = ServiceLocator.Current.GetInstance<MainViewModel>().ProgressController;
                if (progress.Start(0, 0, Config.Instance.SelectedProfile.InstalledAssemblies.Count))
                {
                    Parallel.ForEach(Config.Instance.SelectedProfile.InstalledAssemblies, assembly =>
                    {
                        UpdateAssembly(assembly);
                        _cache.Add(assembly);
                        progress.Value++;
                    });

                    progress.Stop();
                }

                callback(new ObservableCollection<LeagueSharpAssembly>(_cache));
            });
        }

        private static void UpdateAssembly(LeagueSharpAssembly assembly)
        {
            if (Repository.IsValid(assembly.PathToRepository))
            {
                var versions = new List<AssemblyVersion>();

                using (var repo = new Repository(assembly.PathToRepository))
                {
                    var commits = repo.Head.Commits.ToList();
                    var id = commits.Count;
                    foreach (var commit in commits)
                    {
                        versions.Add(
                            new AssemblyVersion
                            {
                                Id = id--,
                                Date = commit.Committer.When,
                                Message = commit.MessageShort,
                                Hash = commit.Sha
                            });
                    }
                }

                assembly.Versions = versions;
            }
        }
    }
}