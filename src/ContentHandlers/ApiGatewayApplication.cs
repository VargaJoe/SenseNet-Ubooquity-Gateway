using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using SenseNet.ApplicationModel;
using SenseNet.ContentRepository.Schema;
using SenseNet.ContentRepository.Storage;

namespace SenseNet.Ubooquity.ContentHandlers
{
    [ContentHandler]
    public class ApiGatewayApplication : Application, IHttpHandler
    {
        public ApiGatewayApplication(Node parent) : this(parent, null) { }
        public ApiGatewayApplication(Node parent, string nodeTypeName) : base(parent, nodeTypeName) { }
        protected ApiGatewayApplication(NodeToken nt) : base(nt) { }

        //****************************************** Start of Repository Properties *********************************************//
        [RepositoryProperty("CustomUrl")]
        public string CustomUrl
        {
            get { return base.GetProperty<string>("CustomUrl"); }
            set { base.SetProperty("CustomUrl", value); }
        }

        //****************************************** Start of Overrides *********************************************//
        public override object GetProperty(string name)
        {
            switch (name)
            {
                case "CustomUrl":
                    return this.CustomUrl;
                default:
                    break;
            }
            return base.GetProperty(name);
        }

        public override void SetProperty(string name, object value)
        {
            switch (name)
            {
                case "CustomUrl":
                    this.CustomUrl = value.ToString();
                    break;
                default:
                    base.SetProperty(name, value);
                    break;
            }
        }

        public override string Icon { get; set; }

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
            string queryPath = ApiGateway.GetUrlParam("path", "/opds-books");

            var byteArray = ApiGateway.RequestFromUbooquity(serviceUrl, queryPath, context);

            // Technical Debt: link should be processed to use url params, it's not working yet
            using (var ms = new MemoryStream(byteArray))
            {
                XPathDocument document = new XPathDocument(ms);
                XPathNavigator navigator = document.CreateNavigator();
                XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
                manager.AddNamespace("dcterms", "http://purl.org/dc/terms/");
                manager.AddNamespace("pse", "http://vaemendis.net/opds-pse/ns");
                manager.AddNamespace("opds", "http://opds-spec.org/2010/catalog");
                manager.AddNamespace("opensearch", "http://a9.com/-/spec/opensearch/1.1/");
                foreach (XPathNavigator nav in navigator.Select("//link", manager))
                {
                    nav.SetValue("?path=" + nav.Value);
                }
            }

            context.Response.BinaryWrite(byteArray);
            context.Response.End();
        }

    }
}