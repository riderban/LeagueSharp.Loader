#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

#endregion

namespace LeagueSharp.Loader.Model.Service.LSharpDB
{
    internal class IlSharpDbService : ILSharpDbService
    {
        private const string Url = "http://lsharpdb.com/api/votes";

        public void GetAssemblyDatabase(Action<ObservableCollection<LSharpDbAssembly>, Exception> callback)
        {
            Task.Factory.StartNew(() =>
            {
                using (var client = new WebClient())
                {
                    callback(
                        new ObservableCollection<LSharpDbAssembly>(
                            JsonConvert.DeserializeObject<List<LSharpDbAssembly>>(client.DownloadString(Url))), null);
                }
            });
        }
    }
}