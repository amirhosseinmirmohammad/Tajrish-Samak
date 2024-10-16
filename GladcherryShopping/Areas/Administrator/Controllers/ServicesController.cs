using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using Microsoft.AspNet.Identity;
using GladcherryShopping.Models;
using GladCherryShopping.Helpers;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ServicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Services
        public ActionResult Index(int page = 1)
        {
            string userId = User.Identity.GetUserId();
            PagerViewModels<Service> pageviewmodel = new PagerViewModels<Service>();
            pageviewmodel.CurrentPage = page;
            pageviewmodel.data = db.Services.Where(current => current.UserId == userId).Include(current => current.User.Roles).Include(current => current.User).OrderByDescending(current => current.CreateDate).Skip((page - 1) * 10).Take(10).ToList();
            pageviewmodel.TotalItemCount = db.Services.Where(current => current.UserId == userId).Count();
            return View(pageviewmodel);
        }

        public ActionResult Contractors(int page = 1)
        {
            string userId = User.Identity.GetUserId();
            PagerViewModels<Service> pageviewmodel = new PagerViewModels<Service>();
            pageviewmodel.CurrentPage = page;
            pageviewmodel.data = db.Services.Where(current => current.UserId != userId).Include(current => current.User.Roles).Include(current => current.User).OrderByDescending(current => current.CreateDate).Skip((page - 1) * 10).Take(10).ToList();
            pageviewmodel.TotalItemCount = db.Services.Where(current => current.UserId == userId).Count();
            return View("Index", pageviewmodel);
        }

        //public ActionResult Comments(int id, int page = 1)
        //{
        //    var model = db.ServiceComments.Where(current => current.ServiceId == id);
        //    PagerViewModels<ServiceComment> pageviewmodel = new PagerViewModels<ServiceComment>();
        //    pageviewmodel.CurrentPage = page;
        //    pageviewmodel.data = model.OrderByDescending(current => current.DateTime).Skip((page - 1) * 10).Take(10).ToList();
        //    pageviewmodel.TotalItemCount = model.Count();
        //    return View(pageviewmodel);
        //}

        //public ActionResult Approve(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ServiceComment comment = db.ServiceComments.Find(id);
        //    if (comment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    if (comment.IsApprove == false)
        //    {
        //        comment.IsApprove = true;
        //    }
        //    else
        //    {
        //        comment.IsApprove = false;
        //    }
        //    db.Entry(comment).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("index");
        //}

        // GET: Administrator/Services/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service Service = db.Services.Find((int)id);
            if (Service == null)
            {
                return HttpNotFound();
            }
            return View(Service);
        }

        // GET: Administrator/Services/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.ServiceCategories, "Id", "PersianName");
            return View();
        }

        // POST: Administrator/Services/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Survey,Like,DissLike,IsVisible,CreateDate,Title,ShortDesc,Body,SefUrl,MetaKey,MetaDesc,UserId,CategoryId")] Service Service, IEnumerable<HttpPostedFileBase> Images)
        {
            if (ModelState.IsValid)
            {
                Service.Survey = 0;
                Service.Like = 0;
                Service.DissLike = 0;
                Service.UserId = User.Identity.GetUserId();
                Service.CreateDate = DateTime.Now;

                #region Meta Key
                string trim = Service.SefUrl.Trim();
                string remove;
                if (trim.Contains("/"))
                {
                    remove = trim.Replace("/", string.Empty);
                }
                else
                {
                    remove = trim;
                }

                if (trim.Contains(":"))
                {
                    remove = remove.Replace(":", string.Empty);
                }

                if (trim.Contains("."))
                {
                    remove = remove.Replace(".", string.Empty);
                }

                if (trim.Contains("%"))
                {
                    remove = remove.Replace("%", string.Empty);
                }


                if (trim.Contains("@"))
                {
                    remove = remove.Replace("@", string.Empty);
                }

                if (trim.Contains("#"))
                {
                    remove = remove.Replace("#", string.Empty);
                }

                if (trim.Contains("*"))
                {
                    remove = remove.Replace("*", string.Empty);
                }

                if (trim.Contains("!"))
                {
                    remove = remove.Replace("!", string.Empty);
                }

                if (trim.Contains("&"))
                {
                    remove = remove.Replace("&", string.Empty);
                }

                var SefUrl = remove.Replace(" ", "-");
                Service.SefUrl = SefUrl;
                #endregion Meta Key

                db.Services.Add(Service);
                db.SaveChanges();

                #region Image
                foreach (var image in Images)
                {
                    if (image != null && image.ContentLength > 0)
                    {
                        File Newimage = new File();
                        Newimage.ServiceId = Service.Id;
                        Newimage.Extension = image.ContentType;
                        Newimage.Directory = "~/images/Services/";
                        string Path = FunctionsHelper.File(FunctionsHelper.FileMode.Upload, FunctionsHelper.FileType.Image, Newimage.Directory, true, image, Server);
                        if (Path != string.Empty)
                        {
                            Newimage.FullPath = Path;
                        }
                        Newimage.FileName = image.FileName + "." + image.ContentType;
                        Newimage.FileNameWithoutExtension = image.FileName;
                        Newimage.FileSize = image.ContentLength;
                        Newimage.UploadDate = DateTime.Now;
                        db.Files.Add(Newimage);
                        db.SaveChanges();
                    }
                }
                #endregion Image

                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.ServiceCategories, "Id", "PersianName");
            return View(Service);
        }

        // GET: Administrator/Services/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service Service = db.Services.Where(current => current.Id == id).Include(current => current.Images).Include(current => current.User).FirstOrDefault();

            if (Service == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.ServiceCategories, "Id", "PersianName", Service.CategoryId);
            return View(Service);
        }

        // POST: Administrator/Services/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Survey,Like,DissLike,IsVisible,CreateDate,Title,ShortDesc,Body,SefUrl,MetaKey,MetaDesc,UserId,CategoryId")] Service Service, IEnumerable<HttpPostedFileBase> Images)
        {
            if (ModelState.IsValid)
            {

                #region Meta Key
                string trim = Service.SefUrl.Trim();
                string remove;
                if (trim.Contains("/"))
                {
                    remove = trim.Replace("/", string.Empty);
                }
                else
                {
                    remove = trim;
                }

                if (trim.Contains(":"))
                {
                    remove = remove.Replace(":", string.Empty);
                }

                if (trim.Contains("."))
                {
                    remove = remove.Replace(".", string.Empty);
                }

                if (trim.Contains("%"))
                {
                    remove = remove.Replace("%", string.Empty);
                }


                if (trim.Contains("@"))
                {
                    remove = remove.Replace("@", string.Empty);
                }

                if (trim.Contains("#"))
                {
                    remove = remove.Replace("#", string.Empty);
                }

                if (trim.Contains("*"))
                {
                    remove = remove.Replace("*", string.Empty);
                }

                if (trim.Contains("!"))
                {
                    remove = remove.Replace("!", string.Empty);
                }

                if (trim.Contains("&"))
                {
                    remove = remove.Replace("&", string.Empty);
                }

                var SefUrl = remove.Replace(" ", "-");
                Service.SefUrl = SefUrl;
                #endregion Meta Key

                db.Entry(Service).State = EntityState.Modified;
                db.SaveChanges();

                #region Image
                foreach (var image in Images)
                {
                    if (image != null && image.ContentLength > 0)
                    {
                        File Newimage = new File();
                        Newimage.ServiceId = Service.Id;
                        Newimage.Extension = image.ContentType;
                        Newimage.Directory = "~/images/Services/";
                        string Path = FunctionsHelper.File(FunctionsHelper.FileMode.Upload, FunctionsHelper.FileType.Image, Newimage.Directory, true, image, Server);
                        if (Path != string.Empty)
                        {
                            Newimage.FullPath = Path;
                        }
                        Newimage.FileName = image.FileName + "." + image.ContentType;
                        Newimage.FileNameWithoutExtension = image.FileName;
                        Newimage.FileSize = image.ContentLength;
                        Newimage.UploadDate = DateTime.Now;
                        db.Files.Add(Newimage);
                        db.SaveChanges();
                    }
                }
                #endregion Image

                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.ServiceCategories, "Id", "PersianName", Service.CategoryId);
            return View(Service);
        }

        // GET: Administrator/Services/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service Service = db.Services.Find((int)id);
            if (Service == null)
            {
                return HttpNotFound();
            }
            return View(Service);
        }

        // POST: Administrator/Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Service Service = db.Services.Where(current => current.Id == id).Include(current => current.Images).FirstOrDefault();
            db.Services.Remove(Service);
            try
            {
                TempData["Success"] = "خدمات مورد نظر با موفقیت حذف شد .";
                db.SaveChanges();
                if (Service.Images.Count > 0)
                {
                    foreach (var img in Service.Images)
                    {
                        var ImagePath = Server.MapPath(img.FullPath);
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }
                }
            }
            catch (Exception)
            {
                TempData["Error"] = " به دلیل وجود زیر شاخه ها امکان حذف این خدمات وجود ندارد . ";
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
