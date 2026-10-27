using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer.Models;
using GladcherryShopping.Models;
using DataLayer.ViewModels.PagerViewModel;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class NewsLettersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/NewsLetters
        public ActionResult Index(int page = 1)
        {
            var newsletters = db.NewsLetters;
            PagerViewModels<NewsLetter> CategoryViewModels = new PagerViewModels<NewsLetter>();
            CategoryViewModels.CurrentPage = page;
            CategoryViewModels.data = newsletters.OrderByDescending(current => current.Email).Skip((page - 1) * 10).Take(10).ToList();
            CategoryViewModels.TotalItemCount = newsletters.Count();
            return View(CategoryViewModels);
        }

        // GET: Administrator/NewsLetters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsLetter newsLetter = db.NewsLetters.Find(id);
            if (newsLetter == null)
            {
                return HttpNotFound();
            }
            return View(newsLetter);
        }

        // GET: Administrator/NewsLetters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/NewsLetters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email")] NewsLetter newsLetter)
        {
            if (ModelState.IsValid)
            {
                db.NewsLetters.Add(newsLetter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(newsLetter);
        }

        // GET: Administrator/NewsLetters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsLetter newsLetter = db.NewsLetters.Find(id);
            if (newsLetter == null)
            {
                return HttpNotFound();
            }
            return View(newsLetter);
        }

        // POST: Administrator/NewsLetters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email")] NewsLetter newsLetter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newsLetter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newsLetter);
        }

        // GET: Administrator/NewsLetters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsLetter newsLetter = db.NewsLetters.Find(id);
            if (newsLetter == null)
            {
                return HttpNotFound();
            }
            return View(newsLetter);
        }

        // POST: Administrator/NewsLetters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NewsLetter newsLetter = db.NewsLetters.Find(id);
            db.NewsLetters.Remove(newsLetter);
            try
            {
                db.SaveChanges();
                TempData["Success"] = "عضو مورد نظر از لیست خبرنامه حذف شد .";
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها امکان حذف این عضو خبرنامه از سیستم وجود ندارد .";
            }
            return RedirectToAction("Index");
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
