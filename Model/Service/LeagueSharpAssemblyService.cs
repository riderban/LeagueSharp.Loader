using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Settings;
using LeagueSharp.Loader.ViewModel;
using LibGit2Sharp;
using Microsoft.Practices.ServiceLocation;

namespace LeagueSharp.Loader.Model.Service
{
    internal class LeagueSharpAssemblyService : ILeagueSharpAssemblyService
    {
        public void GetAssemblyData(Action<ObservableCollection<LeagueSharpAssembly>> callback, bool forceUpdate = false)
        {
            var assemblies = new ObservableCollection<LeagueSharpAssembly>();

            Task.Factory.StartNew(() =>
            {
                var progress = ServiceLocator.Current.GetInstance<MainViewModel>().ProgressController;
                if (progress.Start(0, 0, Config.Instance.SelectedProfile.InstalledAssemblies.Count))
                {
                    Parallel.ForEach(Config.Instance.SelectedProfile.InstalledAssemblies, assembly =>
                    {
                        UpdateAssembly(assembly);
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            assemblies.Add(assembly);
                            progress.Value++;
                        });
                    });

                    progress.Stop();
                }

                callback(assemblies);
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
                                Message = commit.MessageShort
                            });
                    }
                }

                assembly.Versions = versions;
            }
        }
    }
}