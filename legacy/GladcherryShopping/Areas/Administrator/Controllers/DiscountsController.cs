using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataLayer.Models;
using System;
using DataLayer.ViewModels.PagerViewModel;
using System.Web;
using System.Collections.Generic;
using GladcherryShopping.Models;
using GladCherryShopping.Helpers;
using static GladCherryShopping.Helpers.FunctionsHelper;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DiscountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Discounts
        public ActionResult Index(int page = 1)
        {
            var discounts = db.Discounts;
            PagerViewModels<Discount> DiscountViewModels = new PagerViewModels<Discount>();
            DiscountViewModels.CurrentPage = page;
            DiscountViewModels.data = discounts.OrderByDescending(current => current.Title).Skip((page - 1) * 10).Take(10).ToList();
            DiscountViewModels.TotalItemCount = discounts.Count();
            return View(DiscountViewModels);
        }

        public ActionResult Situation(int? id)
        {
            foreach (var item in db.Discounts.Where(current => current.IsActived == true && current.ExpireDate <= DateTime.Now).ToList())
            {
                item.IsActived = false;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            foreach (var item in db.Discounts.Where(current => current.IsActived == true && current.ShowcaseDate <= DateTime.Now).ToList())
            {
                item.IsPublic = false;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            TempData["Success"] = "کدهای تخفیف با موفقیت تعیین وضعیت شدند .";
            return RedirectToAction("Index");
        }
        // GET: Administrator/Discounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Discount discount = db.Discounts.Find(id);
            if (discount == null)
            {
                return HttpNotFound();
            }
            return View(discount);
        }

        // GET: Administrator/Discounts/Create
        public ActionResult Create()
        {
            var cats = db.Categories.Include(current => current.Parent).Where(current => current.Parent.ParentId == null);
            ViewBag.CategoryId = new MultiSelectList(cats, "Id", "PersianName");
            return View();
        }

        // POST: Administrator/Discounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Title,Percent,Count,MaxCount,IsActived,IsPercentage,Amount,MaxOrder,CategoryId,IsPublic,ExDate,DiscountPercent,DiscountCount,IsRepeated,Image,ShDate,IntroductionCount")] Discount discount, HttpPostedFileBase Image)
        {
            var cats = db.Categories.Include(current => current.Parent).Where(current => current.Parent.ParentId == null);
            ViewBag.CategoryId = new MultiSelectList(cats, "Id", "PersianName");
            IQueryable<Discount> query = db.Discounts.Where(currrent => currrent.Code == discount.Code);
            if (query.Count() > 0)
            {
                TempData["Error"] = "کد مورد نظر قبلا در سیستم ثبت شده است .";
                return View(discount);
            }
            Discount exist = db.Discounts.Where(current => current.IntroductionCount != null && current.IntroductionCount > 0).FirstOrDefault();
            if (discount.IntroductionCount != null && discount.IntroductionCount > 0 && exist != null)
            {
                TempData["Error"] = "شما قبلا کد تخفیفی برای تعداد معرفی اپلیکیشن با عنوان " + exist.Title + " در سامانه وارد کردید لطفا اگر قصد قرار دادن کد تخفیف جدید برای معرفی اپلیکیشن دارید تخفیف برای فیلد 'تعداد معرفی مشخص اپلیکیشنِ' کد قبلی را خالی و یا صفر کنید . ";
                ViewBag.Edit1 = 1;
                ViewBag.Id = exist.Id;
                return View(discount);
            }
            Discount exist2 = db.Discounts.Where(current => current.DiscountCount != null && current.DiscountCount > 0 && current.DiscountCount == discount.DiscountCount).FirstOrDefault();
            if (discount.DiscountCount != null && discount.DiscountCount > 0 && exist2 != null)
            {
                TempData["Error"] = "شما قبلا کد تخفیفی برای تعداد سفارش مشخص با عنوان " + exist2.Title + " در سامانه وارد کردید لطفا اگر قصد قرار دادن کد تخفیف جدید برای تعداد سفارش مشخص دارید فیلد 'تخفیف برای تعداد سفارش مشخص' کد قبلی را خالی و یا صفر کنید . ";
                ViewBag.Edit1 = 1;
                ViewBag.Id = exist2.Id;
                return View(discount);
            }
            if (discount.Percent == null && string.IsNullOrEmpty(discount.Amount))
            {
                TempData["Error"] = "شما باید درصد و یا مقدار تخفیف را وارد نمایید .";
                return View(discount);
            }
            if (ModelState.IsValid)
            {
                discount.Count = 0;
                if (discount.ExDate != null)
                {
                    discount.ExpireDate = FunctionsHelper.ConvertToGregorian(discount.ExDate);
                }
                if (discount.ShDate != null)
                {
                    discount.ShowcaseDate = FunctionsHelper.ConvertToGregorian(discount.ShDate);
                }

                #region FileUploading
                if (Image != null && Image.ContentLength > 0)
                {
                    string ImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/content/images/Discounts/", true, Image, Server);
                    discount.Image = ImagePath;
                }
                #endregion FileUploading

                db.Discounts.Add(discount);
                db.SaveChanges();
                return Redirect("/Administrator/Discounts/Index");
            }
            return View(discount);
        }

        // GET: Administrator/Discounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Discount discount = db.Discounts.Where(current => current.Id == id).FirstOrDefault();
            if (discount == null)
            {
                return HttpNotFound();
            }
            var cats = db.Categories.Include(current => current.Parent).Where(current => current.Parent.ParentId == null);
            return View(discount);
        }

        // POST: Administrator/Discounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Title,Percent,Count,MaxCount,IsActived,IsPercentage,Amount,MaxOrder,CategoryId,IsPublic,ExDate,DiscountPercent,DiscountCount,IsRepeated,Image,ShDate,IntroductionCount")] Discount discount, HttpPostedFileBase Image)
        {
            var cats = db.Categories.Include(current => current.Parent).Where(current => current.Parent.ParentId == null);
            IQueryable<Discount> query = db.Discounts.Where(currrent => currrent.Code == discount.Code && currrent.Id != discount.Id);
            if (query.Count() > 0)
            {
                TempData["Error"] = "کد مورد ویرایش قبلا در سیستم ثبت شده است .";
                return View(discount);
            }
            Discount exist = db.Discounts.Where(current => current.IntroductionCount != null && current.IntroductionCount > 0 && current.Id != discount.Id).FirstOrDefault();
            if (discount.IntroductionCount != null && discount.IntroductionCount > 0 && exist != null)
            {
                TempData["Error"] = "شما قبلا کد تخفیفی برای تعداد معرفی اپلیکیشن با عنوان " + exist.Title + " در سامانه وارد کردید لطفا اگر قصد قرار دادن کد تخفیف جدید برای معرفی اپلیکیشن دارید تخفیف برای فیلد 'تعداد معرفی مشخص اپلیکیشنِ' کد قبلی را خالی و یا صفر کنید . ";
                ViewBag.Edit1 = 1;
                ViewBag.Id = exist.Id;
                return View(discount);
            }
            Discount exist2 = db.Discounts.Where(current => current.DiscountCount != null && current.DiscountCount > 0 && current.DiscountCount == discount.DiscountCount && current.Id != discount.Id).FirstOrDefault();
            if (discount.DiscountCount != null && discount.DiscountCount > 0 && exist2 != null)
            {
                TempData["Error"] = "شما قبلا کد تخفیفی برای تعداد سفارش مشخص با عنوان " + exist2.Title + " در سامانه وارد کردید لطفا اگر قصد قرار دادن کد تخفیف جدید برای تعداد سفارش مشخص دارید فیلد 'تخفیف برای تعداد سفارش مشخص' کد قبلی را خالی و یا صفر کنید . ";
                ViewBag.Edit1 = 1;
                ViewBag.Id = exist2.Id;
                return View(discount);
            }
            if (discount.Percent == null && string.IsNullOrEmpty(discount.Amount))
            {
                TempData["Error"] = "شما باید درصد و یا مقدار تخفیف را وارد نمایید .";
                return View(discount);
            }
            if (ModelState.IsValid)
            {
                var edittedDiscount = db.Discounts.Where(current => current.Id == discount.Id).FirstOrDefault();

                if (discount.ExDate != null)
                {
                    edittedDiscount.ExpireDate = ConvertToGregorian(discount.ExDate);
                }
                if (discount.ShDate != null)
                {
                    discount.ShowcaseDate = ConvertToGregorian(discount.ShDate);
                }
                edittedDiscount.Amount = discount.Amount;
                edittedDiscount.Code = discount.Code;
                edittedDiscount.Count = discount.Count;
                edittedDiscount.DiscountCount = discount.DiscountCount;
                edittedDiscount.DiscountPercent = discount.DiscountPercent;
                edittedDiscount.IsActived = discount.IsActived;
                edittedDiscount.IsPercentage = discount.IsPercentage;
                edittedDiscount.IsPublic = discount.IsPublic;
                edittedDiscount.IsRepeated = discount.IsRepeated;
                edittedDiscount.MaxCount = discount.MaxCount;
                edittedDiscount.MaxOrder = discount.MaxOrder;
                edittedDiscount.Percent = discount.Percent;
                edittedDiscount.Title = discount.Title;
                edittedDiscount.IntroductionCount = discount.IntroductionCount;

                #region FileUpdating

                if (Image != null && Image.ContentLength > 0)
                {
                    if (discount.Image != null)
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Update, FileType.Image, discount.Image, true, Image, Server);
                        edittedDiscount.Image = smallImagePath;
                    }
                    else
                    {
                        string smallImagePath = FunctionsHelper.File(FileMode.Upload, FileType.Image, "~/images/Discounts/", true, Image, Server);
                        edittedDiscount.Image = smallImagePath;
                    }
                }

                #endregion FileUpdating

                db.Entry(edittedDiscount).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("/Administrator/Discounts/Index");
            }
            return View(discount);
        }

        // GET: Administrator/Discounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Discount discount = db.Discounts.Find(id);
            if (discount == null)
            {
                return HttpNotFound();
            }
            return View(discount);
        }

        // POST: Administrator/Discounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Discount discount = db.Discounts.Find(id);
            db.Discounts.Remove(discount);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها در سیستم امکان حذف این کد تخفیف وجود ندارد .";
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
