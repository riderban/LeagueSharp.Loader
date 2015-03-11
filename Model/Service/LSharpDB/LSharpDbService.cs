using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Service.LSharpDB
{
    internal class IlSharpDbService : ILSharpDbService
    {
        private const string Url = "http://lsharpdb.com/api/votes";

        public void GetAssemblyDatabase(Action<ObservableCollection<LSharpDbAssembly>, Exception> callback)
        {
            using (var client = new WebClient())
            {
                callback(
                    new ObservableCollection<LSharpDbAssembly>(
                        JsonConvert.DeserializeObject<List<LSharpDbAssembly>>(client.DownloadString(Url))), null);
            }
        }
    }
}