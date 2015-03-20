using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Settings;
using LeagueSharp.Loader.Service;

namespace LeagueSharp.Loader.Core.Service
{
    internal class LoaderService : ILoaderService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<LSharpAssembly> GetAssemblyList(int pid)
        {
            throw new NotImplementedException();
            //return
            //    Config.Instance.SelectedProfile.InstalledAssemblies.Where(
            //        a => a.Inject && a.Type != AssemblyType.Library).Select(assembly => assembly.PathToBinary).ToList();
        }

        public LoginCredentials GetLoginCredentials(int pid)
        {
            return new LoginCredentials
            {
                User = Config.Instance.Username,
                Password = Config.Instance.Password
            };
        }

        public Configuration GetConfiguration(int pid)
        {
            var reloadKey = Config.Instance.Hotkeys.SelectedHotkeys
                .First(h => h.Name == "Reload").HotkeyInt;

            var unloadKey = Config.Instance.Hotkeys.SelectedHotkeys
                .First(h => h.Name == "Unload").HotkeyInt;

            var recompileKey = Config.Instance.Hotkeys.SelectedHotkeys
                .First(h => h.Name == "CompileAndReload").HotkeyInt;

            var menuKey = Config.Instance.Hotkeys.SelectedHotkeys
                .First(h => h.Name == "ShowMenuPress").HotkeyInt;

            var menuToggleKey = Config.Instance.Hotkeys.SelectedHotkeys
                .First(h => h.Name == "ShowMenuToggle").HotkeyInt;

            var afk = Config.Instance.Settings.GameSettings
                .First(s => s.Name == "Anti-AFK").Value == "True";

            var console = Config.Instance.Settings.GameSettings
                .First(s => s.Name == "Debug Console").Value == "True";

            var tower = Config.Instance.Settings.GameSettings
                .First(s => s.Name == "Display Enemy Tower Range").Value == "True";

            var zoom = Config.Instance.Settings.GameSettings
                .First(s => s.Name == "Extended Zoom").Value == "True";

            return new Configuration
            {
                DataDirectory = Directories.DataDirectory,
                ReloadKey = reloadKey,
                ReloadAndRecompileKey = recompileKey,
                UnloadKey = unloadKey,
                MenuKey = menuKey,
                MenuToggleKey = menuToggleKey,
                AntiAfk = afk,
                Console = console,
                TowerRange = tower,
                ExtendedZoom = zoom
            };
        }

        public void Recompile(int pid)
        {
            var assemblies =
                Config.Instance.SelectedProfile.InstalledAssemblies.Where(
                    a => a.Inject || a.Type == AssemblyType.Library).ToList();

            foreach (var assembly in assemblies.Where(a => a.Type == AssemblyType.Library))
            {
                assembly.Compile();
            }

            foreach (var assembly in assemblies.Where(
                a => a.Type == AssemblyType.Champion || a.Type == AssemblyType.Utility))
            {
                assembly.Compile();
            }
        }
    }
}