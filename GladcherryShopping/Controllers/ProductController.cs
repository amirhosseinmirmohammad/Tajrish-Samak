using DataLayer.Models;
using DataLayer.ViewModels;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class ProductController : Controller
    {
        private const int ProductPageSize = 6;
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: Product
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products
                .AsNoTracking()
                .Where(current => current.Id == id)
                .Include(current => current.RelatedProducts)
                .Include(current => current.category)
                .FirstOrDefault();
            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cookie = new HttpCookie("SeenProduct_" + product.Id.ToString(), 1.ToString());
            cookie.Expires = DateTime.Now.AddMonths(1);
            cookie.HttpOnly = true;
            Response.Cookies.Add(cookie);
            ProductDetailViewModel viewmodel = new ProductDetailViewModel();
            viewmodel.product = product;
            return View(viewmodel);
        }

        [HttpGet]
        public ActionResult Search(string Search, int page = 1)
        {
            page = NormalizePage(page);

            IQueryable<Product> query = db.Products
                .AsNoTracking()
                .Where(current => current.SiteFirstImage != null);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(current =>
                    current.PersianName.Contains(Search) ||
                    current.EnglishName.Contains(Search) ||
                    current.Description.Contains(Search));
            }

            PagerViewModels<Product> productViewModels = CreateProductPager(query, page);
            return View("All", productViewModels);
        }

        [HttpGet]
        public ActionResult Filter(string Title, byte? Discount, int? Category, int? Min, int? Max, int page = 1)
        {
            page = NormalizePage(page);

            IQueryable<Product> query = db.Products
                .AsNoTracking()
                .Where(current => current.SiteFirstImage != null);

            if (!string.IsNullOrWhiteSpace(Title))
            {
                query = query.Where(current =>
                    current.PersianName.Contains(Title) ||
                    current.Description.Contains(Title) ||
                    current.EnglishName.Contains(Title));
            }

            if (Category.HasValue)
            {
                query = query.Where(current => current.CategoryId == Category.Value);
            }

            if (Discount.HasValue)
            {
                query = query.Where(current => current.DiscountPercent >= Discount.Value);
            }

            if (Min.HasValue)
            {
                query = query.Where(current => current.UnitPrice >= Min.Value);
            }

            if (Max.HasValue)
            {
                query = query.Where(current => current.UnitPrice <= Max.Value);
            }

            PagerViewModels<Product> productViewModels = CreateProductPager(query, page);
            return View("All", productViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertComment([Bind(Include = "Id,Text,DateTime,IsApprove,UserId,ProductId,Email,FullName")] Comment comment, long ProductId, string Email, string FullName)
        {
            if (User.Identity.IsAuthenticated)
            {
                string UserId = User.Identity.GetUserId();
                comment.UserId = UserId;
            }
            comment.Fullname = FullName;
            comment.Email = Email;
            comment.ProductId = ProductId;
            comment.DateTime = DateTime.Now;
            db.Comments.Add(comment);
            try
            {
                db.SaveChanges();
                TempData["Success"] = "نظر شما با موفقیت ثبت شد و در انتظار تایید ادمین میباشد .";
            }
            catch (Exception)
            {
                TempData["error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .";
            }
            return RedirectToAction("Details", new { id = comment.ProductId });
        }

        public ActionResult All(string Search, int page = 1)
        {
            page = NormalizePage(page);

            IQueryable<Product> query = db.Products
                .AsNoTracking()
                .Where(current => current.SiteFirstImage != null);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(current =>
                    current.PersianName.Contains(Search) ||
                    current.EnglishName.Contains(Search) ||
                    current.Description.Contains(Search));
            }

            PagerViewModels<Product> productViewModels = CreateProductPager(query, page);
            return View(productViewModels);
        }

        public ActionResult Special(string Search, int page = 1)
        {
            page = NormalizePage(page);

            IQueryable<Product> query = db.Products
                .AsNoTracking()
                .Where(current => current.SiteFirstImage != null && current.IsSpecial == true);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(current =>
                    current.PersianName.Contains(Search) ||
                    current.EnglishName.Contains(Search) ||
                    current.Description.Contains(Search));
            }

            PagerViewModels<Product> productViewModels = CreateProductPager(query, page);
            return View(productViewModels);
        }

        private PagerViewModels<Product> CreateProductPager(IQueryable<Product> query, int page)
        {
            PagerViewModels<Product> productViewModels = new PagerViewModels<Product>();
            productViewModels.CurrentPage = page;
            productViewModels.TotalItemCount = query.Count();
            productViewModels.data = query
                .OrderByDescending(current => current.CreateDate)
                .ThenByDescending(current => current.PersianName)
                .Skip((page - 1) * ProductPageSize)
                .Take(ProductPageSize)
                .ToList();

            return productViewModels;
        }

        private int NormalizePage(int page)
        {
            return page < 1 ? 1 : page;
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