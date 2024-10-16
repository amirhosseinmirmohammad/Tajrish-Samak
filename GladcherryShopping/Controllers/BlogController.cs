using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class BlogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(int? id, string sefUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Where(current => current.Id == id && current.Images.Count > 0).Include(current => current.Category).Include(current => current.Images).Include(current => current.User).Include(current => current.User.Roles).Include(cuurent => cuurent.BlogComments).FirstOrDefault();
            blog.Survey++;
            db.Entry(blog).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.comments = db.BlogComments.Where(current => current.BlogId == id && current.IsApprove == true).ToList();
            return View(blog);
        }

        [HttpGet]
        public ActionResult All(string q, int page = 1)
        {
            var blog = new List<Blog>();
            if (q != null)
            {
                blog = db.Blogs.Where(current => current.Images.Count > 0 && current.IsVisible == true && current.Title.Contains(q) || current.ShortDesc.Contains(q) || current.MetaDesc.Contains(q) || current.MetaKey.Contains(q) || current.SefUrl.Contains(q)).OrderByDescending(current => current.CreateDate).Include(current => current.Images).ToList();
            }
            if (!string.IsNullOrEmpty(q) && blog.Count() == 0)
            {
                TempData["NotFound"] = "متاسفانه موردی پیدا نشد .";
            }
            else
            {
                blog = db.Blogs.Where(current => current.Images.Count > 0 && current.IsVisible == true).OrderByDescending(current => current.CreateDate).Include(current => current.Images).ToList();
            }
            if(string.IsNullOrEmpty(q) && blog.Count() == 0)
            {
                TempData["NotFound"] = "هنوز مطلبی در سایت وجود ندارد .";
            }
            PagerViewModels<Blog> BlogViewModels = new PagerViewModels<Blog>();
            BlogViewModels.data = blog.OrderBy(current => current.CreateDate).Skip((page - 1) * 4).Take(4).ToList();
            BlogViewModels.CurrentPage = page;
            BlogViewModels.TotalItemCount = blog.Count();
            return View(BlogViewModels);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitComment([Bind(Include = "Id,FullName,Text,DateTime,IsApprove,BlogId")] BlogComment model)
        {
            if (ModelState.IsValid)
            {
                model.DateTime = DateTime.Now;
                db.BlogComments.Add(model);
                db.SaveChanges();
                TempData["Success"] = "نظر شما با موفقیت در سیستم ثبت گردید که پس از تایید مدیریت به نمایش در می آید .";
                return RedirectToAction("Index", "Blog", new { id = model.BlogId });
            }
            TempData["Error"] = "متاسفانه خطایی رخ داده است لطفا اطلاعات خود را بررسی و مجدد تلاش نمایید";
            return View("Index", model);
        }
    }
}