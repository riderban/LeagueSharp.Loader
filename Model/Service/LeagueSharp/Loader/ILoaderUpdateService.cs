using System;

namespace LeagueSharp.Loader.Model.Service.LeagueSharp.Loader
{
    internal interface ILoaderUpdateService
    {
        void Update(Action<UpdateResponse> callback, UpdateInfo info);
        void Check(Action<UpdateResponse> callback);
    }
}