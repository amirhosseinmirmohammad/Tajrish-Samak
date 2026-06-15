using GladcherryShopping;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HubSIS
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
                = ReferenceLoopHandling.Ignore;

            GlobalConfiguration.Configuration.Formatters.Remove(
                GlobalConfiguration.Configuration.Formatters.XmlFormatter
            );

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            Server.ClearError();

            var httpException = exception as HttpException;

            int statusCode = 500;

            if (httpException != null)
            {
                statusCode = httpException.GetHttpCode();
            }

            if (statusCode == 404)
            {
                Response.Clear();
                Response.StatusCode = 404;
                Response.TrySkipIisCustomErrors = true;

                Response.Redirect("~/Error/NotFound");
                return;
            }

            Response.Clear();
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;

            Response.Write("Internal Server Error");
        }
    }
}