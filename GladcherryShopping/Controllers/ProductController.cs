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

        public ActionResult Details(long? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products
                .AsNoTracking()
                .Include(current => current.RelatedProducts)
                .Include(current => current.category)
                .FirstOrDefault(current => current.Id == id.Value);

            if (product == null)
            {
                return HttpNotFound();
            }

            HttpCookie cookie = new HttpCookie("SeenProduct_" + product.Id, "1");
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

            PopulateProductListingViewBags(null, false);
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

            PopulateProductListingViewBags(Category, false);
            PagerViewModels<Product> productViewModels = CreateProductPager(query, page);

            return View("All", productViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertComment([Bind(Include = "Id,Text,DateTime,IsApprove,UserId,ProductId,Email,FullName")] Comment comment, long ProductId, string Email, string FullName)
        {
            bool productExists = db.Products
                .AsNoTracking()
                .Any(current => current.Id == ProductId);

            if (!productExists)
            {
                TempData["error"] = "محصول مورد نظر پیدا نشد .";
                return RedirectToAction("All");
            }

            if (User.Identity.IsAuthenticated)
            {
                string UserId = User.Identity.GetUserId();
                comment.UserId = UserId;
            }

            comment.Id = 0;
            comment.Fullname = FullName;
            comment.Email = Email;
            comment.ProductId = ProductId;
            comment.DateTime = DateTime.Now;
            comment.IsApprove = false;

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

        [HttpGet]
        public ActionResult All(int? id, string Search, int page = 1)
        {
            page = NormalizePage(page);

            IQueryable<Product> query = db.Products
                .AsNoTracking()
                .Where(current => current.SiteFirstImage != null);

            bool showSubCategories = false;

            if (id.HasValue)
            {
                Category category = db.Categories
                    .AsNoTracking()
                    .FirstOrDefault(current => current.Id == id.Value && current.IsBlog == false);

                if (category == null)
                {
                    return HttpNotFound();
                }

                List<Category> subCategories = db.Categories
                    .AsNoTracking()
                    .Where(current => current.ParentId == id.Value && current.IsBlog == false)
                    .OrderByDescending(current => current.PersianName)
                    .ToList();

                showSubCategories = subCategories.Any();

                ViewBag.CategoryId = id.Value;
                ViewBag.CategoryName = category.PersianName;
                ViewBag.SubCategories = subCategories;

                if (!showSubCategories)
                {
                    query = query.Where(current => current.CategoryId == id.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(current =>
                    current.PersianName.Contains(Search) ||
                    current.EnglishName.Contains(Search) ||
                    current.Description.Contains(Search));
            }

            PopulateProductListingViewBags(id, showSubCategories);

            PagerViewModels<Product> productViewModels = showSubCategories
                ? CreateEmptyProductPager(page)
                : CreateProductPager(query, page);

            return View(productViewModels);
        }

        [HttpGet]
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

            PopulateProductListingViewBags(null, false);
            PagerViewModels<Product> productViewModels = CreateProductPager(query, page);

            return View(productViewModels);
        }

        private void PopulateProductListingViewBags(int? categoryId, bool showSubCategories)
        {
            ViewBag.ShowSubCategories = showSubCategories;

            if (categoryId.HasValue)
            {
                ViewBag.CategoryId = categoryId.Value;

                if (ViewBag.CategoryName == null)
                {
                    string categoryName = db.Categories
                        .AsNoTracking()
                        .Where(current => current.Id == categoryId.Value)
                        .Select(current => current.PersianName)
                        .FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(categoryName))
                    {
                        ViewBag.CategoryName = categoryName;
                    }
                }

                if (ViewBag.SubCategories == null)
                {
                    ViewBag.SubCategories = db.Categories
                        .AsNoTracking()
                        .Where(current => current.ParentId == categoryId.Value && current.IsBlog == false)
                        .OrderByDescending(current => current.PersianName)
                        .ToList();
                }
            }

            ViewBag.Categories = db.Categories
                .AsNoTracking()
                .Include(current => current.Products)
                .Where(current => current.Products.Count() > 0 && current.IsBlog == false)
                .OrderByDescending(current => current.PersianName)
                .ToList();

            ViewBag.PopularProducts = db.Products
                .AsNoTracking()
                .Where(current => current.SiteFirstImage != null)
                .OrderByDescending(current => current.CreateDate)
                .Take(6)
                .ToList();
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

        private PagerViewModels<Product> CreateEmptyProductPager(int page)
        {
            PagerViewModels<Product> productViewModels = new PagerViewModels<Product>();
            productViewModels.CurrentPage = page;
            productViewModels.TotalItemCount = 0;
            productViewModels.data = new List<Product>();

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
