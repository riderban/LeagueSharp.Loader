using System;

namespace LeagueSharp.Loader.Model.Service.LeagueSharp.Core
{
    internal interface ICoreUpdateService
    {
        void Update(Action<UpdateResponse> callback, UpdateInfo info);
        void Check(Action<UpdateResponse> callback);
    }
}