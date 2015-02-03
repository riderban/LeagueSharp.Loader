#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LeagueSharp.Loader.Model.Assembly;
using LibGit2Sharp;

#endregion

namespace LeagueSharp.Loader.Model.Service
{

    #region

    #endregion

    public class AssemblyDatabaseService : IDataService
    {
        public void GetAssemblyDatabase(Action<ObservableCollection<LeagueSharpAssembly>, Exception> callback)
        {
            var supportVersions = new List<AssemblyVersion>();
            var commonVersions = new List<AssemblyVersion>();

            if (Repository.IsValid(@"D:\GitHub\LeagueSharp"))
            {
                using (var repo = new Repository(@"D:\GitHub\LeagueSharp"))
                {
                    var commits = repo.Head.Commits.ToList();
                    for (var i = 0; i < commits.Count; i++)
                    {
                        supportVersions.Add(
                            new AssemblyVersion
                            {
                                Id = commits.Count - i,
                                Date = commits[i].Committer.When,
                                Message = commits[i].MessageShort
                            });
                    }
                }

                using (var repo = new Repository(@"D:\GitHub\LeagueSharpCommon"))
                {
                    var commits = repo.Head.Commits.ToList();
                    for (var i = 0; i < commits.Count; i++)
                    {
                        commonVersions.Add(
                            new AssemblyVersion
                            {
                                Id = commits.Count - i,
                                Date = commits[i].Committer.When,
                                Message = commits[i].MessageShort
                            });
                    }
                }
            }

            // TODO: call webservice & cache data
            callback(
                new ObservableCollection<LeagueSharpAssembly>
                {
                    new LeagueSharpAssembly
                    {
                        Name = "LeagueSharp.Common",
                        Rating = 5,
                        Type = AssemblyType.Library,
                        Version = 0,
                        Author = "LeagueSharp",
                        Location = "https://github.com/LeagueSharp/LeagueSharp.Common",
                        Versions = commonVersions
                    },
                    new LeagueSharpAssembly
                    {
                        Name = "Support is too Easy",
                        Rating = 5,
                        Type = AssemblyType.Champion,
                        Version = 0,
                        Author = "h3h3",
                        Location = "https://github.com/h3h3/LeagueSharp",
                        Versions = supportVersions
                    }
                }, null);
        }
    }
}