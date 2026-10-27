using DataLayer.Models;
using GladcherryShopping.Models;
using DataLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(string q, string type)
        {
            string query = Normalize(q);
            string activeType = NormalizeType(type);

            SiteSearchViewModel model = new SiteSearchViewModel();
            model.Query = query;
            model.ActiveType = activeType;
            model.Results = new List<SiteSearchResult>();

            if (string.IsNullOrWhiteSpace(query))
            {
                AddDefaultResults(model.Results);
            }
            else
            {
                SearchStaticGeoPages(query, model.Results);
                SearchProducts(query, model.Results);
                SearchBlogs(query, model.Results);
                SearchServiceCategories(query, model.Results);
                SearchProductAndBlogCategories(query, model.Results);
                SearchBrands(query, model.Results);
                SearchCoreSitePages(query, model.Results);
            }

            model.Results = model.Results
                .GroupBy(current => current.Url.ToLowerInvariant())
                .Select(current => current.OrderByDescending(item => item.Score).FirstOrDefault())
                .Where(current => current != null)
                .OrderByDescending(current => current.Score)
                .ThenBy(current => current.TypeOrder)
                .ThenBy(current => current.Title)
                .ToList();

            if (!string.IsNullOrWhiteSpace(activeType) && activeType != "all")
            {
                model.Results = model.Results
                    .Where(current => current.TypeKey == activeType)
                    .ToList();
            }

            model.TotalCount = model.Results.Count;
            model.ProductCount = model.Results.Count(current => current.TypeKey == "product");
            model.BlogCount = model.Results.Count(current => current.TypeKey == "blog");
            model.ServiceCount = model.Results.Count(current => current.TypeKey == "service");
            model.GeoCount = model.Results.Count(current => current.TypeKey == "geo");
            model.BrandCount = model.Results.Count(current => current.TypeKey == "brand");
            model.PageCount = model.Results.Count(current => current.TypeKey == "page");

            model.Results = model.Results.Take(60).ToList();

            ViewBag.Title = string.IsNullOrWhiteSpace(query)
                ? "جستجوی سایت | کلینیک شنوایی و سمعک شکوه تجریش"
                : "جستجو برای " + query + " | کلینیک شنوایی و سمعک شکوه تجریش";

            ViewBag.MetaDescription = "جستجوی سریع در محصولات، خدمات، مقالات، برندها و صفحات محله‌ای کلینیک شنوایی و سمعک شکوه تجریش.";
            var baseUrl = "https://tajrish-samak.ir/search";

            if (!string.IsNullOrEmpty(Request.QueryString["q"]) || !string.IsNullOrEmpty(Request.QueryString["type"]))
            {
                ViewBag.CanonicalUrl = baseUrl;
            }
            else
            {
                ViewBag.CanonicalUrl = baseUrl;
            }

            ViewBag.Robots = "noindex, follow";

            return View(model);
        }

        private void SearchProducts(string query, List<SiteSearchResult> results)
        {
            List<Product> products = db.Products
                .AsNoTracking()
                .Where(current =>
                    current.SiteFirstImage != null &&
                    (
                        current.PersianName.Contains(query) ||
                        current.EnglishName.Contains(query) ||
                        current.Description.Contains(query)
                    ))
                .OrderByDescending(current => current.CreateDate)
                .Take(12)
                .ToList();

            foreach (Product item in products)
            {
                AddResult(results, new SiteSearchResult
                {
                    TypeKey = "product",
                    TypeName = "محصول",
                    TypeOrder = 2,
                    Title = item.PersianName,
                    Description = MakeSnippet(item.Description, "مشاهده مشخصات، بررسی و دریافت مشاوره تخصصی برای انتخاب سمعک."),
                    Url = "/Product/Details/" + item.Id,
                    ImageUrl = item.SiteFirstImage,
                    Score = ScoreText(query, item.PersianName, item.EnglishName, item.Description) + 60
                });
            }
        }

        private void SearchBlogs(string query, List<SiteSearchResult> results)
        {
            List<Blog> blogs = db.Blogs
                .AsNoTracking()
                .Where(current =>
                    current.IsVisible == true &&
                    (
                        current.Title.Contains(query) ||
                        current.ShortDesc.Contains(query)
                    ))
                .OrderByDescending(current => current.Survey)
                .ThenByDescending(current => current.CreateDate)
                .Take(10)
                .ToList();

            foreach (Blog item in blogs)
            {
                AddResult(results, new SiteSearchResult
                {
                    TypeKey = "blog",
                    TypeName = "مقاله",
                    TypeOrder = 4,
                    Title = item.Title,
                    Description = MakeSnippet(item.ShortDesc, "مطالعه مقاله و راهنمای آموزشی کلینیک شنوایی شکوه تجریش."),
                    Url = "/blog/" + item.Id + "/" + item.SefUrl,
                    ImageUrl = "",
                    Score = ScoreText(query, item.Title, item.ShortDesc) + 42
                });
            }
        }

        private void SearchServiceCategories(string query, List<SiteSearchResult> results)
        {
            List<ServiceCategory> services = db.ServiceCategories
                .AsNoTracking()
                .Where(current => current.PersianName.Contains(query))
                .OrderByDescending(current => current.PersianName)
                .Take(10)
                .ToList();

            foreach (ServiceCategory item in services)
            {
                AddResult(results, new SiteSearchResult
                {
                    TypeKey = "service",
                    TypeName = "خدمت",
                    TypeOrder = 3,
                    Title = item.PersianName,
                    Description = "صفحه خدمات کلینیک شنوایی و سمعک شکوه تجریش.",
                    Url = "/Home/Services/" + item.Id,
                    ImageUrl = "",
                    Score = ScoreText(query, item.PersianName) + 46
                });
            }
        }

        private void SearchProductAndBlogCategories(string query, List<SiteSearchResult> results)
        {
            List<Category> categories = db.Categories
                .AsNoTracking()
                .Where(current => current.PersianName.Contains(query))
                .OrderByDescending(current => current.PersianName)
                .Take(12)
                .ToList();

            foreach (Category item in categories)
            {
                bool isBlog = item.IsBlog == true;
                AddResult(results, new SiteSearchResult
                {
                    TypeKey = isBlog ? "blog" : "product",
                    TypeName = isBlog ? "دسته مقاله" : "دسته محصول",
                    TypeOrder = isBlog ? 5 : 2,
                    Title = item.PersianName,
                    Description = isBlog ? "مشاهده مقالات این دسته." : "مشاهده محصولات این دسته و دریافت مشاوره انتخاب سمعک.",
                    Url = isBlog ? "/Home/Blogs/" + item.Id : "/Product/All/" + item.Id,
                    ImageUrl = item.SmallImage,
                    Score = ScoreText(query, item.PersianName) + 36
                });
            }
        }

        private void SearchBrands(string query, List<SiteSearchResult> results)
        {
            List<BrandSearchItem> staticBrands = GetStaticBrands();

            foreach (BrandSearchItem brand in staticBrands)
            {
                int score = ScoreText(query, brand.PersianName, brand.EnglishName, brand.Alias);
                if (score > 0)
                {
                    AddResult(results, new SiteSearchResult
                    {
                        TypeKey = "brand",
                        TypeName = "برند",
                        TypeOrder = 6,
                        Title = "سمعک " + brand.PersianName,
                        Description = "بررسی برند " + brand.PersianName + "، مدل‌های مرتبط و مشاوره انتخاب سمعک.",
                        Url = "/hearing-aid-brands/" + brand.Slug,
                        ImageUrl = "",
                        Score = score + 55
                    });
                }
            }

            List<Brand> brands = db.Brands
                .AsNoTracking()
                .Where(current => current.PersianName.Contains(query))
                .OrderByDescending(current => current.PersianName)
                .Take(10)
                .ToList();

            foreach (Brand item in brands)
            {
                AddResult(results, new SiteSearchResult
                {
                    TypeKey = "brand",
                    TypeName = "برند",
                    TypeOrder = 6,
                    Title = item.PersianName,
                    Description = "مشاهده برند و مدل‌های مرتبط در سایت شکوه تجریش.",
                    Url = "/Home/Brands/" + item.Id,
                    ImageUrl = item.SmallImage,
                    Score = ScoreText(query, item.PersianName) + 38
                });
            }
        }

        private void SearchStaticGeoPages(string query, List<SiteSearchResult> results)
        {
            List<SearchAliasItem> areas = GetAreas();
            List<SearchAliasItem> services = GetServices();

            List<SearchAliasItem> matchedAreas = areas
                .Select(current => new SearchAliasItem
                {
                    Slug = current.Slug,
                    Name = current.Name,
                    Alias = current.Alias,
                    Score = ScoreText(query, current.Name, current.Alias)
                })
                .Where(current => current.Score > 0)
                .OrderByDescending(current => current.Score)
                .Take(5)
                .ToList();

            List<SearchAliasItem> matchedServices = services
                .Select(current => new SearchAliasItem
                {
                    Slug = current.Slug,
                    Name = current.Name,
                    Alias = current.Alias,
                    Score = ScoreText(query, current.Name, current.Alias)
                })
                .Where(current => current.Score > 0)
                .OrderByDescending(current => current.Score)
                .Take(6)
                .ToList();

            if (matchedAreas.Count == 0)
            {
                matchedAreas.Add(new SearchAliasItem { Slug = "tajrish", Name = "تجریش", Alias = "شمیرانات شمال تهران", Score = 1 });
                matchedAreas.Add(new SearchAliasItem { Slug = "tehran", Name = "تهران", Alias = "شهر تهران", Score = 1 });
                matchedAreas.Add(new SearchAliasItem { Slug = "north-tehran", Name = "شمال تهران", Alias = "شمیرانات", Score = 1 });
            }

            if (matchedServices.Count == 0)
            {
                matchedServices.Add(new SearchAliasItem { Slug = "best-audiology-clinic", Name = "بهترین کلینیک شنوایی", Alias = "بهترین مرکز شنوایی", Score = 1 });
                matchedServices.Add(new SearchAliasItem { Slug = "hearing-test", Name = "تست شنوایی", Alias = "آزمایش شنوایی نوار گوش", Score = 1 });
                matchedServices.Add(new SearchAliasItem { Slug = "hearing-aid", Name = "خرید سمعک", Alias = "سمعک", Score = 1 });
            }

            foreach (SearchAliasItem area in matchedAreas)
            {
                foreach (SearchAliasItem service in matchedServices)
                {
                    AddResult(results, new SiteSearchResult
                    {
                        TypeKey = "geo",
                        TypeName = "صفحه محله‌ای",
                        TypeOrder = 1,
                        Title = service.Name + " در " + area.Name,
                        Description = "صفحه اختصاصی " + service.Name + " برای محدوده " + area.Name + " در سایت کلینیک شنوایی و سمعک شکوه تجریش.",
                        Url = "/" + area.Slug + "/" + service.Slug,
                        ImageUrl = "",
                        Score = 78 + area.Score + service.Score
                    });
                }
            }
        }

        private void SearchCoreSitePages(string query, List<SiteSearchResult> results)
        {
            List<CorePageItem> pages = new List<CorePageItem>
            {
                new CorePageItem { Title = "تماس با کلینیک شنوایی و سمعک شکوه تجریش", Description = "آدرس، شماره تماس و مسیر مراجعه به کلینیک.", Url = "/contactus", Alias = "تماس آدرس شماره تلفن رزرو وقت تجریش" },
                new CorePageItem { Title = "درباره کلینیک شنوایی و سمعک شکوه تجریش", Description = "معرفی کلینیک، خدمات و مسیر فعالیت.", Url = "/aboutus", Alias = "درباره ما شکوه تجریش کلینیک شنوایی" },
                new CorePageItem { Title = "برندهای سمعک", Description = "بررسی برندهای سمعک مانند سیگنیا، ویدکس، فوناک و ریساند.", Url = "/hearing-aid-brands", Alias = "برند سمعک سیگنیا ویدکس فوناک ریساند" },
                new CorePageItem { Title = "مجله کلینیک شنوایی", Description = "مقالات آموزشی درباره تست شنوایی، سمعک و مراقبت شنوایی.", Url = "/blog/all", Alias = "مقاله مجله آموزش شنوایی سمعک" },
                new CorePageItem { Title = "محصولات سمعک", Description = "مشاهده محصولات و مدل‌های سمعک داخل سایت.", Url = "/Product/All", Alias = "محصولات سمعک خرید مدل" }
            };

            foreach (CorePageItem page in pages)
            {
                int score = ScoreText(query, page.Title, page.Description, page.Alias);
                if (score > 0)
                {
                    AddResult(results, new SiteSearchResult
                    {
                        TypeKey = "page",
                        TypeName = "صفحه",
                        TypeOrder = 7,
                        Title = page.Title,
                        Description = page.Description,
                        Url = page.Url,
                        ImageUrl = "",
                        Score = score + 25
                    });
                }
            }
        }

        private void AddDefaultResults(List<SiteSearchResult> results)
        {
            AddResult(results, new SiteSearchResult { TypeKey = "geo", TypeName = "صفحه محله‌ای", TypeOrder = 1, Title = "بهترین کلینیک شنوایی شمال تهران", Description = "صفحه مرجع برای انتخاب کلینیک شنوایی در شمال تهران.", Url = "/north-tehran/best-audiology-clinic", Score = 100 });
            AddResult(results, new SiteSearchResult { TypeKey = "geo", TypeName = "صفحه محله‌ای", TypeOrder = 1, Title = "تست شنوایی تجریش", Description = "شروع مسیر ارزیابی شنوایی در محدوده تجریش.", Url = "/tajrish/hearing-test", Score = 95 });
            AddResult(results, new SiteSearchResult { TypeKey = "geo", TypeName = "صفحه محله‌ای", TypeOrder = 1, Title = "قیمت سمعک سال ۱۴۰۵", Description = "راهنمای هزینه، بیمه، برند و انتخاب سمعک.", Url = "/tehran/hearing-aid-price-1405", Score = 92 });
            AddResult(results, new SiteSearchResult { TypeKey = "brand", TypeName = "برند", TypeOrder = 6, Title = "برندهای سمعک", Description = "بررسی برندهای سمعک و مدل‌های مرتبط.", Url = "/hearing-aid-brands", Score = 88 });
            AddResult(results, new SiteSearchResult { TypeKey = "page", TypeName = "صفحه", TypeOrder = 7, Title = "تماس با شکوه تجریش", Description = "آدرس، شماره تماس و رزرو وقت.", Url = "/contactus", Score = 80 });
        }

        private List<SearchAliasItem> GetAreas()
        {
            return new List<SearchAliasItem>
            {
                new SearchAliasItem { Slug = "tehran", Name = "تهران", Alias = "شهر تهران" },
                new SearchAliasItem { Slug = "tajrish", Name = "تجریش", Alias = "بازار تجریش پل تجریش میدان قدس امامزاده صالح" },
                new SearchAliasItem { Slug = "north-tehran", Name = "شمال تهران", Alias = "شمال شهر شمیرانات" },
                new SearchAliasItem { Slug = "northeast-tehran", Name = "شمال شرق تهران", Alias = "هروی پاسداران اقدسیه ارتش سوهانک" },
                new SearchAliasItem { Slug = "near-me", Name = "نزدیک من", Alias = "محله ما اطراف من نزدیکترین" },
                new SearchAliasItem { Slug = "shemiran", Name = "شمیران", Alias = "شمیرانات" },
                new SearchAliasItem { Slug = "shemiranat", Name = "شمیرانات", Alias = "شمیران شمال تهران" },
                new SearchAliasItem { Slug = "darabad", Name = "دارآباد", Alias = "داراباد" },
                new SearchAliasItem { Slug = "kashanak", Name = "کاشانک", Alias = "" },
                new SearchAliasItem { Slug = "jamaran", Name = "جماران", Alias = "" },
                new SearchAliasItem { Slug = "dezashib", Name = "دزاشیب", Alias = "" },
                new SearchAliasItem { Slug = "yaser", Name = "یاسر", Alias = "" },
                new SearchAliasItem { Slug = "farmanieh", Name = "فرمانیه", Alias = "" },
                new SearchAliasItem { Slug = "niavaran", Name = "نیاوران", Alias = "" },
                new SearchAliasItem { Slug = "qeytarieh", Name = "قیطریه", Alias = "" },
                new SearchAliasItem { Slug = "zaferanieh", Name = "زعفرانیه", Alias = "" },
                new SearchAliasItem { Slug = "elahiyeh", Name = "الهیه", Alias = "" },
                new SearchAliasItem { Slug = "velenjak", Name = "ولنجک", Alias = "" },
                new SearchAliasItem { Slug = "pasdaran", Name = "پاسداران", Alias = "" },
                new SearchAliasItem { Slug = "heravi", Name = "هروی", Alias = "" },
                new SearchAliasItem { Slug = "aghdasiyeh", Name = "اقدسیه", Alias = "" },
                new SearchAliasItem { Slug = "azgol", Name = "ازگل", Alias = "" },
                new SearchAliasItem { Slug = "sohanak", Name = "سوهانک", Alias = "" },
                new SearchAliasItem { Slug = "artesh", Name = "ارتش", Alias = "" },
                new SearchAliasItem { Slug = "nobonyad", Name = "نوبنیاد", Alias = "" },
                new SearchAliasItem { Slug = "kamranieh", Name = "کامرانیه", Alias = "" },
                new SearchAliasItem { Slug = "andarzgoo", Name = "اندرزگو", Alias = "" },
                new SearchAliasItem { Slug = "darband", Name = "دربند", Alias = "" },
                new SearchAliasItem { Slug = "emamzadeh-saleh", Name = "امامزاده صالح", Alias = "" },
                new SearchAliasItem { Slug = "bazar-tajrish", Name = "بازار تجریش", Alias = "" },
                new SearchAliasItem { Slug = "meydan-ghods", Name = "میدان قدس", Alias = "" },
                new SearchAliasItem { Slug = "bagh-ferdos", Name = "باغ فردوس", Alias = "" },
                new SearchAliasItem { Slug = "parkway", Name = "پارک‌وی", Alias = "پارک وی" },
                new SearchAliasItem { Slug = "lavasan", Name = "لواسان", Alias = "" },
                new SearchAliasItem { Slug = "feshm", Name = "فشم", Alias = "" }
            };
        }

        private List<SearchAliasItem> GetServices()
        {
            return new List<SearchAliasItem>
            {
                new SearchAliasItem { Slug = "best-audiology-clinic", Name = "بهترین کلینیک شنوایی", Alias = "بهترین مرکز شنوایی کلینیک معتبر شنوایی" },
                new SearchAliasItem { Slug = "best-hearing-clinic", Name = "بهترین مرکز شنوایی", Alias = "مرکز شنوایی معتبر" },
                new SearchAliasItem { Slug = "best-hearing-aid-clinic", Name = "بهترین مرکز سمعک", Alias = "بهترین کلینیک سمعک" },
                new SearchAliasItem { Slug = "hearing-test", Name = "تست شنوایی", Alias = "آزمایش شنوایی نوار گوش ادیومتری" },
                new SearchAliasItem { Slug = "audiology-clinic", Name = "کلینیک شنوایی", Alias = "مرکز شنوایی شنوایی سنجی" },
                new SearchAliasItem { Slug = "hearing-aid", Name = "خرید سمعک", Alias = "سمعک انتخاب سمعک تجویز سمعک" },
                new SearchAliasItem { Slug = "hearing-aid-adjustment", Name = "تنظیم سمعک", Alias = "تنظیم تخصصی سمعک" },
                new SearchAliasItem { Slug = "home-visit-hearing-aid", Name = "ویزیت سمعک در منزل", Alias = "ویزیت در منزل" },
                new SearchAliasItem { Slug = "hearing-aid-price", Name = "قیمت سمعک", Alias = "سمعک چنده هزینه سمعک" },
                new SearchAliasItem { Slug = "hearing-aid-price-1405", Name = "قیمت سمعک سال ۱۴۰۵", Alias = "قیمت سمعک 1405 قیمت سمعک ۱۴۰۵" },
                new SearchAliasItem { Slug = "installment-hearing-aid", Name = "سمعک قسطی", Alias = "خرید قسطی سمعک اقساط" },
                new SearchAliasItem { Slug = "government-hearing-aid", Name = "سمعک دولتی", Alias = "سمعک حمایتی دولتی" },
                new SearchAliasItem { Slug = "free-behzisti-hearing-aid", Name = "سمعک رایگان بهزیستی", Alias = "سمعک بهزیستی سمعک رایگان" },
                new SearchAliasItem { Slug = "deaf-support", Name = "حمایت ناشنوایان و کم‌شنوایان", Alias = "حمایت ناشنوایان حمایت کم شنوایان" },
                new SearchAliasItem { Slug = "insurance-hearing-aid", Name = "سمعک با بیمه", Alias = "بیمه سمعک" },
                new SearchAliasItem { Slug = "hearing-aid-insurance-tariff", Name = "تعرفه سمعک با بیمه", Alias = "تعرفه بیمه سمعک" },
                new SearchAliasItem { Slug = "hearing-aid-battery", Name = "باتری سمعک", Alias = "مصرف باتری سمعک" },
                new SearchAliasItem { Slug = "hearing-aid-filter", Name = "فیلتر سمعک", Alias = "" },
                new SearchAliasItem { Slug = "hearing-aid-repair", Name = "تعمیر سمعک", Alias = "تعمیرات سمعک" },
                new SearchAliasItem { Slug = "earmold", Name = "قالب سمعک", Alias = "قالب ضد آب قالب سیلیکونی تعویض قالب" },
                new SearchAliasItem { Slug = "rechargeable-hearing-aid", Name = "سمعک شارژی", Alias = "سمعک بدون باتری" },
                new SearchAliasItem { Slug = "invisible-hearing-aid", Name = "سمعک نامرئی", Alias = "" },
                new SearchAliasItem { Slug = "tinnitus-hearing-aid", Name = "وزوز گوش و سمعک", Alias = "وزوز گوش" },
                new SearchAliasItem { Slug = "signia-hearing-aid-price", Name = "قیمت سمعک سیگنیا", Alias = "قیمت سمعک زیمنس سیگنیا" },
                new SearchAliasItem { Slug = "siemens-hearing-aid-price", Name = "قیمت سمعک زیمنس", Alias = "زیمنس سیگنیا" },
                new SearchAliasItem { Slug = "widex-hearing-aid-price", Name = "قیمت سمعک ویدکس", Alias = "ویدکس" },
                new SearchAliasItem { Slug = "phonak-hearing-aid-price", Name = "قیمت سمعک فوناک", Alias = "فوناک" }
            };
        }

        private List<BrandSearchItem> GetStaticBrands()
        {
            return new List<BrandSearchItem>
            {
                new BrandSearchItem { Slug = "signia", PersianName = "سیگنیا", EnglishName = "Signia", Alias = "زیمنس Siemens" },
                new BrandSearchItem { Slug = "widex", PersianName = "ویدکس", EnglishName = "Widex", Alias = "" },
                new BrandSearchItem { Slug = "phonak", PersianName = "فوناک", EnglishName = "Phonak", Alias = "" },
                new BrandSearchItem { Slug = "resound", PersianName = "ریساند", EnglishName = "ReSound", Alias = "ریساوند" },
                new BrandSearchItem { Slug = "oticon", PersianName = "اتیکن", EnglishName = "Oticon", Alias = "" },
                new BrandSearchItem { Slug = "starkey", PersianName = "استارکی", EnglishName = "Starkey", Alias = "" }
            };
        }

        private int ScoreText(string query, params string[] values)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return 0;
            }

            string normalizedQuery = Normalize(query);
            int score = 0;

            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                string normalizedValue = Normalize(value);

                if (normalizedValue == normalizedQuery)
                {
                    score += 100;
                }
                else if (normalizedValue.Contains(normalizedQuery))
                {
                    score += 55;
                }

                string[] parts = normalizedQuery.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    if (part.Length >= 2 && normalizedValue.Contains(part))
                    {
                        score += 6;
                    }
                }
            }

            return score;
        }

        private string NormalizeType(string type)
        {
            string value = Normalize(type).ToLowerInvariant();
            switch (value)
            {
                case "product":
                case "blog":
                case "service":
                case "geo":
                case "brand":
                case "page":
                    return value;
                default:
                    return "all";
            }
        }

        private string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value
                .Trim()
                .Replace("ي", "ی")
                .Replace("ك", "ک")
                .Replace("‌", " ")
                .Replace("  ", " ");
        }

        private string MakeSnippet(string text, string fallback)
        {
            string value = string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();

            if (value.Length <= 145)
            {
                return value;
            }

            return value.Substring(0, 145) + "…";
        }

        private void AddResult(List<SiteSearchResult> results, SiteSearchResult result)
        {
            if (result == null || string.IsNullOrWhiteSpace(result.Url) || string.IsNullOrWhiteSpace(result.Title))
            {
                return;
            }

            results.Add(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
    internal class SearchAliasItem
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public int Score { get; set; }
    }

    internal class BrandSearchItem
    {
        public string Slug { get; set; }
        public string PersianName { get; set; }
        public string EnglishName { get; set; }
        public string Alias { get; set; }
    }

    internal class CorePageItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Alias { get; set; }
    }
}
