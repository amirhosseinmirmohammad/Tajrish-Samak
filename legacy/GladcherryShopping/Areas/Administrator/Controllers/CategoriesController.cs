using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using static GladCherryShopping.Helpers.FunctionsHelper;
using GladcherryShopping.Models;
using GladCherryShopping.Helpers;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Categories
        public ActionResult Index(int page = 1)
        {
            var categories = db.Categories.Include(c => c.Parent).Include(current => current.Products);
            PagerViewModels<Category> CategoryViewModels = new PagerViewModels<Category>();
            CategoryViewModels.CurrentPage = page;
            CategoryViewModels.data = categories.Include(current => current.Parent).OrderByDescending(current => current.PersianName).Skip((page - 1) * 10).Take(10).ToList();
            CategoryViewModels.TotalItemCount = categories.Count();
            return View(CategoryViewModels);
        }

        // GET: Administrator/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Administrator/Categories/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.Categories, "Id", "PersianName");
            return View();
        }

        // POST: Administrator/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PersianName,EnglishName,Icon,SmallImage,LargeImage,BackgroundProImage,ParentId,IsBlog")] Category category,
            HttpPostedFileBase small_image, HttpPostedFileBase large_image, HttpPostedFileBase background_image)
        {
            if (ModelState.IsValid)
            {
                #region FileUploading
                if (small_image != null && small_image.ContentLength > 0)
                {
                    string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Categories/", true, small_image, Server);
                    category.SmallImage = smallImagePath;
                }
                if (large_image != null && large_image.ContentLength > 0)
                {
                    string largeImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Categories/", true, large_image, Server);
                    category.LargeImage = largeImagePath;
                }
                if (background_image != null && background_image.ContentLength > 0)
                {
                    string backgroundImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Categories/", true, background_image, Server);
                    category.BackgroundProImage = backgroundImagePath;
                }
                #endregion FileUploading
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.Categories, "Id", "PersianName", category.ParentId);
            return View(category);
        }

        // GET: Administrator/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.Categories, "Id", "PersianName", category.ParentId);
            return View(category);
        }

        // POST: Administrator/Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PersianName,EnglishName,Icon,SmallImage,LargeImage,BackgroundProImage,ParentId,IsBlog")] Category category,
            HttpPostedFileBase small_image, HttpPostedFileBase large_image, HttpPostedFileBase background_image)
        {
            if (ModelState.IsValid)
            {
                #region FileUploading 
                if (small_image != null && small_image.ContentLength > 0)
                {
                    if (category.SmallImage != null)
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, category.SmallImage, true, small_image, Server);
                        category.SmallImage = smallImagePath;
                    }
                    else
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Categories/", true, small_image, Server);
                        category.SmallImage = smallImagePath;
                    }
                }
                if (large_image != null && large_image.ContentLength > 0)
                {
                    if (category.LargeImage != null)
                    {
                        string largeImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, category.LargeImage, true, large_image, Server);
                        category.LargeImage = largeImagePath;
                    }
                    else
                    {
                        string largeImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Categories/", true, large_image, Server);
                        category.LargeImage = largeImagePath;
                    }
                }
                if (background_image != null && background_image.ContentLength > 0)
                {
                    if (category.BackgroundProImage != null)
                    {
                        string backgroundImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, category.BackgroundProImage, true, background_image, Server);
                        category.BackgroundProImage = backgroundImagePath;
                    }
                    else
                    {
                        string backgroundImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Categories/", true, background_image, Server);
                        category.BackgroundProImage = backgroundImagePath;
                    }
                }
                #endregion FileUploading
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.Categories, "Id", "PersianName", category.ParentId);
            return View(category);
        }

        // GET: Administrator/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Administrator/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها در سیستم امکان حذف این دسته بندی وجود ندارد .";
                return RedirectToAction("Index");
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
