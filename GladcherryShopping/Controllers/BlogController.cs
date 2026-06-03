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
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Blog blog = db.Blogs
                .Include(current => current.Category)
                .Include(current => current.Images)
                .Include(current => current.User)
                .Include(current => current.User.Roles)
                .Include(current => current.BlogComments)
                .FirstOrDefault(current =>
                    current.Id == id.Value &&
                    current.IsVisible == true &&
                    current.Images.Any());

            if (blog == null)
            {
                return HttpNotFound();
            }

            blog.Survey++;
            db.Entry(blog).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.comments = db.BlogComments
                .AsNoTracking()
                .Where(current => current.BlogId == id.Value && current.IsApprove == true)
                .OrderByDescending(current => current.DateTime)
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
