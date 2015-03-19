using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using LeagueSharp.Loader.Core;
using LeagueSharp.Loader.Model.Settings;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Service.LeagueSharp.Core
{
    internal class CoreUpdateService : ICoreUpdateService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<string> WhiteList = new List<string>
        {
            "https://github.com/joduskame/"
        };

        private static string UpdateZip
        {
            get { return Path.Combine(Directories.CoreDirectory, "update.zip"); }
        }

        private static string Url
        {
            get { return "http://api.joduska.me/public/deploy/kernel/"; }
        }

        public void Update(Action<CoreUpdateResponse> callback)
        {
            var leagueMd5 = Utility.Md5Checksum(Config.Instance.LeagueOfLegendsExePath);
            var coreMd5 = Utility.Md5Checksum(Directories.CoreFilePath);

            #region iwanttogetbanned

            if (Config.Instance.DeveloperSettings.IgnoreUpdate)
            {
                Log.Info("Ignore Update");
                callback(new CoreUpdateResponse {State = CoreUpdateState.UpToDate});
            }

            #endregion

            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var client = new GZipWebClient {Timeout = 4000})
                    {
                        var info = JsonConvert.DeserializeObject<UpdateInfo>(client.DownloadString(Url + leagueMd5));

                        #region VersionNotSupported

                        if (info.Version == "0")
                        {
                            Log.Info("League Version not Supported " + leagueMd5);
                            callback(new CoreUpdateResponse {State = CoreUpdateState.VersionNotSupported});
                            return;
                        }

                        #endregion

                        #region Update

                        if (info.Version != coreMd5 && WhiteList.Any(s => info.Url.StartsWith(s)))
                        {
                            Log.InfoFormat("Update Core from {0} to {1}", coreMd5, info.Version);

                            try
                            {
                                client.DownloadFile(info.Url, UpdateZip);

                                using (var archive = ZipFile.OpenRead(UpdateZip))
                                {
                                    foreach (var entry in archive.Entries)
                                    {
                                        var file = Path.Combine(Directories.CoreDirectory, entry.FullName);
                                        Log.Info("Update " + file);
                                        entry.ExtractToFile(file, true);
                                    }
                                }

                                callback(new CoreUpdateResponse {State = CoreUpdateState.Updated});
                            }
                            catch (Exception e)
                            {
                                Log.Warn("Core Download failed", e);
                                callback(new CoreUpdateResponse {State = CoreUpdateState.DownloadError});
                            }
                            finally
                            {
                                if (File.Exists(UpdateZip))
                                {
                                    File.Delete(UpdateZip);
                                }
                            }

                            return;
                        }

                        #endregion

                        #region UpToDate

                        callback(new CoreUpdateResponse {State = CoreUpdateState.UpToDate});

                        #endregion
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