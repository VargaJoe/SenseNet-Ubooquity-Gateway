using System.Web;
using System.Web.Routing;
using SenseNet.Services.Wopi;
using SenseNet.Ubooquity.ContentHandlers;

namespace SenseNet.Ubooquity.Routing
{
    public class OpdsRouteHandler : IRouteHandler
    {
        // To enable this router, override RegisterRoutes with global.asax to add these route:
        //    routes.Add("SnOpdsRoute", new Route("opds-books/{*path}", new OpdsRouteHandler()));
        //    routes.Add("SnOpdsRoute", new Route("opds-comics/{*path}", new OpdsRouteHandler()));

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ApiGateway();
        }
    }
}
