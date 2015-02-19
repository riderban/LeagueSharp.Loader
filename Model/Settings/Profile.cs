using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.Model.Assembly;

namespace LeagueSharp.Loader.Model.Settings
{
    internal class Profile : ObservableObject
    {
        private ObservableCollection<LeagueSharpAssembly> _installedAssemblies;
        private string _name;

        public ObservableCollection<LeagueSharpAssembly> InstalledAssemblies
        {
            get { return _installedAssemblies; }
            set { Set(() => InstalledAssemblies, ref _installedAssemblies, value); }
        }

        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }
    }
}