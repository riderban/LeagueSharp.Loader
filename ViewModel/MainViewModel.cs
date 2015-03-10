using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Service;
using LeagueSharp.Loader.View;
using Microsoft.Practices.ServiceLocation;

namespace LeagueSharp.Loader.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        private FrameworkElement _assembliesView = new AssembliesView();
        private RelayCommand<FrameworkElement> _changeViewCommand;
        private FrameworkElement _currentAppBarViewElement;
        private FrameworkElement _currentView;
        private FrameworkElement _databaseView = new DatabaseView();
        private FrameworkElement _newsView = new NewsView();
        private ProgressController _progressController;
        private FrameworkElement _settingsView = new SettingsView();
        private RelayCommand _updateCommand;

        public FrameworkElement AssembliesView
        {
            get { return _assembliesView; }
            set { Set(() => AssembliesView, ref _assembliesView, value); }
        }

        public RelayCommand<FrameworkElement> ChangeViewCommand
        {
            get
            {
                return _changeViewCommand
                       ?? (_changeViewCommand = new RelayCommand<FrameworkElement>(
                           p => { CurrentView = p; }));
            }
        }

        public FrameworkElement CurrentAppBarView
        {
            get { return _currentAppBarViewElement; }
            set { Set(() => CurrentAppBarView, ref _currentAppBarViewElement, value); }
        }

        public FrameworkElement CurrentView
        {
            get { return _currentView; }
            set { Set(() => CurrentView, ref _currentView, value); }
        }

        public FrameworkElement DatabaseView
        {
            get { return _databaseView; }
            set { Set(() => DatabaseView, ref _databaseView, value); }
        }

        public FrameworkElement NewsView
        {
            get { return _newsView; }
            set { Set(() => NewsView, ref _newsView, value); }
        }

        public ProgressController ProgressController
        {
            get { return _progressController; }
            set { Set(() => ProgressController, ref _progressController, value); }
        }

        public FrameworkElement SettingsView
        {
            get { return _settingsView; }
            set { Set(() => SettingsView, ref _settingsView, value); }
        }

        public RelayCommand UpdateCommand
        {
            get
            {
                return _updateCommand
                       ?? (_updateCommand = new RelayCommand(
                           () =>
                           {
                               Task.Factory.StartNew(() =>
                               {
                                   var assemblies = ServiceLocator.Current.GetInstance<AssembliesViewModel>().Database;

                                   if (ProgressController.Start(0, 0, assemblies.Count))
                                   {
                                       foreach (var assembly in assemblies)
                                       {
                                           assembly.State = AssemblyState.Queue;
                                       }

                                       Parallel.ForEach(assemblies, new ParallelOptions {MaxDegreeOfParallelism = 5},
                                           (assembly, state) =>
                                           {
                                               assembly.Update();
                                               assembly.Compile();
                                               ProgressController.Value++;
                                           });

                                       ProgressController.Stop();
                                   }
                               });
                           }));
            }
        }

        public MainViewModel()
        {
            CurrentView = AssembliesView;
            CurrentAppBarView = new AppBarView();
            ProgressController = new ProgressController();
        }
    }
}