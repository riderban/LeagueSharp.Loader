using System;
using System.IO;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using LeagueSharp.Loader.Core;
using LeagueSharp.Loader.Model.Log;
using LeagueSharp.Loader.Model.Settings;

namespace LeagueSharp.Loader
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private bool _createdNew;
        private Mutex _mutex;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            #region Mutex

            _mutex = new Mutex(true,
                Utility.Md5Hash(Directories.LoaderFilePath + Environment.UserDomainName + Environment.UserName),
                out _createdNew);

            if (!_createdNew)
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

            #endregion

            #region Config

            // TODO: create default config
            //Utility.CreateFileFromResource(Directories.ConfigFilePath, "LeagueSharp.Loader.Resources.config.json");

            var configCorrupted = false;

            try
            {
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
                    File.Copy(Directories.ConfigFilePath, Directories.ConfigFilePath + ".bak", true);
                    File.SetAttributes(Directories.ConfigFilePath + ".bak", FileAttributes.Hidden);
                }
                catch
                {
                    //ignore
                }
            }
            else
            {
                try
                {
                    Utility.LoadFromJson<Config>(Directories.ConfigFilePath + ".bak");
                    File.Copy(Directories.ConfigFilePath + ".bak", Directories.ConfigFilePath, true);
                    File.SetAttributes(Directories.ConfigFilePath, FileAttributes.Normal);
                }
                catch (Exception ex)
                {
                    File.Delete(Directories.ConfigFilePath + ".bak");
                    File.Delete(Directories.ConfigFilePath);
                    Utility.Log(LogLevel.Error, "Couldn't load config.json." + "\n" + ex.Message);
                    Environment.Exit(0);
                }
            }

            if (Config.Instance == null)
            {
                Config.Instance = new Config();
            }

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
                    Utility.SaveToJson(Config.Instance, Directories.ConfigFilePath);
                }
            }
            catch (Exception ex)
            {
                Utility.Log(LogLevel.Error, "Couldn't save " + Directories.ConfigFilePath + "\n" + ex.Message);
            }

            #endregion

            #region Mutex

            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
            }

            #endregion

            base.OnExit(e);
        }
    }
}