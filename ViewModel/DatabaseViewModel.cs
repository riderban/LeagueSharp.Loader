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
                var cv = CollectionViewSource.GetDefaultView(Database);
                cv.SortDescriptions.Clear();
                cv.GroupDescriptions.Clear();
                cv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                cv.GroupDescriptions.Add(new PropertyGroupDescription("Name"));
            });
        }
    }
}