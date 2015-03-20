using System.Collections.Generic;
using System.ServiceModel;

namespace LeagueSharp.Loader.Service
{
    [ServiceContract(Name = "http://joduska.me/v1/LoaderService")]
    public interface ILoaderService
    {
        [OperationContract]
        List<string> GetAssemblyPathList(int pid);

        [OperationContract]
        LoginCredentials GetLoginCredentials(int pid);

        [OperationContract]
        Configuration GetConfiguration(int pid);

        [OperationContract]
        void Recompile(int pid);
    }
}