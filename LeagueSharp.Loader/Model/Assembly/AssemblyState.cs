namespace LeagueSharp.Loader.Model.Assembly
{
    internal enum AssemblyState
    {
        Unknown,
        Ready,
        Injected,
        Queue,
        Downloading,
        DownloadingError,
        Compiling,
        CompilingError
    }
}