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
    public class ServiceCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/ServiceCategories
        public ActionResult Index(int page = 1)
        {
            var ServiceCategories = db.ServiceCategories.Include(c => c.Parent).Include(current => current.Services);
            PagerViewModels<ServiceCategory> ServiceCategoryViewModels = new PagerViewModels<ServiceCategory>();
            ServiceCategoryViewModels.CurrentPage = page;
            ServiceCategoryViewModels.data = ServiceCategories.Include(current => current.Parent).OrderByDescending(current => current.PersianName).Skip((page - 1) * 10).Take(10).ToList();
            ServiceCategoryViewModels.TotalItemCount = ServiceCategories.Count();
            return View(ServiceCategoryViewModels);
        }

        // GET: Administrator/ServiceCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceCategory ServiceCategory = db.ServiceCategories.Find(id);
            if (ServiceCategory == null)
            {
                return HttpNotFound();
            }
            return View(ServiceCategory);
        }

        // GET: Administrator/ServiceCategories/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.ServiceCategories, "Id", "PersianName");
            return View();
        }

        // POST: Administrator/ServiceCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PersianName,EnglishName,Icon,SmallImage,LargeImage,BackgroundProImage,ParentId,IsBlog")] ServiceCategory ServiceCategory,
            HttpPostedFileBase small_image, HttpPostedFileBase large_image, HttpPostedFileBase background_image)
        {
            if (ModelState.IsValid)
            {
                #region FileUploading
                if (small_image != null && small_image.ContentLength > 0)
                {
                    string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/ServiceCategories/", true, small_image, Server);
                    ServiceCategory.SmallImage = smallImagePath;
                }
                if (large_image != null && large_image.ContentLength > 0)
                {
                    string largeImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/ServiceCategories/", true, large_image, Server);
                    ServiceCategory.LargeImage = largeImagePath;
                }
                if (background_image != null && background_image.ContentLength > 0)
                {
                    string backgroundImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/ServiceCategories/", true, background_image, Server);
                    ServiceCategory.BackgroundProImage = backgroundImagePath;
                }
                #endregion FileUploading
                db.ServiceCategories.Add(ServiceCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.ServiceCategories, "Id", "PersianName", ServiceCategory.ParentId);
            return View(ServiceCategory);
        }

        // GET: Administrator/ServiceCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceCategory ServiceCategory = db.ServiceCategories.Find(id);
            if (ServiceCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.ServiceCategories, "Id", "PersianName", ServiceCategory.ParentId);
            return View(ServiceCategory);
        }

        // POST: Administrator/ServiceCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PersianName,EnglishName,Icon,SmallImage,LargeImage,BackgroundProImage,ParentId,IsBlog")] ServiceCategory ServiceCategory,
            HttpPostedFileBase small_image, HttpPostedFileBase large_image, HttpPostedFileBase background_image)
        {
            if (ModelState.IsValid)
            {
                #region FileUploading 
                if (small_image != null && small_image.ContentLength > 0)
                {
                    if (ServiceCategory.SmallImage != null)
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, ServiceCategory.SmallImage, true, small_image, Server);
                        ServiceCategory.SmallImage = smallImagePath;
                    }
                    else
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/ServiceCategories/", true, small_image, Server);
                        ServiceCategory.SmallImage = smallImagePath;
                    }
                }
                if (large_image != null && large_image.ContentLength > 0)
                {
                    if (ServiceCategory.LargeImage != null)
                    {
                        string largeImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, ServiceCategory.LargeImage, true, large_image, Server);
                        ServiceCategory.LargeImage = largeImagePath;
                    }
                    else
                    {
                        string largeImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/ServiceCategories/", true, large_image, Server);
                        ServiceCategory.LargeImage = largeImagePath;
                    }
                }
                if (background_image != null && background_image.ContentLength > 0)
                {
                    if (ServiceCategory.BackgroundProImage != null)
                    {
                        string backgroundImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, ServiceCategory.BackgroundProImage, true, background_image, Server);
                        ServiceCategory.BackgroundProImage = backgroundImagePath;
                    }
                    else
                    {
                        string backgroundImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/ServiceCategories/", true, background_image, Server);
                        ServiceCategory.BackgroundProImage = backgroundImagePath;
                    }
                }
                #endregion FileUploading
                db.Entry(ServiceCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.ServiceCategories, "Id", "PersianName", ServiceCategory.ParentId);
            return View(ServiceCategory);
        }

        // GET: Administrator/ServiceCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceCategory ServiceCategory = db.ServiceCategories.Find(id);
            if (ServiceCategory == null)
            {
                return HttpNotFound();
            }
            return View(ServiceCategory);
        }

        // POST: Administrator/ServiceCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceCategory ServiceCategory = db.ServiceCategories.Find(id);
            db.ServiceCategories.Remove(ServiceCategory);
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
