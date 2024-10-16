using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataLayer.Models;
using GladcherryShopping.Models;
using System;
using DataLayer.ViewModels.PagerViewModel;
using System.Web;
using GladCherryShopping.Helpers;
using static GladCherryShopping.Helpers.FunctionsHelper;
using System.Collections.Generic;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class HearingAidsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Products
        public ActionResult Index(int page = 1)
        {
            var products = db.Products.Include(c => c.category);
            PagerViewModels<Product> ProductViewModels = new PagerViewModels<Product>();
            ProductViewModels.CurrentPage = page;
            ProductViewModels.data = products.OrderByDescending(current => current.CreateDate).Skip((page - 1) * 10).Take(10).ToList();
            ProductViewModels.TotalItemCount = products.Count();
            return View(ProductViewModels);
        }

        [HttpGet]
        public ActionResult Price()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Price(bool? IsIncrease, bool? IsPercent, int? Amount, int? Percent)
        {
            if (Amount == null && Percent == null)
            {
                TempData["Error"] = "لطفا مقدار یا درصد کاهش و افزایش را وارد نمایید .";
                return View();
            }
            if (Amount != null & Percent != null)
            {
                TempData["Error"] = "لطفا یا مقدار را وارد نمایید یا درصد .";
                return View();
            }
            if (IsIncrease == true)
            {
                if (IsPercent == true)
                {
                    foreach (var item in db.Products.ToList())
                    {
                        int percentAmount = ((int)Percent * item.UnitPrice) / 100;
                        item.UnitPrice += percentAmount;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    foreach (var item in db.Products.ToList())
                    {
                        item.UnitPrice += (int)Amount;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                if (IsPercent == true)
                {
                    foreach (var item in db.Products.ToList())
                    {
                        int percentAmount = ((int)Percent * item.UnitPrice) / 100;
                        item.UnitPrice -= percentAmount;
                        if (item.UnitPrice > 0)
                        {
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    foreach (var item in db.Products.ToList())
                    {
                        item.UnitPrice -= (int)Amount;
                        if (item.UnitPrice > 0)
                        {
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            TempData["Success"] = "تغییرات مد نظر شما در قیمت ها با موفقیت اعمال شد .";
            return RedirectToAction("Index");
        }

        // GET: Administrator/Products/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Administrator/Products/Create
        public ActionResult Create()
        {
            var products = db.Products;
            ViewBag.Related = new MultiSelectList(products, "Id", "FullName");
            ViewBag.CategoryId = new SelectList(db.Categories.Where(current => current.IsBlog == false), "Id", "PersianName");
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "PersianName");
            return View();
        }

        // POST: Administrator/Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PersianName,EnglishName,UnitPrice,Stock,DiscountPercent,Description,CategoryId,SliderImage,AppSmallImage,AppLargeImage,SiteFirstImage,SiteSecondImage,SiteThirdImage,CreateDate,IsSpecial,BrandId")] Product product,
            HttpPostedFileBase SliderImage, HttpPostedFileBase AppSmallImage, HttpPostedFileBase AppLargeImage, HttpPostedFileBase SiteFirstImage, HttpPostedFileBase SiteSecondImage, HttpPostedFileBase SiteThirdImage, IEnumerable<int> Related)
        {
            var products = db.Products;
            ViewBag.Related = new MultiSelectList(products, "Id", "FullName");
            if (ModelState.IsValid)
            {
                {
                    #region FileUploading
                    if (SliderImage != null && SliderImage.ContentLength > 0)
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, SliderImage, Server);
                        product.SliderImage = smallImagePath;
                    }
                    if (AppSmallImage != null && AppSmallImage.ContentLength > 0)
                    {
                        string largeImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, AppSmallImage, Server);
                        product.AppSmallImage = largeImagePath;
                    }
                    if (AppLargeImage != null && AppLargeImage.ContentLength > 0)
                    {
                        string backgroundImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, AppLargeImage, Server);
                        product.AppLargeImage = backgroundImagePath;
                    }
                    if (SiteFirstImage != null && SiteFirstImage.ContentLength > 0)
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, SiteFirstImage, Server);
                        product.SiteFirstImage = smallImagePath;
                    }
                    if (SiteSecondImage != null && SiteSecondImage.ContentLength > 0)
                    {
                        string largeImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, SiteSecondImage, Server);
                        product.SiteSecondImage = largeImagePath;
                    }
                    if (SiteThirdImage != null && SiteThirdImage.ContentLength > 0)
                    {
                        string backgroundImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, SiteThirdImage, Server);
                        product.SiteThirdImage = backgroundImagePath;
                    }
                    #endregion FileUploading
                    product.CreateDate = DateTime.Now;
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.CategoryId = new SelectList(db.Categories.Where(current => current.IsBlog == false), "Id", "PersianName", product.CategoryId);
                ViewBag.BrandId = new SelectList(db.Brands, "Id", "PersianName",product.BrandId);
                return RedirectToAction("Index");
            }
        }

        // GET: Administrator/Products/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Where(current => current.Id == id).Include(current => current.RelatedProducts).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories.Where(current => current.IsBlog == false), "Id", "PersianName", product.CategoryId);
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "PersianName", product.BrandId);
            var products = db.Products;
            ViewBag.Related = new MultiSelectList(products, "Id", "FullName", product.RelatedProducts.Select(current => current.Id));
            return View(product);
        }

        // POST: Administrator/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PersianName,EnglishName,UnitPrice,Stock,DiscountPercent,Description,CategoryId,SliderImage,AppSmallImage,AppLargeImage,SiteFirstImage,SiteSecondImage,SiteThirdImage,CreateDate,IsSpecial,BrandId")] Product product,
            HttpPostedFileBase SliderImage, HttpPostedFileBase AppSmallImage, HttpPostedFileBase AppLargeImage, HttpPostedFileBase SiteFirstImage, HttpPostedFileBase SiteSecondImage, HttpPostedFileBase SiteThirdImage, IEnumerable<int> Related)
        {
            var products = db.Products;
            ViewBag.Related = new MultiSelectList(products, "Id", "FullName", product.RelatedProducts.Select(current => current.Id));
            if (ModelState.IsValid)
            {

                Product editedProduct = db.Products.Where(current => current.Id == product.Id).Include(current => current.RelatedProducts).FirstOrDefault();
                editedProduct.CategoryId = product.CategoryId;
                editedProduct.Description = product.Description;
                editedProduct.DiscountPercent = product.DiscountPercent;
                editedProduct.EnglishName = product.EnglishName;
                editedProduct.IsSpecial = product.IsSpecial;
                editedProduct.PersianName = product.PersianName;
                editedProduct.Stock = product.Stock;
                editedProduct.UnitPrice = product.UnitPrice;

                #region FileUploading 
                //if (SliderImage != null && SliderImage.ContentLength > 0)
                //{
                //    if (editedProduct.SliderImage != null)
                //    {
                //        string smallImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, product.SliderImage, true, SliderImage, Server);
                //        editedProduct.SliderImage = smallImagePath;
                //    }
                //    else
                //    {
                //        string SliderImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, SliderImage, Server);
                //        editedProduct.SliderImage = SliderImagePath;
                //    }
                //}
                //if (AppSmallImage != null && AppSmallImage.ContentLength > 0)
                //{
                //    if (editedProduct.AppSmallImage != null)
                //    {
                //        string AppSmallImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, product.AppSmallImage, true, AppSmallImage, Server);
                //        editedProduct.AppSmallImage = AppSmallImagePath;
                //    }
                //    else
                //    {
                //        string largeImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, AppSmallImage, Server);
                //        editedProduct.AppSmallImage = largeImagePath;
                //    }
                //}
                //if (AppLargeImage != null && AppLargeImage.ContentLength > 0)
                //{
                //    if (editedProduct.AppLargeImage != null)
                //    {
                //        string AppLargeImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, product.AppLargeImage, true, AppLargeImage, Server);
                //        editedProduct.AppLargeImage = AppLargeImagePath;
                //    }
                //    else
                //    {
                //        string backgroundImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, AppLargeImage, Server);
                //        editedProduct.AppLargeImage = backgroundImagePath;
                //    }
                //}
                if (SiteFirstImage != null && SiteFirstImage.ContentLength > 0)
                {
                    if (editedProduct.SiteFirstImage != null)
                    {
                        string SiteFirstImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, product.SiteFirstImage, true, SiteFirstImage, Server);
                        editedProduct.SiteFirstImage = SiteFirstImagePath;
                    }
                    else
                    {
                        string SiteFirstImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, SiteFirstImage, Server);
                        editedProduct.SiteFirstImage = SiteFirstImagePath;
                    }
                }
                if (SiteSecondImage != null && SiteSecondImage.ContentLength > 0)
                {
                    if (editedProduct.SiteSecondImage != null)
                    {
                        string SiteSecondImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, product.SiteSecondImage, true, SiteSecondImage, Server);
                        editedProduct.SiteSecondImage = SiteSecondImagePath;
                    }
                    else
                    {
                        string SiteSecondImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, SiteSecondImage, Server);
                        editedProduct.SiteSecondImage = SiteSecondImagePath;
                    }
                }
                if (SiteThirdImage != null && SiteThirdImage.ContentLength > 0)
                {
                    if (editedProduct.SiteThirdImage != null)
                    {
                        string SiteThirdImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, product.SiteThirdImage, true, SiteThirdImage, Server);
                        editedProduct.SiteThirdImage = SiteThirdImagePath;
                    }
                    else
                    {
                        string SiteThirdImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Products/", true, SiteThirdImage, Server);
                        editedProduct.SiteThirdImage = SiteThirdImagePath;
                    }
                }
                #endregion FileUploading

                if (Related != null)
                {
                    editedProduct.RelatedProducts.Clear();
                    foreach (var item in Related)
                    {
                        Product prod = db.Products.Find(item);
                        if (prod != null)
                        {
                            editedProduct.RelatedProducts.Add(prod);
                        }
                    }
                }

                else
                {
                    editedProduct.RelatedProducts.Clear();
                }

                db.Entry(editedProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "PersianName", product.BrandId);
            ViewBag.CategoryId = new SelectList(db.Categories.Where(current => current.IsBlog == false), "Id", "PersianName", product.CategoryId);
            return RedirectToAction("Index");
        }

        // GET: Administrator/Products/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Administrator/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            try
            {
                db.SaveChanges();
                TempData["Success"] = "محصول مورد نظر با موفقیت از سیستم حذف شد .";
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها امکان حذف این محصول از سیستم وجود ندارد .";
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
