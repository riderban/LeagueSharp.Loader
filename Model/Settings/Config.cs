using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using log4net;
using LeagueSharp.Loader.Core;
using LeagueSharp.Loader.Model.Assembly;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Settings
{
    internal class Config : ObservableObject
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        [JsonIgnore] public static Config Instance;
        private string _dataDirectory;
        private bool _firstRun = true;
        private Hotkeys _hotkeys;
        private bool _install = true;
        private string _leagueOfLegendsExePath;
        private string _password;
        private ObservableCollection<Profile> _profiles;
        private string _selectedLanguage;
        private int _selectedProfileId;
        private ConfigSettings _settings;
        private bool _showDevOptions;
        private bool _updateOnLoad = true;
        private string _username;

        public string DataDirectory
        {
            get { return _dataDirectory; }
            set { Set(() => DataDirectory, ref _dataDirectory, value); }
        }

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

        [JsonIgnore]
        public Profile SelectedProfile
        {
            get { return Profiles[SelectedProfileId]; }
        }

        public int SelectedProfileId
        {
            get { return _selectedProfileId; }
            set { Set(() => SelectedProfileId, ref _selectedProfileId, value); }
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

        public static void Save()
        {
            if (File.Exists(Directories.ConfigFilePath))
            {
                try
                {
                    Log.Info("Backup Config to " + Directories.ConfigFilePath + ".bak");
                    if (File.Exists(Directories.ConfigFilePath + ".bak"))
                    {
                        File.SetAttributes(Directories.ConfigFilePath + ".bak", FileAttributes.Normal);
                    }
                    File.Copy(Directories.ConfigFilePath, Directories.ConfigFilePath + ".bak", true);
                    File.SetAttributes(Directories.ConfigFilePath + ".bak", FileAttributes.Hidden);
                }
                catch (Exception ex)
                {
                    Log.Warn(ex);
                }
            }

            try
            {
                Log.Info("Save Config to " + Directories.ConfigFilePath);
                Utility.SaveToJson(Instance, Directories.ConfigFilePath);
            }
            catch (Exception ex)
            {
                Log.Error("Couldn't save " + Directories.ConfigFilePath, ex);
            }
        }

        public static void Initialize()
        {
            // TODO: create default config
            //Utility.CreateFileFromResource(Directories.ConfigFilePath, "LeagueSharp.Loader.Resources.config.json");

            var configCorrupted = false;

            try
            {
                Log.Info("Loading Config " + Directories.ConfigFilePath);
                Utility.LoadFromJson<Config>(Directories.ConfigFilePath);
            }
            catch
            {
                configCorrupted = true;
            }

            if (configCorrupted)
            {
                try
                {
                    Log.Info("Restore Config backup " + Directories.ConfigFilePath + ".bak");
                    Utility.LoadFromJson<Config>(Directories.ConfigFilePath + ".bak");
                    File.Copy(Directories.ConfigFilePath + ".bak", Directories.ConfigFilePath, true);
                    File.SetAttributes(Directories.ConfigFilePath, FileAttributes.Normal);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Utility.GetMultiLanguageText("ConfigLoadError"), "Config", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    File.Delete(Directories.ConfigFilePath + ".bak");
                    File.Delete(Directories.ConfigFilePath);
                    Log.Fatal("Couldn't load config", ex);
                    Environment.Exit(0);
                }
            }

            #region default Config

            // TODO: remove after default config was created
            if (Instance == null)
            {
                Instance = new Config
                {
                    Install = true,
                    FirstRun = true,
                    UpdateOnLoad = true,
                    SelectedProfileId = 0,
                    Profiles = new ObservableCollection<Profile>
                    {
                        new Profile
                        {
                            Name = "Default",
                            InstalledAssemblies = new ObservableCollection<LeagueSharpAssembly>
                            {
                                new LeagueSharpAssembly
                                {
                                    Name = "LeagueSharp.Common",
                                    Author = "LeagueSharp",
                                    Inject = true,
                                    Type = AssemblyType.Library,
                                    Location = "https://github.com/LeagueSharp/LeagueSharp.Common"
                                },
                                new LeagueSharpAssembly
                                {
                                    Name = "LeagueSharp.CommonEx",
                                    Author = "LeagueSharp",
                                    Inject = true,
                                    Type = AssemblyType.Library,
                                    Location = "https://github.com/LeagueSharp/LeagueSharp.CommonEx"
                                }
                            }
                        }
                    },
                    Hotkeys = new Hotkeys
                    {
                        SelectedHotkeys = new ObservableCollection<HotkeyEntry>
                        {
                            new HotkeyEntry
                            {
                                Name = "Reload",
                                Description = "Reload the assemblies",
                                Hotkey = Key.F5,
                                DefaultKey = Key.F5
                            },
                            new HotkeyEntry
                            {
                                Name = "Unload",
                                Description = "Unloads all assemblies",
                                Hotkey = Key.F6,
                                DefaultKey = Key.F6
                            },
                            new HotkeyEntry
                            {
                                Name = "CompileAndReload",
                                Description = "Recompile and reload the assemblies",
                                Hotkey = Key.F7,
                                DefaultKey = Key.F7
                            },
                            new HotkeyEntry
                            {
                                Name = "ShowMenuToggle",
                                Description = "Shows the menu (Toggle)",
                                Hotkey = Key.F8,
                                DefaultKey = Key.F8
                            },
                            new HotkeyEntry
                            {
                                Name = "ShowMenuPress",
                                Description = "Shows the menu (Press)",
                                Hotkey = Key.LeftShift,
                                DefaultKey = Key.LeftShift
                            }
                        }
                    },
                    Settings = new ConfigSettings
                    {
                        GameSettings = new ObservableCollection<GameSettings>
                        {
                            new GameSettings
                            {
                                Name = "Anti-AFK",
                                Values = new List<string>
                                {
                                    "True",
                                    "False"
                                },
                                Value = "False"
                            },
                            new GameSettings
                            {
                                Name = "Debug Console",
                                Values = new List<string>
                                {
                                    "True",
                                    "False"
                                },
                                Value = "False"
                            },
                            new GameSettings
                            {
                                Name = "Display Enemy Tower Range",
                                Values = new List<string>
                                {
                                    "True",
                                    "False"
                                },
                                Value = "False"
                            },
                            new GameSettings
                            {
                                Name = "Extended Zoom",
                                Values = new List<string>
                                {
                                    "True",
                                    "False"
                                },
                                Value = "False"
                            }
                        }
                    }
                };
            }

            #endregion
        }

        [OnDeserializing]
        internal void OnDeserializing(StreamingContext context)
        {
            Instance = this;
        }
    }
}