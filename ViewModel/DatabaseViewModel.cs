#region LICENSE

// Copyright 2015-2015 LeagueSharp.Loader
// DatabaseViewModel.cs is part of LeagueSharp.Loader.
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

namespace LeagueSharp.Loader.ViewModel
{
    #region

    using System.Collections.ObjectModel;
    using GalaSoft.MvvmLight;
    using LeagueSharp.Loader.Model;
    using LeagueSharp.Loader.Model.Service;

    #endregion

    /// <summary>
    ///     This class contains properties that a View can data bind to.
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class DatabaseViewModel : ViewModelBase
    {
        /// <summary>
        ///     The <see cref="Database" /> property's name.
        /// </summary>
        public const string DatabasePropertyName = "Database";

        private ObservableCollection<LeagueSharpAssembly> _database;

        public DatabaseViewModel(IDataService dataService)
        {
            dataService.GetAssemblyDatabase((list, exception) => { Database = list; });
        }

        /// <summary>
        ///     Sets and gets the Database property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public ObservableCollection<LeagueSharpAssembly> Database
        {
            get { return _database; }

            set
            {
                if (_database == value)
                {
                    return;
                }

                _database = value;
                RaisePropertyChanged(() => Database);
            }
        }
    }
}