using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Service.LSharpDB
{
    internal class IlSharpDbService : ILSharpDbService
    {
        private const string Url =
            "http://lsharpdb.com/api/votes?key=8UzzX1rdGuDR3XDq6DPbCo34Wx6T15scnti6AfvS8REDcI7Bq375YX3MgjOP154W";

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