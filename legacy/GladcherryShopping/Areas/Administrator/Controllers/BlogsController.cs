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
    public class BlogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Blogs
        public ActionResult Index(int page = 1)
        {
            string userId = User.Identity.GetUserId();
            PagerViewModels<Blog> pageviewmodel = new PagerViewModels<Blog>();
            pageviewmodel.CurrentPage = page;
            pageviewmodel.data = db.Blogs.Where(current => current.UserId == userId).Include(current => current.User.Roles).Include(current => current.User).OrderByDescending(current => current.CreateDate).Skip((page - 1) * 10).Take(10).ToList();
            pageviewmodel.TotalItemCount = db.Blogs.Where(current => current.UserId == userId).Count();
            return View(pageviewmodel);
        }

        public ActionResult Contractors(int page = 1)
        {
            string userId = User.Identity.GetUserId();
            PagerViewModels<Blog> pageviewmodel = new PagerViewModels<Blog>();
            pageviewmodel.CurrentPage = page;
            pageviewmodel.data = db.Blogs.Where(current => current.UserId != userId).Include(current => current.User.Roles).Include(current => current.User).OrderByDescending(current => current.CreateDate).Skip((page - 1) * 10).Take(10).ToList();
            pageviewmodel.TotalItemCount = db.Blogs.Where(current => current.UserId == userId).Count();
            return View("Index", pageviewmodel);
        }

        public ActionResult Comments(int id, int page = 1)
        {
            var model = db.BlogComments.Where(current => current.BlogId == id);
            PagerViewModels<BlogComment> pageviewmodel = new PagerViewModels<BlogComment>();
            pageviewmodel.CurrentPage = page;
            pageviewmodel.data = model.OrderByDescending(current => current.DateTime).Skip((page - 1) * 10).Take(10).ToList();
            pageviewmodel.TotalItemCount = model.Count();
            return View(pageviewmodel);
        }

        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogComment comment = db.BlogComments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            if (comment.IsApprove == false)
            {
                comment.IsApprove = true;
            }
            else
            {
                comment.IsApprove = false;
            }
            db.Entry(comment).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("index");
        }

        // GET: Administrator/Blogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find((int)id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Administrator/Blogs/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories.Where(current => current.IsBlog == true), "Id", "PersianName");
            return View();
        }

        // POST: Administrator/Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Survey,Like,DissLike,IsVisible,CreateDate,Title,ShortDesc,Body,SefUrl,MetaKey,MetaDesc,UserId,CategoryId")] Blog blog, IEnumerable<HttpPostedFileBase> Images)
        {
            if (ModelState.IsValid)
            {
                blog.Survey = 0;
                blog.Like = 0;
                blog.DissLike = 0;
                blog.UserId = User.Identity.GetUserId();
                blog.CreateDate = DateTime.Now;

                #region Meta Key
                string trim = blog.SefUrl.Trim();
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
                blog.SefUrl = SefUrl;
                #endregion Meta Key

                db.Blogs.Add(blog);
                db.SaveChanges();

                #region Image
                foreach (var image in Images)
                {
                    if (image != null && image.ContentLength > 0)
                    {
                        File Newimage = new File();
                        Newimage.BlogId = blog.Id;
                        Newimage.Extension = image.ContentType;
                        Newimage.Directory = "~/images/Blogs/";
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
            ViewBag.CategoryId = new SelectList(db.Categories.Where(current => current.IsBlog == true), "Id", "PersianName");
            return View(blog);
        }

        // GET: Administrator/Blogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Where(current => current.Id == id).Include(current => current.Images).Include(current => current.User).FirstOrDefault();

            if (blog == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories.Where(current => current.IsBlog == true), "Id", "PersianName", blog.CategoryId);
            return View(blog);
        }

        // POST: Administrator/Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Survey,Like,DissLike,IsVisible,CreateDate,Title,ShortDesc,Body,SefUrl,MetaKey,MetaDesc,UserId,CategoryId")] Blog blog, IEnumerable<HttpPostedFileBase> Images)
        {
            if (ModelState.IsValid)
            {

                #region Meta Key
                string trim = blog.SefUrl.Trim();
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
                blog.SefUrl = SefUrl;
                #endregion Meta Key

                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();

                #region Image
                foreach (var image in Images)
                {
                    if (image != null && image.ContentLength > 0)
                    {
                        File Newimage = new File();
                        Newimage.BlogId = blog.Id;
                        Newimage.Extension = image.ContentType;
                        Newimage.Directory = "~/images/Blogs/";
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
            ViewBag.CategoryId = new SelectList(db.Categories.Where(current => current.IsBlog == true), "Id", "PersianName", blog.CategoryId);
            return View(blog);
        }

        // GET: Administrator/Blogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find((int)id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Administrator/Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = db.Blogs.Where(current => current.Id == id).Include(current => current.Images).FirstOrDefault();
            db.Blogs.Remove(blog);
            try
            {
                TempData["Success"] = "بلاگ مورد نظر با موفقیت حذف شد .";
                db.SaveChanges();
                if (blog.Images.Count > 0)
                {
                    foreach (var img in blog.Images)
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
                TempData["Error"] = " به دلیل وجود زیر شاخه ها امکان حذف این بلاگ وجود ندارد . ";
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
