using System;
using System.Net;
using System.Web;
using SenseNet.ContentRepository.Schema;
using System.Linq;
using System.Text.RegularExpressions;

namespace SenseNet.Ubooquity.ContentHandlers
{
    [ContentHandler]
    public class ApiGateway : IHttpHandler
    {
        public ApiGateway() { }


        //****************************************** Start of Helpers *********************************************//
        public static string GetUrlParam(string getparam, string defvalue)
        {
            string urlParameter = defvalue;
            if (HttpContext.Current.Request.Params[getparam] != null)
            {
                urlParameter = HttpContext.Current.Request.Params[getparam];
            }
            return urlParameter;
        }

        public static string GetAdditionalUrlParams()
        {
            
            string urlParameter = string.Empty;
            var allparams = HttpContext.Current.Request.QueryString;

            // get url parameters without path that will be used to create request path
            var filteredParams = allparams.Keys.OfType<string>().Where(p => p.ToLower() != "path");

            var filteredLength = filteredParams.Count();
            if (filteredLength > 0)
            {
                var separator = "?";
                foreach (var uparam in filteredParams)
                {
                    urlParameter += $"{separator}{uparam}={allparams[uparam]}";
                    separator = "&";
                }               
            }

            return urlParameter;
        }

        public static Byte[] RequestFromUbooquity(string serviceUrl, string queryPath, HttpContext context)
        {
            // GET parameters
            string paramsByGet = GetAdditionalUrlParams();

            // separator if needed
            string separator = (serviceUrl[serviceUrl.Length - 1] != '/' && queryPath.Length > 0 && queryPath[0] != '/') ? "/" : "";

            // create request url
            string callUrl = $"{serviceUrl}{separator}{queryPath}{paramsByGet}";

            Byte[] byteArray;
            using (var client = new WebClient())
            {
                Regex regex = new Regex("((.+)://)((.*)@)*(.*)");
                MatchCollection macthes = regex.Matches(serviceUrl);
                string stripedUrl = macthes[0].Groups[1].ToString() + macthes[0].Groups[5].ToString();
                string[] kredenc = macthes[0].Groups[4].ToString().Split(':');
                string userName = kredenc[0];
                string pass = (kredenc.Length > 1) ? kredenc[1] : string.Empty;

                if (!string.IsNullOrWhiteSpace(userName))
                {
                    client.UseDefaultCredentials = true;
                    NetworkCredential networkCredential = new NetworkCredential(userName, pass);
                    client.Credentials = networkCredential;
                }

                byteArray = client.DownloadData(callUrl);
                WebHeaderCollection clientHeaders = client.ResponseHeaders;
                for (int i = 0; i < clientHeaders.Count; i++)
                {
                    if (clientHeaders.GetKey(i).ToLower() != "server")
                    {
                        context.Response.AddHeader(clientHeaders.GetKey(i), clientHeaders.Get(i));
                    }
                }

                return byteArray;
            }
        }

        //****************************************** Start of IHttpHandler *********************************************//
        public bool IsReusable
        {
            get { throw new NotImplementedException(); }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ClearHeaders();

            // base url 
            //string serviceUrl = "http://username:password@localhost:2202"; //settings
            string serviceUrl = "http://localhost:2202"; //settings
            string queryPath = HttpContext.Current.Request.Path; 

            var byteArray = RequestFromUbooquity(serviceUrl, queryPath, context);

            context.Response.BinaryWrite(byteArray);
            context.Response.End();
        }

    }
}