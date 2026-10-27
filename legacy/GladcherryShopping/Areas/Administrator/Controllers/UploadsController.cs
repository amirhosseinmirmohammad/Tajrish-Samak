using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
using GladCherryShopping.Helpers;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static GladCherryShopping.Helpers.FunctionsHelper;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UploadsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Administrator/Uploads
        public ActionResult Index(int page = 1)
        {
            IQueryable<Upload> Uploads = db.Uploads;
            PagerViewModels<Upload> UploadViewModels = new PagerViewModels<Upload>();
            UploadViewModels.CurrentPage = page;
            UploadViewModels.data = Uploads.OrderBy(current => current.Id).Skip((page - 1) * 10).Take(10).ToList();
            UploadViewModels.TotalItemCount = Uploads.Count();
            return View(UploadViewModels);
        }

        public ActionResult UpdateMe(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var Upload = db.Uploads.Find(id);

            if (Upload == null)
            {
                return HttpNotFound();
            }
            Upload.LastDate = DateTime.Now;
            db.Entry(Upload).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Success"] = "به روز رسانی تصویر مورد نظر شما با موفقیت انجام شد .";
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Image,Link,CreateDate,LastDate,IsApp,MobImage")] Upload Upload,
            HttpPostedFileBase Image, HttpPostedFileBase MobImage)
        {
            if (ModelState.IsValid)
            {
                #region FileUploading
                if (Image != null && Image.ContentLength > 0)
                {
                    string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/content/images/Uploads/", true, Image, Server);
                    Upload.Image = smallImagePath;
                }
                if (MobImage != null && MobImage.ContentLength > 0)
                {
                    string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/content/images/Uploads/", true, MobImage, Server);
                    Upload.MobImage = smallImagePath;
                }
                #endregion FileUploading
                Upload.CreateDate = DateTime.Now;
                Upload.LastDate = DateTime.Now;
                db.Uploads.Add(Upload);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Upload);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Upload Upload = db.Uploads.Find(id);
            if (Upload == null)
            {
                return HttpNotFound();
            }
            return View(Upload);
        }

        // POST: Administrator/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Image,Link,CreateDate,LastDate,IsApp,MobImage")] Upload Upload,
            HttpPostedFileBase Image, HttpPostedFileBase MobImage)
        {
            if (ModelState.IsValid)
            {
                Upload edited = db.Uploads.Find(Upload.Id);
                edited.Link = Upload.Link;
                edited.IsApp = Upload.IsApp;
                #region FileUploading
                if (Image != null && Image.ContentLength > 0)
                {
                    if (Upload.Image != null)
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, Upload.Image, true, Image, Server);
                        edited.Image = smallImagePath;
                    }
                    else
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/content/images/Uploads/", true, Image, Server);
                        edited.Image = smallImagePath;
                    }
                }
                if (MobImage != null && MobImage.ContentLength > 0)
                {
                    if (Upload.MobImage != null)
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, Upload.Image, true, MobImage, Server);
                        edited.MobImage = smallImagePath;
                    }
                    else
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/content/images/Uploads/", true, MobImage, Server);
                        edited.MobImage = smallImagePath;
                    }
                }
                edited.Link = Upload.Link;
                #endregion FileUploading
                db.Entry(edited).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Upload);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Upload Upload = db.Uploads.Find(id);
            if (Upload == null)
            {
                return HttpNotFound();
            }
            return View(Upload);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Upload Upload = db.Uploads.Find(id);
            db.Uploads.Remove(Upload);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها در سیستم امکان حذف این تصویر وجود ندارد .";
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