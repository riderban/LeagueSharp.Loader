using System;
using System.ServiceModel;

namespace LeagueSharp.Loader.Service
{
    public static class ServiceFactory
    {
        public static TInterfaceType GetInterface<TInterfaceType>() where TInterfaceType : class
        {
            try
            {
                return
                    new ChannelFactory<TInterfaceType>(new NetNamedPipeBinding(),
                        new EndpointAddress("net.pipe://localhost/" + typeof (TInterfaceType).Name)).CreateChannel();
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Failed to connect to assembly pipe for communication. The targetted assembly may not be loaded yet. Desired interface: " +
                    typeof (TInterfaceType).Name, e);
            }
        }

        public static ServiceHost ShareInterface<TImplementationType>(bool open = true)
        {
            var host = new ServiceHost(typeof (TImplementationType), new Uri("net.pipe://localhost"));

            host.AddServiceEndpoint(typeof (TImplementationType).GetInterfaces()[0], new NetNamedPipeBinding(),
                typeof (TImplementationType).GetInterfaces()[0].Name);

            if (open)
            {
                host.Open();
            }

            return host;
        }
    }
}