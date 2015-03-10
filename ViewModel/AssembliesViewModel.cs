using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Service;
using LeagueSharp.Loader.Model.Settings;

namespace LeagueSharp.Loader.ViewModel
{
    internal class AssembliesViewModel : ViewModelBase
    {
        private ObservableCollection<LeagueSharpAssembly> _database;

        public AssembliesViewModel(IDataService dataService)
        {
            dataService.GetAssemblyDatabase((list, exception) =>
            {
                Database = list;
                CollectionViewSource.GetDefaultView(Database).SortDescriptions.Clear();
                CollectionViewSource.GetDefaultView(Database)
                    .SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            });
        }

        public ObservableCollection<LeagueSharpAssembly> Database
        {
            get { return _database; }
            set { Set(() => Database, ref _database, value); }
        }

        public Config Config
        {
            get { return Config.Instance; }
        }
    }
}