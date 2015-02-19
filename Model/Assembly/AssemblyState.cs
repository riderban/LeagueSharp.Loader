namespace LeagueSharp.Loader.Model.Assembly
{
    internal enum AssemblyState
    {
        Unknown,
        Ready,
        Injected,
        Queue,
        Downloading,
        Compiling,
        CompilingError
    }
}