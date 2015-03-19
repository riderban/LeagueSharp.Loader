using System;

namespace LeagueSharp.Loader.Model.Service.LeagueSharp.Core
{
    internal interface ICoreUpdateService
    {
        void Update(Action<CoreUpdateResponse> callback);
    }
}