using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Threading;
using log4net;
using LeagueSharp.Loader.Core;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Log;
using LeagueSharp.Loader.Model.Settings;

namespace LeagueSharp.Loader
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Mutex _mutex;

        static App()
        {
            DispatcherHelper.Initialize();
            Logs.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            #region Mutex

            Log.Info(Assembly.GetExecutingAssembly().GetName().Name + " started");

            var mutexCreated = false;
            var mutexName =
                Utility.Md5Hash(Directories.LoaderFilePath + Environment.UserDomainName + Environment.UserName);
            _mutex = new Mutex(true, mutexName, out mutexCreated);

            if (!mutexCreated)
            {
                if (e.Args.Length > 0)
                {
                    var wnd = Interop.FindWindow(IntPtr.Zero, "LeagueSharp");
                    if (wnd != IntPtr.Zero)
                    {
                        Clipboard.SetText(e.Args[0]);
                        Interop.ShowWindow(wnd, 5);
                        Interop.SetForegroundWindow(wnd);
                    }
                }
                _mutex = null;
                Environment.Exit(1);
            }
            else
            {
                Log.Info("Mutex Created " + mutexName);
            }

            #endregion

            #region Config

            // TODO: create default config
            //Utility.CreateFileFromResource(Directories.ConfigFilePath, "LeagueSharp.Loader.Resources.config.json");

            var configCorrupted = false;

            try
            {
                Log.Info("Loading Config " + Directories.ConfigFilePath.WithoutAppData());
                Utility.LoadFromJson<Config>(Directories.ConfigFilePath);
            }
            catch
            {
                configCorrupted = true;
            }

            if (!configCorrupted)
            {
                try
                {
                    Log.Info("Backing up valid Config " + Directories.ConfigFilePath.WithoutAppData() + ".bak");
                    File.SetAttributes(Directories.ConfigFilePath + ".bak", FileAttributes.Normal);
                    File.Copy(Directories.ConfigFilePath, Directories.ConfigFilePath + ".bak", true);
                    File.SetAttributes(Directories.ConfigFilePath + ".bak", FileAttributes.Hidden);
                }
                catch (Exception ex)
                {
                    Log.Warn(ex);
                }
            }
            else
            {
                try
                {
                    Log.Info("Restore Config backup " + Directories.ConfigFilePath.WithoutAppData() + ".bak");
                    Utility.LoadFromJson<Config>(Directories.ConfigFilePath + ".bak");
                    File.Copy(Directories.ConfigFilePath + ".bak", Directories.ConfigFilePath, true);
                    File.SetAttributes(Directories.ConfigFilePath, FileAttributes.Normal);
                }
                catch (Exception ex)
                {
                    File.Delete(Directories.ConfigFilePath + ".bak");
                    File.Delete(Directories.ConfigFilePath);
                    Log.Fatal("Couldn't load config", ex);
                    Environment.Exit(0);
                }
            }

            // TODO: remove after default config was created
            if (Config.Instance == null)
            {
                Config.Instance = new Config
                {
                    Install = true,
                    FirstRun = true,
                    UpdateOnLoad = true,
                    SelectedProfile = new Profile
                    {
                        Name = "Default",
                        InstalledAssemblies = new ObservableCollection<LeagueSharpAssembly>
                        {
                            new LeagueSharpAssembly
                            {
                                Name = "LeagueSharp.Common",
                                Author = "LeagueSharp",
                                Inject = true,
                                Location = "https://github.com/LeagueSharp/LeagueSharpCommon"
                            }
                        }
                    },
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
                                    Location = "https://github.com/LeagueSharp/LeagueSharpCommon"
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
                                PosibleValues = new List<string>
                                {
                                    "True",
                                    "False"
                                },
                                SelectedValue = "False"
                            },
                            new GameSettings
                            {
                                Name = "Debug Console",
                                PosibleValues = new List<string>
                                {
                                    "True",
                                    "False"
                                },
                                SelectedValue = "False"
                            },
                            new GameSettings
                            {
                                Name = "Display Enemy Tower Range",
                                PosibleValues = new List<string>
                                {
                                    "True",
                                    "False"
                                },
                                SelectedValue = "False"
                            },
                            new GameSettings
                            {
                                Name = "Extended Zoom",
                                PosibleValues = new List<string>
                                {
                                    "True",
                                    "False"
                                },
                                SelectedValue = "False"
                            }
                        }
                    }
                };
            }

            // HACK: testing
            Config.Instance.Username = "h3h3";
            var common = Config.Instance.SelectedProfile.InstalledAssemblies.First(a => a.Name == "LeagueSharp.Common");
            common.PathToRepository = Path.Combine(Directories.RepositoryDirectory, "Github", "LeagueSharp");
            GitUpdater.Clone(common.Location, common.PathToRepository, false);

            #endregion

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            #region Config

            try
            {
                if (e.ApplicationExitCode == 0)
                {
                    Log.Info("Save Config to " + Directories.ConfigFilePath.WithoutAppData());
                    Utility.SaveToJson(Config.Instance, Directories.ConfigFilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Couldn't save " + Directories.ConfigFilePath.WithoutAppData(), ex);
            }

            #endregion

            #region Mutex

            if (_mutex != null)
            {
                Log.Info("Release Mutex");
                _mutex.ReleaseMutex();
            }

            #endregion

            Log.InfoFormat("Exit({0})", e.ApplicationExitCode);

            base.OnExit(e);
        }
    }
}