using System.Web.Mvc;
using System.Web.Routing;

namespace GladcherryShopping
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // GEO Landing Pages
            // نکته مهم: همه Routeهای ثابت GEO باید قبل از Blog/Service/Default باشند.
            // اگر بعد از Default باشند، هیچ‌وقت اجرا نمی‌شوند.

            routes.MapRoute(
                name: "GeoHearingTestTehran",
                url: "tehran/hearing-test",
                defaults: new { controller = "Geo", action = "HearingTestTehran" }
            );

            routes.MapRoute(
                name: "GeoHearingTestTajrish",
                url: "tajrish/hearing-test",
                defaults: new { controller = "Geo", action = "HearingTestTajrish" }
            );

            routes.MapRoute(
                name: "GeoHearingClinicTehran",
                url: "tehran/hearing-clinic",
                defaults: new { controller = "Geo", action = "HearingClinicTehran" }
            );

            routes.MapRoute(
                name: "GeoHearingClinicTajrish",
                url: "tajrish/audiology-clinic",
                defaults: new { controller = "Geo", action = "HearingClinicTajrish" }
            );

            routes.MapRoute(
                name: "GeoHearingAidTehran",
                url: "tehran/hearing-aid",
                defaults: new { controller = "Geo", action = "HearingAidTehran" }
            );

            routes.MapRoute(
                name: "GeoHearingAidAdjustmentTehran",
                url: "tehran/hearing-aid-adjustment",
                defaults: new { controller = "Geo", action = "HearingAidAdjustmentTehran" }
            );

            routes.MapRoute(
                name: "GeoHearingAidTajrish",
                url: "tajrish/hearing-aid",
                defaults: new { controller = "Geo", action = "HearingAidTajrish" }
            );

            routes.MapRoute(
                name: "GeoHearingAidAdjustmentTajrish",
                url: "tajrish/hearing-aid-adjustment",
                defaults: new { controller = "Geo", action = "HearingAidAdjustmentTajrish" }
            );

            routes.MapRoute(
                name: "GeoHearingTestNiavaran",
                url: "niavaran/hearing-test",
                defaults: new { controller = "Geo", action = "HearingTestNiavaran" }
            );

            routes.MapRoute(
                name: "GeoHearingTestQeytarieh",
                url: "qeytarieh/hearing-test",
                defaults: new { controller = "Geo", action = "HearingTestQeytarieh" }
            );

            routes.MapRoute(
                name: "GeoHearingTestZaferanieh",
                url: "zaferanieh/hearing-test",
                defaults: new { controller = "Geo", action = "HearingTestZaferanieh" }
            );

            routes.MapRoute(
                name: "GeoHearingTestElahiyeh",
                url: "elahiyeh/hearing-test",
                defaults: new { controller = "Geo", action = "HearingTestElahiyeh" }
            );

            routes.MapRoute(
                name: "GeoHearingTestFarmanieh",
                url: "farmanieh/hearing-test",
                defaults: new { controller = "Geo", action = "HearingTestFarmanieh" }
            );

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
               name: "HearingAidBrandsIndex",
               url: "hearing-aid-brands",
               defaults: new { controller = "Brand", action = "Index" }
           );

            routes.MapRoute(
                name: "HearingAidBrandLanding",
                url: "hearing-aid-brands/{brand}",
                defaults: new { controller = "Brand", action = "Index", brand = UrlParameter.Optional }
            );


            // Default route همیشه باید آخر باشد.
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
