using System.IO;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using LeagueSharp.Loader.Model.Settings;

namespace LeagueSharp.Loader.Model.Log
{
    internal static class Logs
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string MainLogFile = Path.Combine(Directories.LogsDirectory, "Main.log");

        public static void Initialize()
        {
            var hierarchy = (Hierarchy) LogManager.GetRepository();

            var patternLayout = new PatternLayout
            {
                ConversionPattern =
                    "%date | %-5level | %logger{1}.%method:%line | %message%newline%exception"
            };
            patternLayout.ActivateOptions();

            var file = new RollingFileAppender
            {
                AppendToFile = false,
                File = MainLogFile,
                Layout = patternLayout,
                MaxSizeRollBackups = 10,
                RollingStyle = RollingFileAppender.RollingMode.Once,
                StaticLogFileName = true
            };
            file.ActivateOptions();
            hierarchy.Root.AddAppender(file);

            var console = new DebugAppender {Layout = patternLayout};
            console.ActivateOptions();
            hierarchy.Root.AddAppender(console);

            var user = new UserLog {Layout = patternLayout};
            user.ActivateOptions();
            hierarchy.Root.AddAppender(user);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;

            Log.Info(MainLogFile);
        }
    }
}