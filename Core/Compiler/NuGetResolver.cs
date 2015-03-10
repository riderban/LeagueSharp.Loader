using System;
using System.IO;
using System.IO.Compression;
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
            try
            {
                var serializer = new XmlSerializer(typeof (packages));
                var packages = (packages) serializer.Deserialize(new FileStream(config, FileMode.Open));

                foreach (var package in packages.package)
                {
                    if (
                        !Directory.Exists(Path.Combine(Directories.NuGetDirectory,
                            string.Format("{0}.{1}", package.id, package.version))))
                    {
                        ProcessPackage(package);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning, "NuGet Resolve failed. " + e);
            }
        }

        private static void ProcessPackage(packagesPackage package)
        {
            var packageDir = Path.Combine(Directories.NuGetDirectory,
                string.Format("{0}.{1}", package.id, package.version));
            var packageZip = Path.Combine(packageDir, "package.zip");
            var packageUri =
                new Uri(string.Format("https://www.nuget.org/api/v2/package/{0}/{1}", package.id, package.version));

            try
            {
                Directory.CreateDirectory(packageDir);

                using (var client = new WebClient())
                {
                    client.DownloadFile(packageUri, packageZip);
                }
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning,
                    string.Format("NuGet Download failed. | {0} | {1} | {2}", packageUri, packageZip, e));
            }

            try
            {
                ZipFile.ExtractToDirectory(packageZip, packageDir);
                File.Delete(packageZip);
            }
            catch (Exception e)
            {
                Utility.Log(LogLevel.Warning,
                    string.Format("NuGet Extraction failed. | {0} | {1} | {2}", packageUri, packageZip, e));
            }
        }
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class packages
    {
        [XmlElement("package")]
        public packagesPackage[] package { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class packagesPackage
    {
        [XmlAttribute]
        public string id { get; set; }

        [XmlAttribute]
        public string targetFramework { get; set; }

        [XmlAttribute]
        public string version { get; set; }
    }
}