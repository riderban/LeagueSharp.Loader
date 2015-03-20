#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using LeagueSharp.Loader.Core;
using Newtonsoft.Json;

#endregion

namespace LeagueSharp.Loader.Model.Service.LSharpDB
{
    internal class LSharpDbService : ILSharpDbService
    {
        private const string Url = "http://lsharpdb.com/api/votes";
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<LSharpDbAssembly> _cache = new List<LSharpDbAssembly>();

        public void GetAssemblyDatabase(Action<ObservableCollection<LSharpDbAssembly>> callback,
            bool forceUpdate = false)
        {
            if (_cache.Count > 0 && !forceUpdate)
            {
                callback(new ObservableCollection<LSharpDbAssembly>(_cache));
                return;
            }

            Log.Info("Update Assembly Database " + Url);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var client = new GZipWebClient())
                    {
                        var data = JsonConvert.DeserializeObject<List<LSharpDbAssembly>>(client.DownloadString(Url));

                        _cache.Clear();
                        foreach (var assembly in data)
                        {
                            _cache.Add(assembly);
                        }

                        callback(new ObservableCollection<LSharpDbAssembly>(_cache));
                    }
                }
                catch (Exception e)
                {
                    Log.Warn(e);
                }
            });
        }
    }
}