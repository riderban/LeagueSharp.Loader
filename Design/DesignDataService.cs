#region LICENSE

// Copyright 2015-2015 LeagueSharp.Loader
// DesignDataService.cs is part of LeagueSharp.Loader.
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

namespace LeagueSharp.Loader.Design
{
    #region

    using System;
    using System.Collections.ObjectModel;
    using LeagueSharp.Loader.Model;
    using LeagueSharp.Loader.Model.Service;

    #endregion

    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to LeagueSharp [design]");
            callback(item, null);
        }

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
                        Verion = "1.3.3.7",
                        Author = "h3h3",
                        Location = "https://github.com/h3h3/LeagueSharp"
                    }
                }, null);
        }
    }
}