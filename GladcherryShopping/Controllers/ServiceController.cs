using DataLayer.Models;
using GladcherryShopping.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public class FaqViewModel
        {
            public string Q { get; set; }
            public string A { get; set; }
        }

        [HttpGet]
        public ActionResult All(int page = 1)
        {
            int pageSize = 12;

            var services = db.Services
                .Include(x => x.Images)
                .Include(x => x.Category)
                .Where(x => x.IsVisible)
                .OrderByDescending(x => x.Survey)
                .ToList();

            // =============================
            // 🧠 AUTO CATEGORY DETECTION
            // =============================
            var categories = services
                .GroupBy(x => x.Category?.PersianName ?? "سایر")
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToList();

            ViewBag.Categories = categories;

            // =============================
            // 🔗 SMART INTERNAL LINKS
            // =============================
            ViewBag.InternalLinks = new[]
            {
        new { Title = "تست شنوایی", Url = "/tehran/hearing-test" },
        new { Title = "سمعک", Url = "/tehran/hearing-aid" },
        new { Title = "قیمت سمعک", Url = "/tehran/hearing-aid-price" },
        new { Title = "برندها", Url = "/hearing-aid-brands" }
    };

            // =============================
            // 📊 SEO SCORE ENGINE
            // =============================
            ViewBag.SeoScore = CalculateSeoScore(services);

            // =============================
            // ❓ AUTO FAQ GENERATOR
            // =============================
            ViewBag.Faqs = new List<FaqViewModel>
        {
            new FaqViewModel
            {
                Q = "بهترین زمان تست شنوایی چه زمانی است؟",
                A = "در صورت احساس کاهش شنوایی باید سریع بررسی انجام شود."
            },
            new FaqViewModel
            {
                Q = "قیمت سمعک چقدر است؟",
                A = "بسته به مدل و برند متفاوت است."
            },
            new FaqViewModel
            {
                Q = "آیا تنظیم سمعک ضروری است؟",
                A = "بله، برای عملکرد صحیح ضروری است."
            }
        };

            // =============================
            // 📦 PAGINATION (SEO SAFE)
            // =============================
            var paged = services
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.HasNext = services.Count > page * pageSize;

            return View(paged);
        }

        private int CalculateSeoScore(List<Service> services)
        {
            int score = 60;

            if (services.Any(x => x.Images.Any())) score += 10;
            if (services.Any(x => !string.IsNullOrEmpty(x.ShortDesc))) score += 10;
            if (services.Count > 10) score += 10;
            if (services.Any()) score += 10;

            return score;
        }

        [HttpGet]
        public ActionResult Index(int? id, string sefUrl)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Service service = db.Services
                .Include(current => current.Category)
                .Include(current => current.Images)
                .Include(current => current.User)
                .Include(current => current.User.Roles)
                .FirstOrDefault(current =>
                    current.Id == id.Value &&
                    current.IsVisible == true &&
                    current.Images.Any());

            if (service == null)
            {
                return HttpNotFound();
            }

            service.Survey++;
            db.Entry(service).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.RelatedServices = db.Services
                .AsNoTracking()
                .Include(current => current.Images)
                .Where(current =>
                    current.IsVisible == true &&
                    current.Id != service.Id &&
                    current.CategoryId == service.CategoryId &&
                    current.Images.Any())
                .OrderByDescending(current => current.Survey)
                .Take(4)
                .ToList();

            return View(service);
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
}
