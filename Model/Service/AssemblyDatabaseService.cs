#region LICENSE

// Copyright 2015-2015 LeagueSharp.Loader
// AssemblyDatabaseService.cs is part of LeagueSharp.Loader.
// 
// LeagueSharp.Loader is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Loader is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Loader. If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace LeagueSharp.Loader.Model.Service
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using LibGit2Sharp;

    #endregion

    public class AssemblyDatabaseService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to LeagueSharp");
            callback(item, null);
        }

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