using SenseNet.ContentRepository;
using SenseNet.Logging.ApplicationInsights;
using SenseNet.Tools;
using SenseNet.Ubooquity.Routing;
using System;
using System.Web;
using System.Web.Routing;

namespace SenseNet.Portal
{
    public class Global : SenseNet.Portal.SenseNetGlobal
    {
        protected override void Application_Start(object sender, EventArgs e, HttpApplication application)
        {
            base.Application_Start(sender, e, application);
        }

        protected override void BuildRepository(IRepositoryBuilder repositoryBuilder)
        {
            base.BuildRepository(repositoryBuilder);

            repositoryBuilder.UseLogger(new ApplicationInsightsLogger());
        }

        protected override void RegisterRoutes(RouteCollection routes, HttpApplication application)
        {
            base.RegisterRoutes(routes, application);

            routes.Add("SnOpdsBooksRoute", new Route("opds-books/{*path}", new OpdsRouteHandler()));
            routes.Add("SnOpdsComicsRoute", new Route("opds-comics/{*path}", new OpdsRouteHandler()));
        }

    }
}