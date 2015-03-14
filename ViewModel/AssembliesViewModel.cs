using System;
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
        private ObservableCollection<LeagueSharpAssembly> _assemblies;

        public Config Config
        {
            get { return Config.Instance; }
        }

        public ObservableCollection<LeagueSharpAssembly> Assemblies
        {
            get { return _assemblies; }
            set { Set(() => Assemblies, ref _assemblies, value); }
        }

        public AssembliesViewModel(ILeagueSharpAssemblyService dataService)
        {
            dataService.GetAssemblyData(collection =>
            {
                Assemblies = collection;
                CollectionViewSource.GetDefaultView(Assemblies).SortDescriptions.Clear();
                CollectionViewSource.GetDefaultView(Assemblies)
                    .SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            });
        }
    }
}