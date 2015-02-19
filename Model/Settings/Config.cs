using System.Collections.ObjectModel;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Settings
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    internal class Config : ObservableObject
    {
        [XmlIgnore] public static Config Instance;
        private bool _firstRun = true;
        private Hotkeys _hotkeys;
        private bool _install = true;
        private ObservableCollection<string> _knownRepositories;
        private string _leagueOfLegendsExePath;
        private string _password;
        private ObservableCollection<Profile> _profiles;
        private string _selectedLanguage;
        private Profile _selectedProfile;
        private ConfigSettings _settings;
        private bool _showDevOptions;
        private bool _updateOnLoad;
        private string _username;

        public bool FirstRun
        {
            get { return _firstRun; }
            set { Set(() => FirstRun, ref _firstRun, value); }
        }

        public Hotkeys Hotkeys
        {
            get { return _hotkeys; }
            set { Set(() => Hotkeys, ref _hotkeys, value); }
        }

        public bool Install
        {
            get { return _install; }
            set { Set(() => Install, ref _install, value); }
        }

        [XmlArrayItem("KnownRepositories", IsNullable = true)]
        public ObservableCollection<string> KnownRepositories
        {
            get { return _knownRepositories; }
            set { Set(() => KnownRepositories, ref _knownRepositories, value); }
        }

        public string LeagueOfLegendsExePath
        {
            get { return _leagueOfLegendsExePath; }
            set { Set(() => LeagueOfLegendsExePath, ref _leagueOfLegendsExePath, value); }
        }

        public string Password
        {
            get { return _password; }
            set { Set(() => Password, ref _password, value); }
        }

        [XmlArrayItem("Profiles", IsNullable = true)]
        public ObservableCollection<Profile> Profiles
        {
            get { return _profiles; }
            set { Set(() => Profiles, ref _profiles, value); }
        }

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { Set(() => SelectedLanguage, ref _selectedLanguage, value); }
        }

        public Profile SelectedProfile
        {
            get { return _selectedProfile; }
            set { Set(() => SelectedProfile, ref _selectedProfile, value); }
        }

        public ConfigSettings Settings
        {
            get { return _settings; }
            set { Set(() => Settings, ref _settings, value); }
        }

        public bool ShowDevOptions
        {
            get { return _showDevOptions; }
            set { Set(() => ShowDevOptions, ref _showDevOptions, value); }
        }

        public bool UpdateOnLoad
        {
            get { return _updateOnLoad; }
            set { Set(() => UpdateOnLoad, ref _updateOnLoad, value); }
        }

        public string Username
        {
            get { return _username; }
            set { Set(() => Username, ref _username, value); }
        }
    }
}