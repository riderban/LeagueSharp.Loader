namespace LeagueSharp.Loader.Model.Assembly
{
    public enum AssemblyState
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