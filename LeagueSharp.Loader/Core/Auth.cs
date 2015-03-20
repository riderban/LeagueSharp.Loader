using System;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;

namespace LeagueSharp.Loader.Core
{
    internal class Auth
    {
        public const string AuthServer = "loader.joduska.me";
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static bool Authed { get; set; }

        public static Tuple<bool, string> Login(string user, string hash)
        {
            if (user == null || hash == null)
            {
                return new Tuple<bool, string>(false, Utility.GetMultiLanguageText("AuthEmpty"));
            }
            try
            {
                var data = "p=" + hash;
                var dataBytes = Encoding.UTF8.GetBytes(data);
                var wr = WebRequest.Create("https://" + AuthServer + "/login/" + WebUtility.UrlEncode(user));
                wr.Timeout = 2000;
                wr.ContentLength = dataBytes.Length;
                wr.Method = WebRequestMethods.Http.Post;
                wr.ContentType = "application/x-www-form-urlencoded";

                try
                {
                    var dataStream = wr.GetRequestStream();
                    dataStream.Write(dataBytes, 0, dataBytes.Length);
                    dataStream.Close();
                    wr.GetResponse();
                }
                catch (WebException ex)
                {
                    if ((int) ((HttpWebResponse) ex.Response).StatusCode == 403)
                    {
                        return new Tuple<bool, string>(false,
                            string.Format(Utility.GetMultiLanguageText("WrongAuth"), "www.joduska.me"));
                    }
                }

                return new Tuple<bool, string>(true, "Success");
            }
            catch (Exception)
            {
                return new Tuple<bool, string>(true, "Fallback T_T");
            }
        }

        private static string IPB_Clean_Password(string pass)
        {
            pass = pass.Replace("\xC3\x8A", "");
            pass = pass.Replace("&", "&amp;");
            pass = pass.Replace("\\", "&#092;");
            pass = pass.Replace("!", "&#33;");
            pass = pass.Replace("$", "&#036;");
            pass = pass.Replace("\"", "&quot;");
            pass = pass.Replace("\"", "&quot;");
            pass = pass.Replace("<", "&lt;");
            pass = pass.Replace(">", "&gt;");
            pass = pass.Replace("'", "&#39;");
            return pass;
        }

        public static string Hash(string input)
        {
            return Utility.Md5Hash(IPB_Clean_Password(input));
        }
    }
}