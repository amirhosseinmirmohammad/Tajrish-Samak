using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class BlogController : Controller
    {
        private const int BlogPageSize = 4;
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(int? id, string sefUrl)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var blog = db.Blogs
                .Include(c => c.Category)
                .Include(c => c.Images)
                .Include(c => c.User)
                .Include(c => c.User.Roles)
                .Include(c => c.BlogComments)
                .FirstOrDefault(c =>
                    c.Id == id.Value &&
                    c.IsVisible &&
                    c.Images.Any());

            // ❌ NOT FOUND → redirect (no 404 soft trap)
            if (blog == null)
                return RedirectPermanent("/Blog/All");

            // =========================
            // FIX 1: canonical redirect (VERY IMPORTANT)
            // =========================
            if (!string.IsNullOrWhiteSpace(sefUrl))
            {
                if (!sefUrl.Equals(blog.SefUrl, StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectPermanent(
                        $"/Blog/{blog.Id}/{blog.SefUrl}"
                    );
                }
            }

            // =========================
            // FIX 2: prevent duplicate "terms" junk pages
            // =========================
            if (sefUrl != null && sefUrl.Equals("terms", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectPermanent("/Blog/All");
            }

            blog.Survey++;
            db.Entry(blog).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.comments = db.BlogComments
                .AsNoTracking()
                .Where(c => c.BlogId == id.Value && c.IsApprove)
                .OrderByDescending(c => c.DateTime)
                .ToList();

            return View(blog);
        }

        [HttpGet]
        public ActionResult All(string q, int page = 1)
        {
            page = NormalizePage(page);

            IQueryable<Blog> query = db.Blogs
                .AsNoTracking()
                .Where(current =>
                    current.Images.Any() &&
                    current.IsVisible == true);

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(current =>
                    current.Title.Contains(q) ||
                    current.ShortDesc.Contains(q) ||
                    current.MetaDesc.Contains(q) ||
                    current.MetaKey.Contains(q) ||
                    current.SefUrl.Contains(q));
            }

            int totalCount = query.Count();

            if (!string.IsNullOrWhiteSpace(q) && totalCount == 0)
            {
                TempData["NotFound"] = "متاسفانه موردی پیدا نشد .";
            }

            if (string.IsNullOrWhiteSpace(q) && totalCount == 0)
            {
                TempData["NotFound"] = "هنوز مطلبی در سایت وجود ندارد .";
            }

            PagerViewModels<Blog> blogViewModels = new PagerViewModels<Blog>();
            blogViewModels.CurrentPage = page;
            blogViewModels.TotalItemCount = totalCount;
            blogViewModels.data = query
                .Include(current => current.Images)
                .OrderByDescending(current => current.CreateDate)
                .Skip((page - 1) * BlogPageSize)
                .Take(BlogPageSize)
                .ToList();

            return View(blogViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitComment([Bind(Include = "Id,FullName,Text,DateTime,IsApprove,BlogId")] BlogComment model)
        {
            if (!ModelState.IsValid || model.BlogId <= 0)
            {
                TempData["Error"] = "متاسفانه خطایی رخ داده است لطفا اطلاعات خود را بررسی و مجدد تلاش نمایید";
                return RedirectToAction("All");
            }

            bool blogExists = db.Blogs
                .AsNoTracking()
                .Any(current =>
                    current.Id == model.BlogId &&
                    current.IsVisible == true);

            if (!blogExists)
            {
                TempData["Error"] = "مطلب مورد نظر پیدا نشد .";
                return RedirectToAction("All");
            }

            model.Id = 0;
            model.DateTime = DateTime.Now;
            model.IsApprove = false;

            db.BlogComments.Add(model);
            db.SaveChanges();

            TempData["Success"] = "نظر شما با موفقیت در سیستم ثبت گردید که پس از تایید مدیریت به نمایش در می آید .";
            return RedirectToAction("Index", "Blog", new { id = model.BlogId });
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
    }
}
