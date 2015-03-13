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
                if (progress.Start(0, 0,
                    Directory.EnumerateFiles(Directories.RepositoryDirectory, "*.csproj", SearchOption.AllDirectories)
                        .Count()))
                {
                    Parallel.ForEach(Directory.EnumerateDirectories(Directories.RepositoryDirectory), dir =>
                    {
                        var asm = CreateAssembly(dir);
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            foreach (var assembly in asm)
                            {
                                assemblies.Add(assembly);
                                progress.Value++;
                            }
                        });
                    });

                    progress.Stop();
                }
            });

            callback(assemblies);
        }

        private static IEnumerable<LeagueSharpAssembly> CreateAssembly(string repoPath)
        {
            var assemblies = new List<LeagueSharpAssembly>();

            if (Repository.IsValid(repoPath))
            {
                var location = "";
                var author = "";
                var versions = new List<AssemblyVersion>();

                using (var repo = new Repository(repoPath))
                {
                    location = repo.Network.Remotes.First().Url;
                    author = location.Split('/')[3];

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

                foreach (var project in Directory.EnumerateFiles(repoPath, "*.csproj", SearchOption.AllDirectories))
                {
                    assemblies.Add(new LeagueSharpAssembly
                    {
                        Type = AssemblyType.Unknown,
                        State = AssemblyState.Ready,
                        Name = Path.GetFileNameWithoutExtension(project),
                        Location = location,
                        Author = author,
                        Project = project,
                        Versions = versions
                    });
                }
            }

            return assemblies;
        }
    }
}