namespace TajrishSamak.Web.Controllers
{
    /// <summary>
    /// Brand landing definitions - ported 1:1 from the legacy BrandController list so
    /// /hearing-aid-brands/{slug} URLs keep working, product search terms stay
    /// identical and the sitemap stays in sync.
    /// </summary>
    public static class LegacyBrands
    {
        public class BrandLandingInfo
        {
            public string Slug { get; set; }
            public string PersianName { get; set; }
            public string EnglishName { get; set; }
            public string[] SearchTerms { get; set; }
        }

        public static readonly BrandLandingInfo[] Brands =
        {
            new() { Slug = "widex", PersianName = "ویدکس", EnglishName = "Widex", SearchTerms = new[] { "ویدکس", "Widex", "WIDEX", "وی دکس" } },
            new() { Slug = "signia", PersianName = "سیگنیا", EnglishName = "Signia", SearchTerms = new[] { "سیگنیا", "Signia", "SIGNIA", "زیگنیا", "زیمنس" } },
            new() { Slug = "oticon", PersianName = "اتیکن", EnglishName = "Oticon", SearchTerms = new[] { "اتیکن", "Oticon", "OTICON", "اوتیکن" } },
            new() { Slug = "phonak", PersianName = "فوناک", EnglishName = "Phonak", SearchTerms = new[] { "فوناک", "Phonak", "PHONAK", "فونک" } },
            new() { Slug = "resound", PersianName = "ریساند", EnglishName = "ReSound", SearchTerms = new[] { "ریساند", "Resound", "ReSound", "RESOUND", "ری ساوند" } },
            new() { Slug = "starkey", PersianName = "استارکی", EnglishName = "Starkey", SearchTerms = new[] { "استارکی", "Starkey", "STARKEY" } },
            new() { Slug = "bernafon", PersianName = "برنافن", EnglishName = "Bernafon", SearchTerms = new[] { "برنافن", "Bernafon", "BERNAFON" } },
            new() { Slug = "unitron", PersianName = "یونیترون", EnglishName = "Unitron", SearchTerms = new[] { "یونیترون", "Unitron", "UNITRON" } },
            new() { Slug = "hansaton", PersianName = "هنساتون", EnglishName = "Hansaton", SearchTerms = new[] { "هنساتون", "Hansaton", "HANSATON" } },
            new() { Slug = "beltone", PersianName = "بلتون", EnglishName = "Beltone", SearchTerms = new[] { "بلتون", "Beltone", "BELTONE" } },
            new() { Slug = "audifon", PersianName = "ادیفون", EnglishName = "Audifon", SearchTerms = new[] { "ادیفون", "Audifon", "AUDIFON" } },
            new() { Slug = "sonic", PersianName = "سونیک", EnglishName = "Sonic", SearchTerms = new[] { "سونیک", "Sonic", "SONIC" } },
            new() { Slug = "interton", PersianName = "اینترتون", EnglishName = "Interton", SearchTerms = new[] { "اینترتون", "Interton", "INTERTON" } },
            new() { Slug = "rion", PersianName = "ریون", EnglishName = "Rion", SearchTerms = new[] { "ریون", "Rion", "RION" } },
            new() { Slug = "coselgi", PersianName = "کوسلجی", EnglishName = "Coselgi", SearchTerms = new[] { "کوسلجی", "Coselgi", "COSELGI" } }
        };

        /// <summary>Slugs only - kept for the sitemap endpoint and existing callers.</summary>
        public static readonly string[] All = Brands.Select(b => b.Slug).ToArray();
    }
}
