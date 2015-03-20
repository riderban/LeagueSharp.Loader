using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using MahApps.Metro.Controls;

namespace LeagueSharp.Loader.Core
{
    internal class UriScheme
    {
        public const string Name = "ls";
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string FullName
        {
            get { return Name + "://"; }
        }

        public static void HandleUrl(string url, MetroWindow window)
        {
            url = WebUtility.UrlDecode(url.Remove(0, FullName.Length));
            var r = Regex.Matches(url, "(project|projectGroup)/([^/]*)/([^/]*)/([^/]*)/?");

            foreach (Match m in r)
            {
                var linkType = m.Groups[1].ToString();

                // TODO: decouple window
                //switch (linkType)
                //{
                //    case "project":
                //        var gitHubUser = m.Groups[2].ToString();
                //        var repositoryName = m.Groups[3].ToString();
                //        var assemblyName = m.Groups[4].ToString();
                //        var w = new InstallerWindow { Owner = window };
                //        w.ListAssemblies(
                //        string.Format("https://github.com/{0}/{1}", gitHubUser, repositoryName), true,
                //        assemblyName != "" ? m.Groups[4].ToString() : null);
                //        w.ShowDialog();
                //        break;

                //    case "projectGroup":
                //        var remaining = url.Remove(0, 13);
                //        var assemblies = new List<LeagueSharpAssembly>();
                //        while (remaining.IndexOf("/", StringComparison.InvariantCulture) != -1)
                //        {
                //            var data = remaining.Split(new[] { '/' });
                //            if (data.Length < 3)
                //            {
                //                break;
                //            }
                //            var assembly = new LeagueSharpAssembly(data[2], "",
                //            string.Format("https://github.com/{0}/{1}", data[0], data[1]));
                //            assemblies.Add(assembly);
                //            for (int i = 0; i < 3; i++)
                //            {
                //                remaining = remaining.Remove(0, remaining.IndexOf("/", StringComparison.InvariantCulture) + 1);
                //            }
                //        }
                //        if (assemblies.Count > 0)
                //        {
                //            assemblies.ForEach(
                //            assembly => Config.Instance.SelectedProfile.InstalledAssemblies.Add(assembly));
                //            ((MainWindow)window).PrepareAssemblies(assemblies, true, true, false);
                //            ((MainWindow)window).ShowTextMessage(Utility.GetMultiLanguageText("Installer"),
                //            Utility.GetMultiLanguageText("SuccessfullyInstalled"));
                //        }
                //        break;
                //}
            }
        }
    }
}