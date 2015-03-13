using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using LeagueSharp.Loader.Model.Assembly;
using LeagueSharp.Loader.Model.Log;
using LeagueSharp.Loader.Model.Settings;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Core
{
    internal static class Utility
    {
        public static string UserNameHash
        {
            get { return Environment.UserName.GetHashCode().ToString("X"); }
        }

        static Utility()
        {
            JsonConvert.DefaultSettings =
                () => new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
        }

        public static void Log(LogLevel level, string message, [CallerMemberName] string source = "")
        {
            Logs.Main.Items.Add(new LogItem {Level = level, Source = source, Message = message});
            Debug.WriteLine("LOG | {0} | {1} | {2}", level, source, message);

            if (level >= LogLevel.Warning) // TODO: change to Error after testing
            {
                // workaround to fix autoclose
                Task.Factory.StartNew(
                    () => { MessageBox.Show(message, source, MessageBoxButton.OK, MessageBoxImage.Error); }).Wait();
            }
        }

        public static void SaveToJson(object obj, string path)
        {
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(obj, Formatting.Indented));
            }
            catch (Exception e)
            {
                Log(LogLevel.Warning, string.Format("Failed to Save {0} to {1}\n{2}", obj.GetType(), path, e.Message));
            }
        }

        public static T LoadFromJson<T>(string path)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                Log(LogLevel.Warning, string.Format("Failed to Load {0} to {1}\n{2}", typeof (T), path, e.Message));
            }

            return default(T);
        }

        public static void SaveToXml<T>(object obj, string path)
        {
            try
            {
                using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    new XmlSerializer(typeof (T)).Serialize(sw, obj);
                }
            }
            catch (Exception e)
            {
                Log(LogLevel.Warning, string.Format("Failed to Save {0} to {1}\n{2}", typeof (T), path, e.Message));
            }
        }

        public static T LoadFromXml<T>(string path)
        {
            try
            {
                using (var reader = new StreamReader(path, Encoding.UTF8))
                {
                    return (T) new XmlSerializer(typeof (T)).Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Log(LogLevel.Warning, string.Format("Failed to Load {0} from {1}\n{2}", typeof (T), path, e.Message));
            }

            return default(T);
        }

        public static string ReadResourceString(string resource)
        {
            try
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log(LogLevel.Warning, string.Format("Failed to Read {0}", resource));
            }

            return string.Empty;
        }

        public static void CreateFileFromResource(string path, string resource, bool overwrite = false)
        {
            try
            {
                if (!overwrite && File.Exists(path))
                {
                    return;
                }

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    if (stream == null)
                    {
                        return;
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                        {
                            sw.Write(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log(LogLevel.Warning, string.Format("Failed to Create File {0} from {1}", path, resource));
            }
        }

        public static void ClearDirectory(string directory)
        {
            try
            {
                var dir = new DirectoryInfo(directory);
                foreach (var fi in dir.GetFiles())
                {
                    fi.Attributes = FileAttributes.Normal;
                    fi.Delete();
                }

                foreach (var di in dir.GetDirectories())
                {
                    di.Attributes = FileAttributes.Normal;
                    ClearDirectory(di.FullName);
                    di.Delete();
                }
            }
            catch
            {
                // ignored
            }
        }

        public static string MakeValidFileName(string name)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return Regex.Replace(name, invalidRegStr, "_");
        }

        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
        }

        public static bool OverwriteFile(string file, string path)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (dir != null)
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                try
                {
                    File.Move(file, path);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    throw;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RenameFileIfExists(string file, string path)
        {
            try
            {
                var counter = 1;
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileExtension = Path.GetExtension(file);
                var newPath = path;
                var pathDirectory = Path.GetDirectoryName(path);
                if (pathDirectory != null)
                {
                    if (!Directory.Exists(pathDirectory))
                    {
                        Directory.CreateDirectory(pathDirectory);
                    }

                    while (File.Exists(newPath))
                    {
                        var tmpFileName = string.Format("{0} ({1})", fileName, counter++);
                        newPath = Path.Combine(pathDirectory, tmpFileName + fileExtension);
                    }

                    File.Move(file, newPath);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        ///     Returns the md5 hash from a string.
        /// </summary>
        public static string Md5Hash(string s)
        {
            var sb = new StringBuilder();
            HashAlgorithm algorithm = MD5.Create();
            var h = algorithm.ComputeHash(Encoding.Default.GetBytes(s));

            foreach (var b in h)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string Md5Checksum(string filePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                }
            }
            catch
            {
                return "-1";
            }
        }

        public static string GetMultiLanguageText(string key)
        {
            var resource = Application.Current.FindResource(key);
            return resource != null ? resource.ToString() : "KEY:" + key;
        }

        public static void CopyDirectory(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }
            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        public static LeagueSharpAssembly CreateEmptyAssembly(string assemblyName)
        {
            try
            {
                var appconfig = ReadResourceString("LeagueSharp.Loader.Resources.DefaultProject.App.config");
                var assemblyInfocs = ReadResourceString("LeagueSharp.Loader.Resources.DefaultProject.AssemblyInfo.cs");
                var defaultProjectcsproj =
                    ReadResourceString("LeagueSharp.Loader.Resources.DefaultProject.DefaultProject.csproj");
                var programcs = ReadResourceString("LeagueSharp.Loader.Resources.DefaultProject.Program.cs");
                var targetPath = Path.Combine(Directories.LocalRepositoryDirectory,
                    assemblyName + Environment.TickCount.GetHashCode().ToString("X"));

                programcs = programcs.Replace("{ProjectName}", assemblyName);
                assemblyInfocs = assemblyInfocs.Replace("{ProjectName}", assemblyName);
                defaultProjectcsproj = defaultProjectcsproj.Replace("{ProjectName}", assemblyName);
                defaultProjectcsproj = defaultProjectcsproj.Replace("{SystemDirectory}", Directories.CoreDirectory);

                Directory.CreateDirectory(targetPath);
                File.WriteAllText(Path.Combine(targetPath, "App.config"), appconfig);
                File.WriteAllText(Path.Combine(targetPath, "AssemblyInfo.cs"), assemblyInfocs);
                File.WriteAllText(Path.Combine(targetPath, assemblyName + ".csproj"), defaultProjectcsproj);
                File.WriteAllText(Path.Combine(targetPath, "Program.cs"), programcs);

                return new LeagueSharpAssembly
                {
                    Name = assemblyName,
                    Project = Path.Combine(targetPath, assemblyName + ".csproj"),
                    Author = Config.Instance.Username,
                    Location = targetPath,
                    Type = AssemblyType.Unknown
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static string GetLatestLeagueOfLegendsExePath(string lastKnownPath)
        {
            try
            {
                var dir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(lastKnownPath), "..\\..\\"));
                if (Directory.Exists(dir))
                {
                    var versionPaths = Directory.GetDirectories(dir);
                    var greatestVersionString = "";
                    long greatestVersion = 0;

                    foreach (var versionPath in versionPaths)
                    {
                        Version version;
                        var isVersion = Version.TryParse(Path.GetFileName(versionPath), out version);
                        if (isVersion)
                        {
                            var test = version.Build*Math.Pow(600, 4) + version.Major*Math.Pow(600, 3) +
                                       version.Minor*Math.Pow(600, 2) + version.Revision*Math.Pow(600, 1);
                            if (test > greatestVersion)
                            {
                                greatestVersion = (long) test;
                                greatestVersionString = Path.GetFileName(versionPath);
                            }
                        }
                    }

                    if (greatestVersion != 0)
                    {
                        var exe = Directory.GetFiles(
                            Path.Combine(dir, greatestVersionString), "League of Legends.exe",
                            SearchOption.AllDirectories);
                        return exe.Length > 0 ? exe[0] : null;
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public static int VersionToInt(Version version)
        {
            return version.Major*10000000 + version.Minor*10000 + version.Build*100 + version.Revision;
        }
    }
}