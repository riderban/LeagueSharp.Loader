using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.Model.Service.LSharpDB;

namespace LeagueSharp.Loader.ViewModel
{
    internal class DatabaseViewModel : ViewModelBase
    {
        private ObservableCollection<LSharpDbAssembly> _database;

        public ObservableCollection<LSharpDbAssembly> Database
        {
            get { return _database; }
            set { Set(() => Database, ref _database, value); }
        }

        public DatabaseViewModel(ILSharpDbService serviceService)
        {
            serviceService.GetAssemblyDatabase((list, exception) =>
            {
                Database = list;
                CollectionViewSource.GetDefaultView(Database).SortDescriptions.Clear();
                CollectionViewSource.GetDefaultView(Database)
                    .SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            });
        }
    }
}