#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Service;

#endregion

namespace LeagueSharp.Loader.Design
{

    #region

    #endregion

    public class DesignDataService : IDataService
    {
        public void GetAssemblyDatabase(Action<ObservableCollection<LeagueSharpAssembly>, Exception> callback)
        {
            callback(
                new ObservableCollection<LeagueSharpAssembly>
                {
                    new LeagueSharpAssembly
                    {
                        Name = "Support is too Easy",
                        Rating = 5,
                        Type = AssemblyType.Executable,
                        Version = 0,
                        Author = "h3h3",
                        Location = "https://github.com/h3h3/LeagueSharp",
                        Versions = new List<AssemblyVersion>()
                    }
                }, null);
        }
    }
}