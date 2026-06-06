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
                name: "SiteSearchV86",
                url: "search",
                defaults: new { controller = "Search", action = "Index" }
            );

            // GEO Landing Pages v84
            // Routeهای ثابت قبلی حفظ شده‌اند؛ Route دینامیک v84 همه محله‌ها و خدمت‌های جدید را می‌گیرد.
            // این Route حتماً باید قبل از Blog/Service/Brand/Default باشد.

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
                name: "GeoDynamicAreaServiceV84",
                url: "{areaSlug}/{serviceSlug}",
                defaults: new { controller = "Geo", action = "ByAreaService" },
                constraints: new
                {
                    areaSlug = "tehran|tajrish|shemiran|shemiranat|mantaghe-1|north-tehran|northeast-tehran|near-me|mahale-ma|darabad|kashanak|jamaran|dezashib|yaser|farmanieh|jamalabad|azgol|sohanak|artesh|aghdasiyeh|heravi|ghaem|shahrak-naft|shariati|hekmat|gholhak|dowlat|yakhchal|valiasr|asef|pessian|moghadas-ardabili|maghsoudbeik|darband|elahiyeh|fereshteh|amanieh|chamran|parkway|velenjak|vanak|seoul|jafarabad|sadabad|chaharrah-hesabi|ajodanieh|oshan|mahak|mahalati|sadr|bouali|qanat-kosar|emamzadeh-ghasem|abk|meydan-ghods|sahebqaranieh|falahi|zaferanieh|bagh-shater|ghoba|jolfa|dibaji|hosseinabad|langari|saghdoush|nobonyad|saeedi|araj|mahmoodieh|tandis|palladium|kamranieh|andarzgoo|manzarieh|nakhjavan|bookan|shahrak-omid|mini-city|kolahdooz|chizar|qeytarieh|lavasani|moosivand|pol-roumi|valiasr-sadr|takhti|zahir-dowleh|pol-tajrish|bagh-ferdos|emamzadeh-saleh|bazar-tajrish|zarabkhaneh|yekta|kashanchi|golsang|afshar|mojdeh|moghaddasi|lavasan|roudehen|boomehen|feshm|niavaran|pasdaran|ekhtiyariyeh|darrous",
                    serviceSlug = "best-audiology-clinic|best-hearing-clinic|best-hearing-aid-clinic|hearing-test|audiology-clinic|hearing-aid|hearing-aid-adjustment|home-visit-hearing-aid|in-home-hearing-aid-prescription|in-home-hearing-aid-adjustment|hearing-aid-repair|hearing-aid-battery|hearing-aid-filter|hearing-aid-price|hearing-aid-price-1405|installment-hearing-aid|insurance-hearing-aid|hearing-aid-insurance-tariff|earmold|waterproof-earmold|silicone-earmold|earmold-replacement|hearing-test-price|audiometry-price|hearing-aid-cost|rechargeable-hearing-aid|battery-free-hearing-aid|hearing-aid-battery-consumption|invisible-hearing-aid|tinnitus-hearing-aid|hearing-aid-warranty|hearing-aid-cleaning|hearing-aid-durability|hearing-aid-tube|hearing-aid-hook|waterproof-hearing-aid|sweatproof-hearing-aid|german-hearing-aid|american-hearing-aid|danish-hearing-aid|swiss-hearing-aid|iranian-hearing-aid|government-hearing-aid|free-behzisti-hearing-aid|deaf-support|hearing-aid-consultation|quality-hearing-aid|signia-hearing-aid-price|siemens-hearing-aid-price|widex-hearing-aid-price|phonak-hearing-aid-price"
                }
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

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
