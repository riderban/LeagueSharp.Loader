using System;
using System.Collections.ObjectModel;
using LeagueSharp.Loader.Model.Assembly;

namespace LeagueSharp.Loader.Model.Service.Git
{
    internal interface ILeagueSharpAssemblyService
    {
        void GetAssemblyData(Action<ObservableCollection<LeagueSharpAssembly>> callback, bool forceUpdate = false);
    }
}