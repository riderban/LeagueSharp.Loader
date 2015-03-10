using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using LeagueSharp.Loader.Model.Log;
using LeagueSharp.Loader.Model.Settings;

namespace LeagueSharp.Loader.Core.Compiler
{
    internal class NuGetResolver
    {
        public static void Resolve(string config)
        {
            var serializer = new XmlSerializer(typeof(NuGetPackages));
            var packages = (NuGetPackages)serializer.Deserialize(new FileStream(config, FileMode.Open));

            //foreach (var package in packages.Packages.Select(package => !Directory.Exists(Path.Combine(Directories.NuGetDirectory, string.Format("{0}.{1}", package.Id, package.Version)))))
            //{
            //    using (var client = new WebClient())
            //    {
            //        client.DownloadFileAsync(new Uri(string.Format("https://www.nuget.org/api/v2/package/{0}/{1}", package.Id, package.Version)), Directories.NuGetDirectory);
            //        client.DownloadFileCompleted += OnNuGetDownloadComplete;
            //    }
            //}
        }

        private static void OnNuGetDownloadComplete(object sender, AsyncCompletedEventArgs args)
        {
            if (args.Cancelled || args.Error != null)
            {
                Utility.Log(LogLevel.Warning, "NuGet packet Download faild.");
                return;
            }
        }

        [XmlType(AnonymousType = true)]
        [XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "packages")]
        private class NuGetPackages
        {
            [XmlElementAttribute("package")]
            public NuGetPackage[] Packages { get; set; }
        }

        [XmlTypeAttribute(AnonymousType = true, TypeName = "package")]
        private class NuGetPackage
        {
            [XmlElement("id")]
            public string Id { get; set; }

            [XmlElement("version")]
            public string Version { get; set; }

            [XmlElement("targetFramework")]
            public string TargetFramework { get; set; }
        }
    }
}
