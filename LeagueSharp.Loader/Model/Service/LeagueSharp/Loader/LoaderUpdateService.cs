using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using LeagueSharp.Loader.Core;
using LeagueSharp.Loader.Model.Messages;
using LeagueSharp.Loader.Model.Settings;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Service.LeagueSharp.Loader
{
    internal class LoaderUpdateService : ILoaderUpdateService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<string> WhiteList = new List<string>
        {
            "https://github.com/joduskame/",
            "https://github.com/LeagueSharp/"
        };

        private static string UpdateExe
        {
            get { return Path.Combine(Directories.CurrentDirectory, "LeagueSharp-update.exe"); }
        }

        private static string Url
        {
            get { return "http://api.joduska.me/public/deploy/loader/version"; }
        }

        public void Update(Action<UpdateResponse> callback, UpdateInfo info = null)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var client = new GZipWebClient {Timeout = 4000})
                    {
                        if (info == null)
                        {
                            info = JsonConvert.DeserializeObject<UpdateInfo>(client.DownloadString(Url));
                        }

                        var loaderVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                        var updateVersion = Version.Parse(info.Version);

                        if (updateVersion > loaderVersion && WhiteList.Any(s => info.Url.StartsWith(s)))
                        {
                            Log.InfoFormat("Update Loader from {0} to {1}", loaderVersion, updateVersion);
                            Messenger.Default.Send(new BalloonTipLoaderUpdateMessage());
                            // TODO: implement balloontip service

                            try
                            {
                                client.DownloadFile(info.Url, UpdateExe);

                                using (var archive = ZipFile.OpenRead(UpdateExe))
                                {
                                    foreach (var entry in archive.Entries)
                                    {
                                        var file = Path.Combine(Directories.CoreDirectory, entry.FullName);
                                        Log.Info("Update " + file);
                                        entry.ExtractToFile(file, true);
                                    }
                                }

                                callback(new UpdateResponse {State = UpdateState.Updated});
                            }
                            catch (Exception e)
                            {
                                Log.Warn("Loader Download failed", e);
                                callback(new UpdateResponse {State = UpdateState.DownloadError});
                            }
                            finally
                            {
                                if (File.Exists(UpdateExe))
                                {
                                    File.Delete(UpdateExe);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Warn(e);
                    callback(new UpdateResponse {State = UpdateState.UpdateError});
                }
            });
        }

        public void Check(Action<UpdateResponse> callback)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var client = new GZipWebClient {Timeout = 4000})
                    {
                        var info = JsonConvert.DeserializeObject<UpdateInfo>(client.DownloadString(Url));
                        var loaderVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                        var updateVersion = Version.Parse(info.Version);

                        if (updateVersion > loaderVersion && WhiteList.Any(s => info.Url.StartsWith(s)))
                        {
                            callback(new UpdateResponse {State = UpdateState.UpdateAvailable, Info = info});
                            return;
                        }

                        callback(new UpdateResponse {State = UpdateState.UpToDate});
                    }
                }
                catch (Exception e)
                {
                    Log.Warn(e);
                    callback(new UpdateResponse {State = UpdateState.UpdateError});
                }
            });
        }
    }
}