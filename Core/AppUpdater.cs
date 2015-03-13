using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using log4net;
using LeagueSharp.Loader.Model.Settings;
using LeagueSharp.Loader.View;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Core
{
    internal class AppUpdater
    {
        public delegate void RepositoriesUpdateDelegate(List<string> list);

        public const string VersionCheckUrl = "http://api.joduska.me/public/deploy/loader/version";
        public const string CoreVersionCheckUrl = "http://api.joduska.me/public/deploy/kernel/{0}";
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static string UpdateZip = Path.Combine(Directories.CoreDirectory, "update.zip");
        public static string SetupFile = Path.Combine(Directories.CurrentDirectory, "LeagueSharp-update.exe");
        public static MainWindow MainWindow;
        public static bool Updating = false;
        public static bool CheckedForUpdates = false;

        public static Tuple<bool, string> CheckLoaderVersion()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var data = client.DownloadString(VersionCheckUrl);
                    var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(data);
                    var v = Version.Parse(updateInfo.Version);

                    if (Utility.VersionToInt(Assembly.GetEntryAssembly().GetName().Version) < Utility.VersionToInt(v))
                    {
                        return new Tuple<bool, string>(true, updateInfo.Url);
                    }
                }
            }
            catch
            {
                return new Tuple<bool, string>(false, "");
            }
            return new Tuple<bool, string>(false, "");
        }

        public static void UpdateLoader(Tuple<bool, string> versionCheckResult)
        {
            if (versionCheckResult.Item1 &&
                (versionCheckResult.Item2.StartsWith("https://github.com/LeagueSharp/") ||
                 versionCheckResult.Item2.StartsWith("https://github.com/joduskame/") ||
                 versionCheckResult.Item2.StartsWith("https://github.com/Esk0r/")))
            {
                // TODO: update window
                //var window = new UpdateWindow();
                //window.UpdateUrl = versionCheckResult.Item2;
                //window.ShowDialog();
            }
        }

        public static Tuple<bool, bool?, string> UpdateCore(string leagueOfLegendsFilePath, bool showMessages)
        {
            if (Directory.Exists(Path.Combine(Directories.CurrentDirectory, "iwanttogetbanned")))
            {
                return new Tuple<bool, bool?, string>(true, true, Utility.GetMultiLanguageText("NotUpdateNeeded"));
            }
            try
            {
                var leagueMd5 = Utility.Md5Checksum(leagueOfLegendsFilePath);
                var wr = WebRequest.Create(string.Format(CoreVersionCheckUrl, leagueMd5));
                wr.Timeout = 4000;
                wr.Method = WebRequestMethods.Http.Get;
                var response = wr.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var ser = new DataContractJsonSerializer(typeof (UpdateInfo));
                        var updateInfo = (UpdateInfo) ser.ReadObject(stream);
                        if (updateInfo.Version == "0")
                        {
                            var message = Utility.GetMultiLanguageText("WrongVersion") + leagueMd5;
                            if (showMessages)
                            {
                                MessageBox.Show(message);
                            }
                            return new Tuple<bool, bool?, string>(false, false, message);
                        }
                        if (updateInfo.Version != Utility.Md5Checksum(Directories.CoreFilePath) &&
                            updateInfo.Url.StartsWith("https://github.com/joduskame/")) //Update needed
                        {
                            if (MainWindow != null)
                            {
                                // TODO: decouple window
                                //MainWindow.TrayIcon.ShowBalloonTip(
                                //Utility.GetMultiLanguageText("Updating"),
                                //"LeagueSharp.Core: " + Utility.GetMultiLanguageText("Updating"), BalloonIcon.Info);
                            }
                            try
                            {
                                if (File.Exists(UpdateZip))
                                {
                                    File.Delete(UpdateZip);
                                    Thread.Sleep(500);
                                }
                                using (var webClient = new WebClient())
                                {
                                    webClient.DownloadFile(updateInfo.Url, UpdateZip);
                                    using (var archive = ZipFile.OpenRead(UpdateZip))
                                    {
                                        foreach (var entry in archive.Entries)
                                        {
                                            entry.ExtractToFile(
                                                Path.Combine(Directories.CoreDirectory, entry.FullName), true);
                                        }
                                    }
                                }
                                return new Tuple<bool, bool?, string>(
                                    true, true, Utility.GetMultiLanguageText("UpdateSuccess"));
                            }
                            catch (Exception e)
                            {
                                var message = Utility.GetMultiLanguageText("FailedToDownload") + e;
                                if (showMessages)
                                {
                                    MessageBox.Show(message);
                                }
                                return new Tuple<bool, bool?, string>(false, false, message);
                            }
                            finally
                            {
                                if (File.Exists(UpdateZip))
                                {
                                    File.Delete(UpdateZip);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(e.ToString());
                return new Tuple<bool, bool?, string>(
                    File.Exists(Directories.CoreFilePath), null, Utility.GetMultiLanguageText("UpdateUnknown"));
            }
            return new Tuple<bool, bool?, string>(
                File.Exists(Directories.CoreFilePath), true, Utility.GetMultiLanguageText("NotUpdateNeeded"));
        }

        public static void GetRepositories(RepositoriesUpdateDelegate del)
        {
            var wb = new WebClient();
            wb.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs args)
            {
                var result = new List<string>();
                var matches = Regex.Matches(args.Result, "<repo>(.*)</repo>");
                foreach (Match match in matches)
                {
                    result.Add(match.Groups[1].ToString());
                }
                del(result);
            };
            wb.DownloadStringAsync(
                new Uri(
                    "https://raw.githubusercontent.com/LeagueSharp/LeagueSharpLoader/master/Updates/Repositories.txt"));
        }

        internal class UpdateInfo
        {
            [JsonProperty(PropertyName = "url")]
            public string Url { get; set; }

            [JsonProperty(PropertyName = "version")]
            public string Version { get; set; }
        }
    }
}