using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using log4net.Appender;
using log4net.Core;

namespace LeagueSharp.Loader.Model.Log
{
    internal class UserLog : AppenderSkeleton
    {
        public static ObservableCollection<LogItem> Items { get; set; }

        static UserLog()
        {
            Items = new ObservableCollection<LogItem>();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            Items.Add(new LogItem
            {
                Level = loggingEvent.Level,
                Source = loggingEvent.LocationInformation.MethodName,
                Message = loggingEvent.RenderedMessage
            });

            if (loggingEvent.Level >= Level.Error) // TODO: change to Error after testing
            {
                // workaround to fix autoclose
                Task.Factory.StartNew(
                    () =>
                    {
                        MessageBox.Show(loggingEvent.RenderedMessage,
                            loggingEvent.Level + " in " + loggingEvent.LocationInformation.MethodName,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    })
                    .Wait();
            }
        }
    }
}