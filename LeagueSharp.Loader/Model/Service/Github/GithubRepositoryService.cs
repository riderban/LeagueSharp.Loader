using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using LeagueSharp.Loader.Core;

namespace LeagueSharp.Loader.Model.Service.Github
{
    internal class GithubRepositoryService : IGithubRepositoryService
    {
        private const string Url =
            "https://raw.githubusercontent.com/LeagueSharp/LeagueSharpLoader/master/Updates/Repositories.txt";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<string> _cache = new List<string>();

        public void GetKnownRepositories(Action<ObservableCollection<string>> callback, bool forceUpdate = false)
        {
            if (_cache.Count > 0 && !forceUpdate)
            {
                callback(new ObservableCollection<string>(_cache));
                return;
            }

            Log.Info("Update Known Repositories " + Url);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var client = new GZipWebClient())
                    {
                        var repos = client.DownloadString(Url);
                        var matches = Regex.Matches(repos, "<repo>(.*)</repo>");

                        _cache.Clear();
                        foreach (Match match in matches)
                        {
                            _cache.Add(match.Groups[1].ToString());
                        }

                        callback(new ObservableCollection<string>(_cache));
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