using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LeagueSharp.Loader.Design;
using LeagueSharp.Loader.Model.Service;
using LeagueSharp.Loader.Model.Service.Github;
using LeagueSharp.Loader.Model.Service.LSharpDB;
using LeagueSharp.Loader.ViewModel.Settings;
using Microsoft.Practices.ServiceLocation;

namespace LeagueSharp.Loader.ViewModel
{
    internal class ViewModelLocator
    {
        public AppBarViewModel AppBar
        {
            get { return ServiceLocator.Current.GetInstance<AppBarViewModel>(); }
        }

        public AssembliesViewModel Assemblies
        {
            get { return ServiceLocator.Current.GetInstance<AssembliesViewModel>(); }
        }

        public DatabaseViewModel Database
        {
            get { return ServiceLocator.Current.GetInstance<DatabaseViewModel>(); }
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<ILeagueSharpAssemblyService, DesignServiceService>();
                SimpleIoc.Default.Register<ILSharpDbService, DesignServiceService>();
            }
            else
            {
                SimpleIoc.Default.Register<ILeagueSharpAssemblyService, LeagueSharpAssemblyService>(true);
                SimpleIoc.Default.Register<ILSharpDbService, LSharpDbService>(true);
                SimpleIoc.Default.Register<IGithubRepositoryService, GithubRepositoryService>(true);
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AppBarViewModel>();
            SimpleIoc.Default.Register<DatabaseViewModel>();
            SimpleIoc.Default.Register<AssembliesViewModel>();
            SimpleIoc.Default.Register<InstallerViewModel>();
            SimpleIoc.Default.Register<UpdaterViewModel>();

            #region Settings

            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<GeneralViewModel>();
            SimpleIoc.Default.Register<HotkeysViewModel>();
            SimpleIoc.Default.Register<LogViewModel>();
            SimpleIoc.Default.Register<MenuConfigViewModel>();

            #endregion
        }

        public static void Cleanup()
        {
        }
    }
}