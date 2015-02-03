#region

using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Service;

#endregion

namespace LeagueSharp.Loader.ViewModel
{

    #region

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