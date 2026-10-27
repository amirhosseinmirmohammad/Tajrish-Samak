using System.Web.Mvc;
using System.Web.Routing;

namespace GladcherryShopping
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // =========================
            // 0. SITEMAP (DYNAMIC XML)
            // NOTE: delete the old static sitemap.xml file on the server when
            // deploying, otherwise IIS serves the stale file (it contains
            // hundreds of URLs that return 404 -> GSC "Submitted URL not found").
            // =========================
            routes.MapRoute(
                name: "SitemapXml",
                url: "sitemap.xml",
                defaults: new { controller = "Sitemap", action = "Index" }
            );

            // =========================
            // 1. SEARCH (SPECIFIC FIRST)
            // =========================
            routes.MapRoute(
                name: "SiteSearchV86",
                url: "search",
                defaults: new { controller = "Search", action = "Index" }
            );

            // =========================
            // 2. BLOG ROUTES
            // =========================
            routes.MapRoute(
                name: "BlogsList",
                url: "Blog/All",
                defaults: new { controller = "Blog", action = "All" }
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

            // =========================
            // SERVICE ALL (FIX)
            // =========================
            routes.MapRoute(
                name: "ServiceAll",
                url: "service/all",
                defaults: new { controller = "Service", action = "All" }
            );

            // =========================
            // 3. SERVICE ROUTES
            // =========================
            routes.MapRoute(
                name: "Service",
                url: "service/{id}/{sefUrl}",
                defaults: new { controller = "Service", action = "Index", id = UrlParameter.Optional, sefUrl = UrlParameter.Optional }
            );

            // =========================
            // 4. BRAND ROUTES
            // =========================
            routes.MapRoute(
                name: "HearingAidBrandsIndex",
                url: "hearing-aid-brands",
                defaults: new { controller = "Brand", action = "Index" }
            );

            routes.MapRoute(
                name: "HearingAidBrandLanding",
                url: "hearing-aid-brands/{brand}",
                defaults: new { controller = "Brand", action = "Index", brand = UrlParameter.Optional }
            );

            // =========================
            // 5. GEO STATIC ROUTES (SAFE)
            // =========================
            routes.MapRoute(
                name: "GeoHearingTestTajrish",
                url: "tajrish/hearing-test",
                defaults: new { controller = "Geo", action = "HearingTestTajrish" }
            );

            routes.MapRoute(
                name: "GeoHearingAidTajrish",
                url: "tajrish/hearing-aid",
                defaults: new { controller = "Geo", action = "HearingAidTajrish" }
            );

            routes.MapRoute(
                name: "GeoHearingClinicTajrish",
                url: "tajrish/audiology-clinic",
                defaults: new { controller = "Geo", action = "HearingClinicTajrish" }
            );

            routes.MapRoute(
                name: "GeoHearingTestTehran",
                url: "tehran/hearing-test",
                defaults: new { controller = "Geo", action = "HearingTestTehran" }
            );

            routes.MapRoute(
                name: "GeoHearingAidTehran",
                url: "tehran/hearing-aid",
                defaults: new { controller = "Geo", action = "HearingAidTehran" }
            );

            // =========================
            // 6. 🔥 GEO DYNAMIC ROUTE (constraints generated from GeoController's
            // slug lists so every URL listed in sitemap.xml is routable.
            // Previously the hard-coded regex only accepted 14 areas and 8
            // services while the sitemap advertised all 106 x 51 combos,
            // which made Google report "Submitted URL not found (404)".)
            // =========================
            routes.MapRoute(
                name: "GeoDynamicAreaServiceV84",
                url: "{areaSlug}/{serviceSlug}",
                defaults: new { controller = "Geo", action = "ByAreaService" },
                constraints: new
                {
                    areaSlug = GladcherryShopping.Controllers.GeoController.AreaPattern,

                    serviceSlug = GladcherryShopping.Controllers.GeoController.ServicePattern
                }
            );

            // =========================
            // 7. DEFAULT (LAST ALWAYS)
            // =========================
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}