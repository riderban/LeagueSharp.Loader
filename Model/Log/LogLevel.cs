namespace LeagueSharp.Loader.Model.Log
{
    /// <summary>
    ///     ALL > TRACE > DEBUG > INFO > WARN > ERROR > FATAL > OFF
    /// </summary>
    internal enum LogLevel
    {
        /// <summary>
        ///     All information.
        /// </summary>
        All = 7,

        /// <summary>
        ///     Most detailed information. Expect these to be written to logs only.
        /// </summary>
        Trace = 6,

        /// <summary>
        ///     Detailed information on the flow through the system. Expect these to be written to logs only.
        /// </summary>
        Debug = 5,

        /// <summary>
        ///     Interesting runtime events (startup/shutdown). Expect these to be immediately visible on a console, so be
        ///     conservative and keep to a minimum.
        /// </summary>
        Info = 4,

        /// <summary>
        ///     Use of deprecated APIs, poor use of API, 'almost' errors, other runtime situations that are undesirable or
        ///     unexpected, but not necessarily "wrong". Expect these to be immediately visible on a status console.
        /// </summary>
        Warn = 3,

        /// <summary>
        ///     Other runtime errors or unexpected conditions. Expect these to be immediately visible on a status console.
        /// </summary>
        Error = 2,

        /// <summary>
        ///     Severe errors that cause premature termination. Expect these to be immediately visible on a status console.
        /// </summary>
        Fatal = 1,

        /// <summary>
        ///     The highest possible rank and is intended to turn off logging.
        /// </summary>
        Off = 0
    }
}