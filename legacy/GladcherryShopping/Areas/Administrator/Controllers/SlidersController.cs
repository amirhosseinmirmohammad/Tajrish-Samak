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
    public class SlidersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Administrator/Sliders
        public ActionResult Index(int page = 1)
        {
            IQueryable<Slider> sliders = db.Sliders;
            PagerViewModels<Slider> SliderViewModels = new PagerViewModels<Slider>();
            SliderViewModels.CurrentPage = page;
            SliderViewModels.data = sliders.OrderBy(current => current.Id).Skip((page - 1) * 10).Take(10).ToList();
            SliderViewModels.TotalItemCount = sliders.Count();
            return View(SliderViewModels);
        }

        public ActionResult UpdateMe(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var slider = db.Sliders.Find(id);

            if (slider == null)
            {
                return HttpNotFound();
            }
            slider.LastDate = DateTime.Now;
            db.Entry(slider).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Success"] = "به روز رسانی اسلایدر مورد نظر شما با موفقیت انجام شد .";
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
        public ActionResult Create([Bind(Include = "Id,Image,Link,CreateDate,LastDate,IsApp,MobImage")] Slider slider,
            HttpPostedFileBase Image, HttpPostedFileBase MobImage)
        {
            if (ModelState.IsValid)
            {
                #region FileUploading
                if (Image != null && Image.ContentLength > 0)
                {
                    string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/content/images/sliders/", true, Image, Server);
                    slider.Image = smallImagePath;
                }
                if (MobImage != null && MobImage.ContentLength > 0)
                {
                    string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/content/images/sliders/", true, MobImage, Server);
                    slider.MobImage = smallImagePath;
                }
                #endregion FileUploading
                slider.CreateDate = DateTime.Now;
                slider.LastDate = DateTime.Now;
                db.Sliders.Add(slider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(slider);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.Sliders.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        // POST: Administrator/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Image,Link,CreateDate,LastDate,IsApp,MobImage")] Slider slider,
            HttpPostedFileBase Image, HttpPostedFileBase MobImage)
        {
            if (ModelState.IsValid)
            {
                Slider edited = db.Sliders.Find(slider.Id);
                edited.Link = slider.Link;
                edited.IsApp = slider.IsApp;
                #region FileUploading
                if (Image != null && Image.ContentLength > 0)
                {
                    if (slider.Image != null)
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, slider.Image, true, Image, Server);
                        edited.Image = smallImagePath;
                    }
                    else
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/content/images/sliders/", true, Image, Server);
                        edited.Image = smallImagePath;
                    }
                }
                if (MobImage != null && MobImage.ContentLength > 0)
                {
                    if (slider.MobImage != null)
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, slider.Image, true, MobImage, Server);
                        edited.MobImage = smallImagePath;
                    }
                    else
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/content/images/sliders/", true, MobImage, Server);
                        edited.MobImage = smallImagePath;
                    }
                }
                edited.Link = slider.Link;
                #endregion FileUploading
                db.Entry(edited).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(slider);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.Sliders.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Slider slider = db.Sliders.Find(id);
            db.Sliders.Remove(slider);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها در سیستم امکان حذف این اسلایدر وجود ندارد .";
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