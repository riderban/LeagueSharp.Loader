#region

using System;
using System.Collections.ObjectModel;
using LeagueSharp.Loader.Model.Assembly;

#endregion

namespace LeagueSharp.Loader.Model.Service
{

    #region

    #endregion

    public interface IDataService
    {
        void GetAssemblyDatabase(Action<ObservableCollection<LeagueSharpAssembly>, Exception> callback);
    }
}