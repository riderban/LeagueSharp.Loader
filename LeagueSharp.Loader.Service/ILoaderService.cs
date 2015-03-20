using System.Collections.Generic;
using System.ServiceModel;

namespace LeagueSharp.Loader.Service
{
    [ServiceContract]
    public interface ILoaderService
    {
        [OperationContract]
        List<LSharpAssembly> GetAssemblyPathList(int pid);

        [OperationContract]
        LoginCredentials GetLoginCredentials(int pid);

        [OperationContract]
        Configuration GetConfiguration(int pid);

        [OperationContract]
        void Recompile(int pid);
    }
}