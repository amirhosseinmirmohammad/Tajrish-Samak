using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TajrishSamak.Web.Services;

namespace TajrishSamak.Web.Services
{
    /// <summary>
    /// Route constraint for the geo landing pages: only accepts slug pairs that
    /// exist in the GeoCatalog, so the catch-all "{areaSlug}/{serviceSlug}" route
    /// no longer swallows real app routes like /admin/login.
    /// </summary>
    public class GeoLandingConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey,
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.UrlGeneration)
            {
                return true;
            }

            var area = values["areaSlug"]?.ToString();
            var service = values["serviceSlug"]?.ToString();

            if (string.IsNullOrWhiteSpace(area) || string.IsNullOrWhiteSpace(service))
            {
                return false;
            }

            var areaOk = false;
            foreach (var location in GeoCatalog.LocationList)
            {
                if (location.Slug.Equals(area, StringComparison.OrdinalIgnoreCase)) { areaOk = true; break; }
            }
            if (!areaOk) { return false; }

            foreach (var s in GeoCatalog.ServiceList)
            {
                if (s.Slug.Equals(service, StringComparison.OrdinalIgnoreCase)) { return true; }
            }
            return false;
        }
    }
}
