using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LeagueSharp.Loader.Core.Compiler;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Service;
using LeagueSharp.Loader.View;
using Microsoft.Practices.ServiceLocation;

namespace LeagueSharp.Loader.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        private FrameworkElement _currentAppBarViewElement;
        private FrameworkElement _currentView;
        private ProgressController _progressController;
        private RelayCommand _updateCommand;

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            CurrentView = new DatabaseView();
            CurrentAppBarView = new AppBarView();
            ProgressController = new ProgressController();
        }

        /// <summary>
        ///     Sets and gets the CurrentAppBarView property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public FrameworkElement CurrentAppBarView
        {
            get { return _currentAppBarViewElement; }
            set { Set(() => CurrentAppBarView, ref _currentAppBarViewElement, value); }
        }

        /// <summary>
        ///     Sets and gets the CurrentView property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public FrameworkElement CurrentView
        {
            get { return _currentView; }
            set { Set(() => CurrentView, ref _currentView, value); }
        }

        /// <summary>
        ///     Sets and gets the ProgressController property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public ProgressController ProgressController
        {
            get { return _progressController; }
            set { Set(() => ProgressController, ref _progressController, value); }
        }

        /// <summary>
        ///     Gets the UpdateCommand.
        /// </summary>
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
                                   var assemblies = ServiceLocator.Current.GetInstance<DatabaseViewModel>().Database;
                                   if (ProgressController.Start(0, 0, assemblies.Count))
                                   {
                                       foreach (var assembly in assemblies)
                                       {
                                           assembly.State = AssemblyState.Queue;
                                       }

                                       Parallel.ForEach(assemblies, new ParallelOptions {MaxDegreeOfParallelism = 5},
                                           (assembly, state) =>
                                           {
                                               assembly.State = AssemblyState.Compiling;
                                               RoslynCompiler.Compile(assembly);
                                               assembly.State = AssemblyState.Ready;
                                               ProgressController.Value++;
                                           });

                                       ProgressController.Stop();
                                   }
                               });
                           }));
            }
        }
    }
}