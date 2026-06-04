using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class BrandController : Controller
    {
        private const int BrandPageSize = 9;
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private static readonly List<BrandLandingInfo> Brands = new List<BrandLandingInfo>
        {
                new BrandLandingInfo
                {
                    Slug = "widex",
                    PersianName = "ویدکس",
                    EnglishName = "Widex",
                    SearchTerms = new[] { "ویدکس", "Widex", "WIDEX", "وی دکس" }
                },
                new BrandLandingInfo
                {
                    Slug = "signia",
                    PersianName = "سیگنیا",
                    EnglishName = "Signia",
                    SearchTerms = new[] { "سیگنیا", "Signia", "SIGNIA", "زیگنیا", "زیمنس" }
                },
                new BrandLandingInfo
                {
                    Slug = "oticon",
                    PersianName = "اتیکن",
                    EnglishName = "Oticon",
                    SearchTerms = new[] { "اتیکن", "Oticon", "OTICON", "اوتیکن" }
                },
                new BrandLandingInfo
                {
                    Slug = "phonak",
                    PersianName = "فوناک",
                    EnglishName = "Phonak",
                    SearchTerms = new[] { "فوناک", "Phonak", "PHONAK", "فونک" }
                },
                new BrandLandingInfo
                {
                    Slug = "resound",
                    PersianName = "ریساند",
                    EnglishName = "ReSound",
                    SearchTerms = new[] { "ریساند", "Resound", "ReSound", "RESOUND", "ری ساوند" }
                },
                new BrandLandingInfo
                {
                    Slug = "starkey",
                    PersianName = "استارکی",
                    EnglishName = "Starkey",
                    SearchTerms = new[] { "استارکی", "Starkey", "STARKEY" }
                },
                new BrandLandingInfo
                {
                    Slug = "bernafon",
                    PersianName = "برنافن",
                    EnglishName = "Bernafon",
                    SearchTerms = new[] { "برنافن", "Bernafon", "BERNAFON" }
                },
                new BrandLandingInfo
                {
                    Slug = "unitron",
                    PersianName = "یونیترون",
                    EnglishName = "Unitron",
                    SearchTerms = new[] { "یونیترون", "Unitron", "UNITRON" }
                },
                new BrandLandingInfo
                {
                    Slug = "hansaton",
                    PersianName = "هنساتون",
                    EnglishName = "Hansaton",
                    SearchTerms = new[] { "هنساتون", "Hansaton", "HANSATON" }
                },
                new BrandLandingInfo
                {
                    Slug = "beltone",
                    PersianName = "بلتون",
                    EnglishName = "Beltone",
                    SearchTerms = new[] { "بلتون", "Beltone", "BELTONE" }
                },
                new BrandLandingInfo
                {
                    Slug = "audifon",
                    PersianName = "ادیفون",
                    EnglishName = "Audifon",
                    SearchTerms = new[] { "ادیفون", "Audifon", "AUDIFON" }
                },
                new BrandLandingInfo
                {
                    Slug = "sonic",
                    PersianName = "سونیک",
                    EnglishName = "Sonic",
                    SearchTerms = new[] { "سونیک", "Sonic", "SONIC" }
                },
                new BrandLandingInfo
                {
                    Slug = "interton",
                    PersianName = "اینترتون",
                    EnglishName = "Interton",
                    SearchTerms = new[] { "اینترتون", "Interton", "INTERTON" }
                },
                new BrandLandingInfo
                {
                    Slug = "rion",
                    PersianName = "ریون",
                    EnglishName = "Rion",
                    SearchTerms = new[] { "ریون", "Rion", "RION" }
                },
                new BrandLandingInfo
                {
                    Slug = "coselgi",
                    PersianName = "کوسلجی",
                    EnglishName = "Coselgi",
                    SearchTerms = new[] { "کوسلجی", "Coselgi", "COSELGI" }
                }
        };

        [HttpGet]
        public ActionResult Index(string brand, int page = 1)
        {
            page = NormalizePage(page);

            ViewBag.BrandLinks = Brands
                .Select(current => Tuple.Create(current.Slug, current.PersianName, current.EnglishName))
                .ToList();

            if (string.IsNullOrWhiteSpace(brand))
            {
                ViewBag.IsBrandList = true;

                PagerViewModels<Product> emptyModel = new PagerViewModels<Product>();
                emptyModel.CurrentPage = page;
                emptyModel.TotalItemCount = 0;
                emptyModel.data = new List<Product>();

                return View(emptyModel);
            }

            string normalizedBrand = NormalizeBrand(brand);
            BrandLandingInfo brandInfo = Brands.FirstOrDefault(current =>
                current.Slug.Equals(normalizedBrand, StringComparison.OrdinalIgnoreCase) ||
                current.PersianName.Equals(brand, StringComparison.OrdinalIgnoreCase) ||
                current.EnglishName.Equals(brand, StringComparison.OrdinalIgnoreCase));

            if (brandInfo == null)
            {
                return HttpNotFound();
            }

            ViewBag.IsBrandList = false;
            ViewBag.BrandSlug = brandInfo.Slug;
            ViewBag.BrandPersianName = brandInfo.PersianName;
            ViewBag.BrandEnglishName = brandInfo.EnglishName;

            IQueryable<Product> baseQuery = db.Products
                .AsNoTracking()
                .Include(current => current.category)
                .Where(current => current.SiteFirstImage != null);

            IQueryable<Product> query = null;

            foreach (string rawTerm in brandInfo.SearchTerms.Where(current => !string.IsNullOrWhiteSpace(current)))
            {
                string term = rawTerm.Trim();
                IQueryable<Product> part = baseQuery.Where(current =>
                    current.PersianName.Contains(term) ||
                    current.EnglishName.Contains(term) ||
                    current.Description.Contains(term) ||
                    current.category.PersianName.Contains(term));

                query = query == null ? part : query.Union(part);
            }

            if (query == null)
            {
                query = baseQuery.Where(current => false);
            }

            PagerViewModels<Product> viewModel = new PagerViewModels<Product>();
            viewModel.CurrentPage = page;
            viewModel.TotalItemCount = query.Count();
            viewModel.data = query
                .OrderByDescending(current => current.CreateDate)
                .ThenByDescending(current => current.PersianName)
                .Skip((page - 1) * BrandPageSize)
                .Take(BrandPageSize)
                .ToList();

            return View(viewModel);
        }

        private string NormalizeBrand(string value)
        {
            return (value ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("_", "-");
        }

        private int NormalizePage(int page)
        {
            return page < 1 ? 1 : page;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private class BrandLandingInfo
        {
            public string Slug { get; set; }
            public string PersianName { get; set; }
            public string EnglishName { get; set; }
            public string[] SearchTerms { get; set; }
        }
    }
}
