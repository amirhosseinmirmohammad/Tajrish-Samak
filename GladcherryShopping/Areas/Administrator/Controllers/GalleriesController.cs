using DataLayer.Models;
using GladcherryShopping.Models;
using GladCherryShopping.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class GalleriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Sliders
        public ActionResult Index()
        {
            return View(db.Galleries.ToList());
        }

        // GET: Administrator/Sliders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        // GET: Administrator/Sliders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/Sliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id")] Gallery gallery, IEnumerable<HttpPostedFileBase> Images)
        {
            if (ModelState.IsValid)
            {
                db.Galleries.Add(gallery);
                db.SaveChanges();
                #region Image
                foreach (var image in Images)
                {
                    if (image != null && image.ContentLength > 0)
                    {
                        Image Newimage = new Image();
                        Newimage.Alt = "VIP";
                        Newimage.Title = "VIP";
                        Newimage.Source = "~/content/images/slider/";
                        Newimage.GalleryId = gallery.Id;
                        string Path = FunctionsHelper.File(FunctionsHelper.FileMode.Upload, FunctionsHelper.FileType.Image, Newimage.Source, true, image, Server);
                        if (Path != string.Empty)
                        {
                            Newimage.Link = Path;
                        }
                        db.Images.Add(Newimage);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception)
                        {
                            TempData["Success"] = " خطایی رخ داده است لطفا مجدد تلاش فرمایید . ";
                            return RedirectToAction("Index");
                        }
                    }
                }
                #endregion Image
                TempData["Success"] = " عکس مورد نظر با موفقیت ثبت شد . ";
                return RedirectToAction("Index");
            }

            return View(gallery);
        }

        // GET: Administrator/Sliders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gallery gallery = db.Galleries.Where(current => current.Id == id).Include(current => current.Images).FirstOrDefault();
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        // POST: Administrator/Sliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id")] Gallery gallery, IEnumerable<HttpPostedFileBase> Images)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gallery).State = EntityState.Modified;
                db.SaveChanges();
                #region Image
                foreach (var image in Images)
                {
                    if (image != null && image.ContentLength > 0)
                    {
                        Image Newimage = new Image();
                        Newimage.Alt = "VIP";
                        Newimage.Title = "VIP";
                        Newimage.Source = "~/content/images/slider/";
                        Newimage.GalleryId = gallery.Id;
                        string Path = FunctionsHelper.File(FunctionsHelper.FileMode.Upload, FunctionsHelper.FileType.Image, Newimage.Source, true, image, Server);
                        if (Path != string.Empty)
                        {
                            Newimage.Link = Path;
                        }
                        db.Images.Add(Newimage);
                        db.SaveChanges();
                    }
                }
                #endregion Image
                TempData["Success"] = " عکس مورد نظر با موفقیت ویرایش شد . ";
                return RedirectToAction("Index");
            }
            return View(gallery);
        }

        // GET: Administrator/Sliders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        // POST: Administrator/Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Gallery gallery = db.Galleries.Find(id);
            db.Galleries.Remove(gallery);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool DeleteImages(int id)
        {
            var list = db.Images.ToList();
            foreach (var image in list)
            {
                if (image.Id == id)
                {
                    db.Images.Remove(image);
                }
                db.SaveChanges();
            }
            return true;
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