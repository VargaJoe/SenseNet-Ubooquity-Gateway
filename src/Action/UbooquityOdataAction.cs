using SenseNet.ApplicationModel;
using SenseNet.ContentRepository;
using System;
using Newtonsoft.Json;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.IO;
using System.Xml;

namespace SenseNet.Ubooquity.OData
{
    public class UbooquityContentAction : UrlAction
    {
        //we do not support HTML behavior
        public override string Uri
        {
            get { return null; }
        }

        public override bool IsHtmlOperation { get { return false; } }
        public override bool IsODataOperation { get { return true; } }
        public override bool CausesStateChange { get { return false; } }

        public override object Execute(Content content, params object[] parameters)
        {
            string path = HttpContext.Current.Request.QueryString.Get("path")??"/opds-books";
            
            string type = HttpContext.Current.Request.QueryString.Get("type"); 
            string uboId = HttpContext.Current.Request.QueryString.Get("id"); 
            string urlParam = HttpContext.Current.Request.QueryString.Get("param");
            //string CustomUrl = @"http://username:password@localhost:2202/opds-books";
            string CustomUrl = @"http://localhost:2202";
            CustomUrl = CustomUrl + path;
            int Timeout = 3000;

            //Ubooquity logic start
            XPathDocument preResult = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(CustomUrl))
                {
                    Regex regex = new Regex("((.+)://)((.*)@)*(.*)");
                    MatchCollection macthes = regex.Matches(CustomUrl);
                    string stripedUrl = macthes[0].Groups[1].ToString() + macthes[0].Groups[5].ToString();
                    string[] kredenc = macthes[0].Groups[4].ToString().Split(':');
                    string userName = kredenc[0];
                    string pass = (kredenc.Length > 1) ? kredenc[1] : string.Empty;
                    WebRequest feedRequest = WebRequest.Create(HttpUtility.HtmlDecode(stripedUrl));
                    feedRequest.Timeout = Timeout; // this should come from settings
                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        feedRequest.PreAuthenticate = true;
                        NetworkCredential networkCredential = new NetworkCredential(userName, pass);
                        feedRequest.Credentials = networkCredential;
                    }

                    string sb = string.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)feedRequest.GetResponse())
                    {
                        using (Stream streamData = response.GetResponseStream())
                        {
                            preResult = new XPathDocument(streamData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string excMsg = e.Message;
                string inExcMsg = (e.InnerException != null) ? e.InnerException.Message : string.Empty;
                //document = getErrorDocument(excMsg, inExcMsg);
                //this.ErrorOccured = true;
            }
            //Ubooquity logic end

            XmlDocument result = new XmlDocument();
            result.Load(preResult.CreateNavigator().ReadSubtree());
            return JsonConvert.SerializeXmlNode(result);
        }
    }

    public class CopyResultObject
    {
        public string target;
        public string source;
        public string success;
        public string childpath;
    }
}