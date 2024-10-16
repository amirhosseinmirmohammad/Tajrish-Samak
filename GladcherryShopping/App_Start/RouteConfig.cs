using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GladcherryShopping
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
    name: "BlogsList",
    url: "Blog/All",
    defaults: new { controller = "Blog", action = "All", id = UrlParameter.Optional }
);

            routes.MapRoute(
name: "BlogsSubmitComment",
url: "Blog/SubmitComment",
defaults: new { controller = "Blog", action = "SubmitComment" }
);

            routes.MapRoute(
                name: "Blog",
                url: "Blog/{id}/{sefUrl}",
                defaults: new { controller = "Blog", action = "Index", id = UrlParameter.Optional, sefUrl = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "service",
                url: "service/{id}/{sefUrl}",
                defaults: new { controller = "service", action = "Index", id = UrlParameter.Optional, sefUrl = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
