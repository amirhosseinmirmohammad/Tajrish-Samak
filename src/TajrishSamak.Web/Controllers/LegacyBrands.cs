namespace TajrishSamak.Web.Controllers
{
    /// <summary>
    /// Brand landing slugs - ported 1:1 from the legacy BrandController list so
    /// /hearing-aid-brands/{slug} URLs keep working and the sitemap stays in sync.
    /// </summary>
    public static class LegacyBrands
    {
        public static readonly string[] All =
        {
            "widex", "signia", "oticon", "phonak", "resound", "starkey", "bernafon",
            "unitron", "hansaton", "beltone", "audifon", "sonic", "interton", "rion", "coselgi"
        };
    }
}
