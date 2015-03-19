using System;
using System.Net;
using System.Reflection;
using log4net;

namespace LeagueSharp.Loader.Core
{
    internal class GZipWebClient : WebClient
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            try
            {
                var request = (HttpWebRequest) base.GetWebRequest(address);
                request.Timeout = Timeout;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return request;
            }
            catch (Exception e)
            {
                Log.Warn(e);
                return null;
            }
        }
    }
}