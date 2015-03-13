using System;
using System.Collections.ObjectModel;

namespace LeagueSharp.Loader.Model.Service.Github
{
    internal interface IGithubRepositoryService
    {
        void GetKnownRepositories(Action<ObservableCollection<string>> callback, bool forceUpdate = false);
    }
}