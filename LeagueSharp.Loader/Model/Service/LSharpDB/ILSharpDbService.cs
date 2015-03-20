using System;
using System.Collections.ObjectModel;

namespace LeagueSharp.Loader.Model.Service.LSharpDB
{
    internal interface ILSharpDbService
    {
        void GetAssemblyDatabase(Action<ObservableCollection<LSharpDbAssembly>> callback, bool forceUpdate = false);
    }
}