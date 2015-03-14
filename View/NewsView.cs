using System.Windows;
using MahApps.Metro;

namespace LeagueSharp.Loader.View
{
    /// <summary>
    ///     Description for NewsView.
    /// </summary>
    public partial class NewsView
    {
        /// <summary>
        ///     Initializes a new instance of the NewsView class.
        /// </summary>
        public NewsView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent("Red");
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
        }
    }
}