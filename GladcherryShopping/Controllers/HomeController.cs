using DataLayer.Models;
using DataLayer.ViewModels;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitMessage([Bind(Include = "Id,FullName,Body,Email")] SiteMessage model)
        {
            if (ModelState.IsValid)
            {
                model.RegisterDate = DateTime.Now;
                db.SiteMessages.Add(model);
                db.SaveChanges();
                TempData["Success"] = "پیام شما با موفقیت در سیستم ثبت گردید";
                return RedirectToAction("Index", "ContactUs");
            }
            TempData["Error"] = "متاسفانه خطایی رخ داده است لطفا اطلاعات خود را بررسی و مجدد تلاش نمایید";
            return RedirectToAction("Index", "ContactUs");
        }

        [HttpGet]
        public JsonResult NewsLetter([Bind(Include = "Email")] string text, NewsLetter newsletter)
        {
            if (text == string.Empty || text == null)
            {
                return Json(new { text = "لطفا ایمیل خود را وارد نمایید .", status = 0 }, JsonRequestBehavior.AllowGet);
            }
            if (!new EmailAddressAttribute().IsValid(text))
            {
                return Json(new { text = "لطفا ایمیل معتبری را وارد نمایید .", status = 0 }, JsonRequestBehavior.AllowGet);
            }

            var email = db.NewsLetters.Where(n => n.Email == text).FirstOrDefault();
            if (email != null)
            {
                return Json(new { text = "ایمیل شما قبلا در سیستم ثبت شده است .", status = 0 }, JsonRequestBehavior.AllowGet);
            }

            newsletter.Email = text;
            db.NewsLetters.Add(newsletter);
            try
            {
                db.SaveChanges();
                return Json(new { text = "ایمیل شما با موفقیت در سیستم ثبت شد .", status = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .", status = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetDiscount(string Code)
        {

            #region Validation
            if (string.IsNullOrEmpty(Code))
            {
                return Json(new { Status = false, Text = "لطفا کد خود را وارد نمایید ." }, JsonRequestBehavior.AllowGet);
            }
            IQueryable<Discount> query = db.Discounts.Where(currrent => currrent.Code == Code && currrent.IsActived == true);
            if (query.Count() == 0)
            {
                return Json(new { Status = false, Text = "کد وارد شده معتبر نمیباشد ." }, JsonRequestBehavior.AllowGet);
            }
            if (query.Count() == 1 && query.FirstOrDefault().IsActived == false)
            {
                return Json(new { Status = false, Text = "کد وارد شده غیر فعال شده است ." }, JsonRequestBehavior.AllowGet);
            }
            Discount dis = query.FirstOrDefault();
            if (dis == null)
            {
                return Json(new { Status = false, Text = "کد تخفیف مورد نظر شما پیدا نشد ." }, JsonRequestBehavior.AllowGet);
            }
            if (dis.Count == dis.MaxCount)
            {
                if (dis.IsActived == true)
                {
                    dis.IsActived = false;
                    db.Entry(dis).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { Status = false, Text = "ظرفیت استفاده از این کد به پایان رسیده است ." }, JsonRequestBehavior.AllowGet);
            }
            #endregion Validation

            #region Login
            //اگر عضو بود و ورود کرده بود
            if (User.Identity.IsAuthenticated == true)
            {
                string userId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { Status = false, Text = "کد کاربری شما وجود ندارد ." }, JsonRequestBehavior.AllowGet);
                }
                ApplicationUser user = db.Users.Where(current => current.Id == userId).Include(current => current.Discounts).FirstOrDefault();
                if (user == null)
                {
                    return Json(new { Status = false, Text = "حساب کاربری شما پیدا نشد ." }, JsonRequestBehavior.AllowGet);
                }
                if (user.Discounts.Contains(dis) || Request.Cookies.AllKeys.Contains("Discount_" + dis.Id.ToString()))
                {
                    return Json(new { Status = false, Text = "شما قبلا از این کد تخفیف استفاده کرده اید ." }, JsonRequestBehavior.AllowGet);
                }

                #region Cookie
                //Add Cookie
                var cookie = new HttpCookie("Discount_" + dis.Id.ToString(), dis.Percent.ToString());
                cookie.Expires = DateTime.Now.AddMonths(1);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
                #endregion Cookie

                user.Discounts.Add(dis);
                db.Entry(user).State = EntityState.Modified;
            }
            #endregion Login

            else
            {
                #region Guest
                if (Request.Cookies.AllKeys.Contains("Discount_" + dis.Id.ToString()))
                {
                    return Json(new { Status = false, Text = "شما قبلا از این کد تخفیف استفاده کرده اید ." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region Cookie
                    //Add Cookie
                    var cookie = new HttpCookie("Discount_" + dis.Id.ToString(), dis.Percent.ToString());
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                    #endregion Cookie
                }
                #endregion Guest
            }

            dis.Count++;
            db.Entry(dis).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                var discountPrice = double.Parse(CartPrice()) - double.Parse(CartPrice()) * (dis.Percent) / 100;
                return Json(new { Status = true, PriceHtml = discountPrice, Percent = dis.Percent, Text = "تخفیف شما با مقدار " + dis.Percent + " درصد بر روی سفارش شما اعمال شد ." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Status = false, Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید ." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Services(int? id, int page = 1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceCategory category = db.ServiceCategories.Where(current => current.Id == id).FirstOrDefault();
            if (category == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var blog = new List<Service>();
            blog = db.Services.Where(current => current.Images.Count > 0 && current.IsVisible == true && current.CategoryId == id).Include(current => current.Images).ToList();
            if (blog.Count() == 0)
            {
                TempData["NotFound"] = "هنوز خدماتی در این گروه وجود ندارد .";
            }
            PagerViewModels<Service> BlogViewModels = new PagerViewModels<Service>();
            BlogViewModels.data = blog.OrderByDescending(current => current.CreateDate).Skip((page - 1) * 16).Take(16).ToList();
            BlogViewModels.CurrentPage = page;
            BlogViewModels.TotalItemCount = blog.Count();
            return View(BlogViewModels);
        }

        [HttpGet]
        public ActionResult Blogs(int? id, int page = 1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Where(current => current.Id == id).FirstOrDefault();
            if (category == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var blog = new List<Blog>();
            blog = db.Blogs.Where(current => current.Images.Count > 0 && current.IsVisible == true && current.CategoryId == id).Include(current => current.Images).ToList();
            if (blog.Count() == 0)
            {
                TempData["NotFound"] = "هنوز مطلبی در این گروه وجود ندارد .";
            }
            PagerViewModels<Blog> BlogViewModels = new PagerViewModels<Blog>();
            BlogViewModels.data = blog.OrderByDescending(current => current.CreateDate).Skip((page - 1) * 16).Take(16).ToList();
            BlogViewModels.CurrentPage = page;
            BlogViewModels.TotalItemCount = blog.Count();
            return View(BlogViewModels);
        }

        public ActionResult Products(int? id, int page = 1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Where(current => current.Id == id).Include(current => current.SubCategories).FirstOrDefault();
            if (category == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                PagerViewModels<Product> ProductViewModels = new PagerViewModels<Product>();
                ViewBag.CategoryName = category.PersianName.ToString();
                if (category.SubCategories.Count() > 0)
                {
                    ViewBag.CategoryId = category.Id;
                }
                else
                {
                    var products = db.Products.Where(current => current.CategoryId == id);
                    ProductViewModels.CurrentPage = page;
                    ProductViewModels.data = products.OrderByDescending(current => current.CreateDate).ThenByDescending(current => current.PersianName).Skip((page - 1) * 12).Take(12).ToList();
                    ProductViewModels.TotalItemCount = products.Count();
                }
                return View(ProductViewModels);
            }
        }

        public ActionResult Brands(int? id, int page = 1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand category = db.Brands.Where(current => current.Id == id).Include(current => current.SubCategories).FirstOrDefault();
            if (category == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                PagerViewModels<Product> ProductViewModels = new PagerViewModels<Product>();
                ViewBag.CategoryName = category.PersianName.ToString();
                if (category.SubCategories.Count() > 0)
                {
                    ViewBag.CategoryId = category.Id;
                }
                else
                {
                    var products = db.Products.Where(current => current.BrandId == id);
                    ProductViewModels.CurrentPage = page;
                    ProductViewModels.data = products.OrderByDescending(current => current.CreateDate).ThenByDescending(current => current.PersianName).Skip((page - 1) * 12).Take(12).ToList();
                    ProductViewModels.TotalItemCount = products.Count();
                }
                return View(ProductViewModels);
            }
        }


        public JsonResult AddToShoppingCart(int Id)
        {
            try
            {
                if (Request.Cookies.AllKeys.Contains("Cart_" + Id.ToString()))
                {
                    //Cookie Editing
                    var cookie = new HttpCookie("Cart_" + Id.ToString(), (Convert.ToByte(Request.Cookies["Cart_" + Id.ToString()].Value) + 1).ToString());
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    cookie.HttpOnly = true;
                    Response.Cookies.Set(cookie);
                }
                else
                {
                    //Add Cookie
                    var cookie = new HttpCookie("Cart_" + Id.ToString(), 1.ToString());
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                }
                return Json(new CardViewModel()
                {
                    Success = true,
                    CountHtml = CartCount(),
                    PriceHtml = CartPrice()
                });
            }
            catch (Exception)
            {
                return Json(new CardViewModel()
                {
                    Success = false,
                    CountHtml = "",
                    PriceHtml = ""
                });
            }
        }

        //merchantcode f253a3fe-9add-11e8-99b4-005056a205be

        public JsonResult MinusFromShoppingCart(int Id)
        {
            try
            {
                if (Request.Cookies.AllKeys.Contains("Cart_" + Id.ToString()))
                {
                    //Cookie Editing
                    var cookie = new HttpCookie("Cart_" + Id.ToString(), (Convert.ToByte(Request.Cookies["Cart_" + Id.ToString()].Value) - 1).ToString());
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    cookie.HttpOnly = true;
                    Response.Cookies.Set(cookie);
                }
                return Json(new CardViewModel()
                {
                    Success = true,
                    CountHtml = CartCount(),
                    PriceHtml = CartPrice()
                });
            }
            catch (Exception)
            {
                return Json(new CardViewModel()
                {
                    Success = false,
                    CountHtml = "",
                    PriceHtml = ""
                });
            }
        }
        [HttpPost]
        public JsonResult RemoveCart(int Id)
        {
            try
            {
                if (Request.Cookies.AllKeys.Contains("Cart_" + Id.ToString()))
                {
                    Response.Cookies["Cart_" + Id.ToString()].Expires = DateTime.Now.AddDays(-1);
                    Request.Cookies.Remove("Cart_" + Id.ToString());
                    return Json(new CardViewModel()
                    {
                        Success = true,
                        CountHtml = CartCount(),
                        PriceHtml = CartPrice()
                    });
                }
                else
                {
                    return Json(new CardViewModel()
                    {
                        Success = false,
                        CountHtml = CartCount(),
                        PriceHtml = CartPrice()
                    });
                }
            }
            catch (Exception)
            {
                return Json(new CardViewModel()
                {
                    Success = false,
                    CountHtml = "",
                    PriceHtml = ""
                });
            }
        }

        public string CartCount()
        {
            List<HttpCookie> lst = new List<HttpCookie>();
            for (int i = Request.Cookies.Count - 1; i >= 0; i--)
            {
                if (lst.Where(p => p.Name == Request.Cookies[i].Name).Any() == false)
                    lst.Add(Request.Cookies[i]);
            }
            int CartCount = lst.Where(p => p.Name.StartsWith("Cart_")).Count();
            return CartCount.ToString();
        }

        public string CartPrice()
        {
            List<HttpCookie> lst = new List<HttpCookie>();
            for (int i = Request.Cookies.Count - 1; i >= 0; i--)
            {
                if (lst.Where(p => p.Name == Request.Cookies[i].Name).Any() == false)
                    lst.Add(Request.Cookies[i]);
            }
            int TotalPrice = 0;
            foreach (var item in lst.Where(p => p.Name.StartsWith("Cart_")))
            {
                string idstring = item.Name.Substring(5);
                int id = Convert.ToInt32(idstring);
                int CartCount = Convert.ToInt32(item.Value);
                Product product = db.Products.Find(id);
                if (product.DiscountPercent > 0)
                {
                    var discountPrice = product.UnitPrice - (product.UnitPrice) * (product.DiscountPercent) / 100;
                    TotalPrice += CartCount * discountPrice;
                }
                else
                {
                    TotalPrice += CartCount * product.UnitPrice;
                }
            }
            string x = string.Format("{0:n0}", TotalPrice.ToString());
            return TotalPrice.ToString("N0").Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, "");
        }

        [HttpGet]
        public ActionResult Cart()
        {
            List<BasketViewModel> listBasket = new List<BasketViewModel>();
            List<HttpCookie> lst = new List<HttpCookie>();
            for (int i = Request.Cookies.Count - 1; i >= 0; i--)
            {
                if (lst.Where(p => p.Name == Request.Cookies[i].Name).Any() == false)
                    lst.Add(Request.Cookies[i]);
            }
            foreach (var item in lst.Where(p => p.Name.StartsWith("Cart_")))
            {
                listBasket.Add(new BasketViewModel
                {
                    Product = db.Products.Find(Convert.ToInt32(item.Name.Substring(5))),
                    Count = Convert.ToInt32(item.Value)
                });
            }

            List<SeenViewModel> lstSeen = new List<SeenViewModel>();
            foreach (var item in lst.Where(p => p.Name.StartsWith("SeenProduct_")))
            {
                lstSeen.Add(new SeenViewModel
                {
                    Product = db.Products.Find(Convert.ToInt32(item.Name.Substring(12))),
                    Count = Convert.ToInt32(item.Value)
                });
            }
            ViewBag.SeenProducts = lstSeen.Take(2);
            return View(listBasket);
        }

        [HttpPost]
        public JsonResult GetBasket()
        {
            List<Product> productlist = new List<Product>();

            List<HttpCookie> lst = new List<HttpCookie>();
            for (int i = Request.Cookies.Count - 1; i >= 0; i--)
            {
                if (lst.Where(p => p.Name == Request.Cookies[i].Name).Any() == false)
                    lst.Add(Request.Cookies[i]);
            }
            foreach (var item in lst.Where(p => p.Name.StartsWith("Cart_")))
            {
                string idstring = item.Name.Substring(5);
                int id = Convert.ToInt32(idstring);
                Product product = db.Products.Find(id);
                if (product != null)
                {
                    productlist.Add(product);
                }
            }
            return new JsonResult { Data = productlist.Select(current => new { Product = current.PersianName, Count = productlist.Count }), JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        //[HttpGet]
        //public ActionResult InsertOrder(byte? PaymentType, byte? Type)
        //{
        //    if (PaymentType == null || PaymentType == 0 || Type == null || Type == 0)
        //    {
        //        TempData["error"] = "در حال حاضر امکان ثبت سفارش شما وجود ندارد .";
        //        return RedirectToAction("Cart", "Home");
        //    }
        //    else
        //    {
        //        List<BasketViewModel> listBasket = new List<BasketViewModel>();
        //        List<HttpCookie> lst = new List<HttpCookie>();
        //        for (int i = Request.Cookies.Count - 1; i >= 0; i--)
        //        {
        //            if (lst.Where(p => p.Name == Request.Cookies[i].Name).Any() == false)
        //                lst.Add(Request.Cookies[i]);
        //        }
        //        foreach (var item in lst.Where(p => p.Name.StartsWith("Cart_")))
        //        {
        //            listBasket.Add(new BasketViewModel
        //            {
        //                Product = db.Products.Find(Convert.ToInt32(item.Name.Substring(5))),
        //                Count = Convert.ToInt32(item.Value)
        //            });
        //        }

        //        List<DiscountViewModel> listDiscount = new List<DiscountViewModel>();
        //        List<HttpCookie> lstDiscount = new List<HttpCookie>();
        //        for (int i = Request.Cookies.Count - 1; i >= 0; i--)
        //        {
        //            if (lst.Where(p => p.Name == Request.Cookies[i].Name).Any() == false)
        //                lst.Add(Request.Cookies[i]);
        //        }
        //        foreach (var item in lst.Where(p => p.Name.StartsWith("Discount_")))
        //        {
        //            listDiscount.Add(new DiscountViewModel
        //            {
        //                Discount = db.Discounts.Find(Convert.ToInt32(item.Name.Substring(9))),
        //                Percent = Convert.ToInt32(item.Value)
        //            });
        //        }

        //        List<SeenViewModel> lstSeen = new List<SeenViewModel>();
        //        foreach (var item in lst.Where(p => p.Name.StartsWith("SeenProduct_")))
        //        {
        //            lstSeen.Add(new SeenViewModel
        //            {
        //                Product = db.Products.Find(Convert.ToInt32(item.Name.Substring(12))),
        //                Count = Convert.ToInt32(item.Value)
        //            });
        //        }
        //        ViewBag.SeenProducts = lstSeen.Take(2);
        //        SubmitOrderViewModel viewmodel = new SubmitOrderViewModel();
        //        viewmodel.basket = listBasket;
        //        viewmodel.discount = listDiscount;
        //        viewmodel.FinalPrice = double.Parse(CartPrice());
        //        return View(viewmodel);
        //    }
        //}

        //[HttpGet]
        //public ActionResult InsertOrder()
        //{
        //    return View();
        //}

        [HttpPost, ActionName("SubmitOrder")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitOrder(FinalViewModel order)
        {

            if (Convert.ToInt32(CartPrice()) == 0)
            {
                TempData["Error"] = "سبد خرید شما خالی است و امکان ثبت سفارش وجود ندارد .";
                return RedirectToAction("Index", "Invoice");
            }
            string userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(current => current.Id == userId).Include(current => current.Discounts).Include(current => current.Notifications).FirstOrDefault();
            if (user == null)
            {
                TempData["Error"] = "شناسه کاربری شما یافت نشد .";
                return RedirectToAction("Index", "Invoice");
            }

            #region Validate_Discount
            if (!string.IsNullOrEmpty(order.Discount))
            {
                Discount dis = db.Discounts.Where(currrent => currrent.Code == order.Discount).FirstOrDefault();
                if (dis == null)
                {
                    TempData["Error"] = "کد تخفیف وارد شده معتبر نمیباشد .";
                    return RedirectToAction("Index", "Invoice");
                }
                if (dis.IsActived == false)
                {
                    TempData["Error"] = "کد تخفیف وارد شده غیر فعال شده است .";
                    return RedirectToAction("Index", "Invoice");
                }
                if (dis.IsPercentage == true)
                {
                    if (dis.Percent == 0 || dis.Percent == null)
                    {
                        TempData["Error"] = "کد تخفیف وارد شده مخدوش شده است .";
                        return RedirectToAction("Index", "Invoice");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dis.Amount))
                    {
                        TempData["Error"] = "کد تخفیف وارد شده مخدوش شده است .";
                        return RedirectToAction("Index", "Invoice");
                    }
                }
                //اگر کد تخفیف سقف داشت .
                if (!string.IsNullOrEmpty(dis.MaxOrder) && Convert.ToInt32(dis.MaxOrder) > 0)
                {
                    if (Convert.ToInt32(Convert.ToInt32(CartPrice())) > Convert.ToInt32(dis.MaxOrder))
                    {
                        TempData["Error"] = "مبلغ سفارش شما از کد تخفیف مورد نظر بیشتر است .";
                        return RedirectToAction("Index", "Invoice");
                    }
                }
                if (dis.Count > 0 && dis.Count == dis.MaxCount)
                {
                    if (dis.IsActived == true)
                    {
                        dis.IsActived = false;
                        db.Entry(dis).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Error"] = "ظرفیت استفاده از این کد تخفیف به پایان رسیده است .";
                        return RedirectToAction("Index", "Invoice");
                    }
                }
                if (dis.ExpireDate != null && dis.ExpireDate <= DateTime.Now)
                {
                    if (dis.IsActived == true)
                    {
                        dis.IsActived = false;
                        db.Entry(dis).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Error"] = "زمان استفاده از کد تخفیف مورد نظر شما به اتمام رسیده است .";
                        return RedirectToAction("Index", "Invoice");
                    }
                }
                if (dis.ShowcaseDate != null && dis.ShowcaseDate <= DateTime.Now)
                {
                    if (dis.IsPublic == true)
                    {
                        dis.IsPublic = false;
                        db.Entry(dis).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (user.Discounts.Contains(dis))
                {
                    TempData["Error"] = "شما قبلا از این کد تخفیف استفاده کرده اید .";
                    return RedirectToAction("Index", "Invoice");
                }
            }
            #endregion Validate_Discount

            List<DiscountViewModel> listDiscount = new List<DiscountViewModel>();
            List<HttpCookie> lst = new List<HttpCookie>();
            for (int i = Request.Cookies.Count - 1; i >= 0; i--)
            {
                if (lst.Where(p => p.Name == Request.Cookies[i].Name).Any() == false)
                    lst.Add(Request.Cookies[i]);
            }
            foreach (var item in lst.Where(p => p.Name.StartsWith("Discount_")))
            {
                listDiscount.Add(new DiscountViewModel
                {
                    Discount = db.Discounts.Find(Convert.ToInt32(item.Name.Substring(9))),
                    Percent = Convert.ToInt32(item.Value)
                });
            }
            int TotalPrice = 0;
            if (listDiscount.Count() > 0)
            {
                TotalPrice = Convert.ToInt32(CartPrice()) - Convert.ToInt32(CartPrice()) * Convert.ToInt32(listDiscount.FirstOrDefault().Percent) / 100;
            }
            else
            {
                TotalPrice = Convert.ToInt32(CartPrice());
            }

            #region Order
            Order newOrder = new Order();

            //آنلاین
            //اتصال به درگاه
            if (TotalPrice < 1000)
            {
                TempData["Error"] = "حداقل تراکنش 1000 ریال است .";
                return RedirectToAction("Index", "Invoice");
            }
            Random rnd = new Random();
            if (User.Identity.IsAuthenticated)
            {
                string UserId = User.Identity.GetUserId();
                newOrder.FactorNumber = rnd.Next(11111, 99999).ToString();
                newOrder.FullName = order.FullName;
                newOrder.InvoiceNumber = rnd.Next(11111, 99999).ToString();
                newOrder.PaymentType = 2;
                newOrder.Phone = order.Phone;
                newOrder.UserOrderDescription = order.UserOrderDescription;
                newOrder.UserAddress = order.UserAddress;
                newOrder.TotalPrice = TotalPrice.ToString();
                newOrder.Receiver = 1;
                newOrder.Type = 1;
                newOrder.UserId = UserId;
                newOrder.OrderDate = DateTime.Now;
            }
            else
            {
                TempData["Error"] = "لطفا به حساب کاربری خود ورود کنید .";
                return RedirectToAction("Index", "Invoice");
            }
            try
            {
                db.Orders.Add(newOrder);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید . 0";
                return RedirectToAction("Index", "Invoice");
            }
            List<BasketViewModel> listBasket = new List<BasketViewModel>();
            foreach (var item in lst.Where(p => p.Name.StartsWith("Cart_")))
            {
                listBasket.Add(new BasketViewModel
                {
                    Product = db.Products.Find(Convert.ToInt32(item.Name.Substring(5))),
                    Count = Convert.ToInt32(item.Value)
                });
            }
            foreach (var item in listBasket)
            {
                ProductInOrder productInOrder = new ProductInOrder();
                productInOrder.ProductId = item.Product.Id;
                productInOrder.OrderId = newOrder.Id;
                productInOrder.Count = item.Count;
                try
                {
                    Product product = db.Products.Find(item.Product.Id);
                    if (product.Stock > 0)
                    {
                        product.Stock -= item.Count;
                        db.Entry(product).State = EntityState.Modified;
                    }
                    newOrder.Done = true;
                    db.ProductInOrders.Add(productInOrder);
                    db.Entry(newOrder).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    db.Orders.Remove(newOrder);
                    db.SaveChanges();
                    TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید . 3" + ex;
                    return RedirectToAction("Index", "Invoice");
                }
            }

            #region HasDiscount
            if (!string.IsNullOrEmpty(order.Discount))
            {
                Discount dis = db.Discounts.Where(currrent => currrent.Code == order.Discount).FirstOrDefault();
                dis.Count++;
                db.Entry(dis).State = EntityState.Modified;
                user.Discounts.Add(dis);
                db.Entry(user).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    if (dis.IsPercentage == true)
                    {
                        var discountPrice = int.Parse(newOrder.TotalPrice) - int.Parse(newOrder.TotalPrice) * (dis.Percent) / 100;
                        newOrder.TotalPrice = Convert.ToInt32(discountPrice).ToString();
                    }
                    else
                    {
                        int cost = Convert.ToInt32(newOrder.TotalPrice) - Convert.ToInt32(dis.Amount);
                        if (cost > 0)
                            newOrder.TotalPrice = cost.ToString();
                        else
                            newOrder.TotalPrice = "0";
                    }
                    db.Entry(newOrder).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید . 5";
                    return RedirectToAction("Index", "Invoice");
                }
            }
            #endregion HasDiscount

            #region Conditional_Discount
            Discount conditionalDiscount = db.Discounts.Where(current => current.DiscountCount != null && current.DiscountCount > 0 && current.IsActived == true).FirstOrDefault();
            if (conditionalDiscount != null)
            {
                //مکرر هست
                if (conditionalDiscount.IsRepeated == true)
                {
                    var catOrder = db.Orders.Where(current => current.UserId == newOrder.UserId && current.Done == true);
                    int remain = catOrder.Count() % (int)conditionalDiscount.DiscountCount;
                    if (remain == 0)
                    {
                        Notification notification = new Notification();
                        notification.Text = "کد تخفیف جدید برای شما : " + conditionalDiscount.Code;
                        notification.ForwardDate = DateTime.Now;
                        db.Notifications.Add(notification);
                        db.SaveChanges();
                        user.Notifications.Add(notification);
                        db.SaveChanges();
                    }
                }
                //مکرر نیست
                else
                {
                    var catOrder = db.Orders.Where(current => current.UserId == newOrder.UserId && current.Done == true);
                    if (catOrder.Count() == 1 && conditionalDiscount.DiscountCount == 1)
                    {
                        if (conditionalDiscount.IsPercentage == true)
                        {
                            var discountPrice = int.Parse(newOrder.TotalPrice) - int.Parse(newOrder.TotalPrice) * (conditionalDiscount.Percent) / 100;
                            newOrder.TotalPrice = Convert.ToInt32(discountPrice).ToString();
                        }
                        else
                        {
                            int cost = Convert.ToInt32(newOrder.TotalPrice) - Convert.ToInt32(conditionalDiscount.Amount);
                            if (cost > 0)
                                newOrder.TotalPrice = cost.ToString();
                            else
                                newOrder.TotalPrice = "0";
                        }
                        db.Entry(newOrder).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (catOrder.Count() == conditionalDiscount.DiscountCount)
                        {
                            Notification notification = new Notification();
                            notification.Text = "کد تخفیف جدید برای شما : " + conditionalDiscount.Code;
                            notification.ForwardDate = DateTime.Now;
                            db.Notifications.Add(notification);
                            db.SaveChanges();
                            user.Notifications.Add(notification);
                            db.SaveChanges();
                        }
                    }
                }
            }
            #endregion Conditional_Discount

            //sep
            //#region Saman_Sep
            //var callbackurl = Url.Action("Verify", "Home", new { OrderId = newOrder.Id }, protocol: Request.Url.Scheme);
            //Sep.Init.Payment.PaymentIFBindingSoapClient bankInit = new Sep.Init.Payment.PaymentIFBindingSoapClient();
            //BypassCertificateError();
            //Random random = new Random();
            //var randomNumber = random.Next(10000, 99999);
            //string result = bankInit.RequestToken("11522753",
            //    randomNumber.ToString(),
            //    Convert.ToInt64(newOrder.TotalPrice),
            //    0,
            //    0,
            //    0,
            //    0,
            //    0,
            //    0,
            //    "پرداخت هزینه سفارش در کلینیک شنوایی و سمعک شکوه تجریش",
            //    "",
            //    0
            //   );
            //if (result != null)
            //{
            //    string res = string.Empty;
            //    string[] ResultArray = result.Split(',');
            //    res = ResultArray[0].ToString();
            //    int n;
            //    bool isNumeric = int.TryParse(res, out n);
            //    if (isNumeric == false)
            //    {
            //        return Redirect("http://TajrishSamak.com/home/samangateway?Token=" + res + "&RedirectURL=" + callbackurl);
            //    }
            //    else
            //    {
            //        TempData["Error"] = "امکان اتصال به درگاه بانک وجود ندارد . 1";
            //        return RedirectToAction("Index", "Invoice");
            //    }
            //}
            //else
            //{
            //    TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید 2";
            //    return RedirectToAction("Index", "Invoice");
            //}
            //#endregion Saman_Sep

            Application app = db.Applications.FirstOrDefault();
            if (app != null && app.AfterUserOrderText != null)
            {
                IHtmlString htmlString = new HtmlString(app.AfterUserOrderText);
                string htmlResult = Regex.Replace(htmlString.ToString(), @"<[^>]*>", string.Empty).Replace("\r", "").Replace("\n", "").Replace("&nbsp", ""); ;
                TempData["Success"] = htmlResult;
            }
            else
            {
                TempData["Success"] = "کاربر گرامی سفارش شما با کد " + newOrder.Id + " با موفقیت در سیستم ثبت گردید . ";
            }
            return RedirectToAction("Index", "Home");

        }

        #endregion Order

        public void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate (
                    object sender1,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
        }

        private string GetDate()
        {
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') +
            DateTime.Now.Day.ToString().PadLeft(2, '0');
        }
        private string GetTime()
        {
            return DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') +
            DateTime.Now.Second.ToString().PadLeft(2, '0');
        }

        public ActionResult samangateway()
        {
            return View();
        }

        public ActionResult Verify(int? OrderId)
        {

            Order order = db.Orders.Find(OrderId);
            if (order == null)
            {
                ViewBag.Message = "سفارش شما از سامانه حذف شده است .";
                return View("Result");
            }

            BypassCertificateError();

            try
            {
                if (RequestUnpack())
                {
                    if (transactionState.Equals("OK"))
                    {
                        double result = -1000;
                        ///////////////////////////////////////////////////////////////////////////////////
                        //   *** IMPORTANT  ****   ATTENTION
                        // Here you should check refrenceNumber in your DataBase tp prevent double spending
                        ///////////////////////////////////////////////////////////////////////////////////

                        ///For Ignore SSL Error
                        ServicePointManager.ServerCertificateValidationCallback =
                            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                        ///WebService Instance
                        var srv = new Sep.Refrence.Payment.PaymentIFBindingSoapClient();
                        result = srv.verifyTransaction(Request.Form["RefNum"], Request.Form["MID"]);
                        string CardNumber = Request.Form["SecurePan"].ToString();
                        string MID = Request.Form["MID"].ToString();
                        string RefNum = Request.Form["RefNum"].ToString();
                        string TRACENO = Request.Form["TRACENO"].ToString();
                        string ResNum = Request.Form["ResNum"].ToString();
                        double Ammount = double.Parse(Request.Form["Amount"].ToString());

                        if (result > 0)
                        {
                            if (result < (double)Ammount) //Total Amount of Basket
                            {
                                succeedMsg = "مبلغ انتقالي کمتر از مبلغ کل فاکتور ميباشد";
                                isError = true;
                                ViewBag.Message = succeedMsg;
                                return View("Result");
                            }
                            else if (result > (double)Ammount) //Total Amount of Basket
                            {
                                succeedMsg = "خريد شما تاييد و نهايي گشت اما مبلغ انتقالي بيش از مبلغ خريد ميباشد";
                                isError = true;
                                ViewBag.Message = succeedMsg;
                                return View("Result");
                            }
                            //Success
                            else if (result == (double)Ammount)
                            {
                                isError = false;
                                succeedMsg = "بانک صحت رسيد ديجيتالي شما را تصديق نمود. فرايند خريد تکميل گشت";
                                //تراکنش تایید و ستل شده است 
                                //پس از اتصال به درگاه بانکی و در صورت موفقیت تراکنش
                                #region AddPayment
                                Transaction trans = new Transaction()
                                {
                                    Number = RefNum,
                                    InvoiceNumber = TRACENO,
                                    BankName = "سپ(سامان)",
                                    RecievedDocumentNumber = ResNum,
                                    RecievedDocumentDate = DateTime.Now,
                                    CardNumber = CardNumber,
                                    TerminalNumber = MID,
                                    Acceptor = "TajrishSamak",
                                    OperationResult = 1,
                                    AcceptorPostalCode = "",
                                    AcceptorPhoneNumber = "0912-8445184",
                                    Date = DateTime.Now,
                                    OrderId = order.Id
                                };
                                db.Transactions.Add(trans);
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    TempData["Error"] = " خطایی رخ داده است لطفا با پشتیبانی تماس بگیرید ، کد پیگیری : " + RefNum.ToString();
                                    ViewBag.Message = succeedMsg + ex;
                                    return View("Result");
                                }
                                Payment payment = new Payment();
                                payment.UserId = order.UserId;
                                payment.Amount = (long)result;
                                payment.Status = 3;
                                payment.ActionType = 2;
                                Random rnd = new Random();
                                payment.Description = "هزینه سفارش شماره : " + order.Id;
                                payment.TrackingCode = RefNum ?? rnd.Next(1, 6).ToString();
                                payment.CreateDate = DateTime.Now;
                                db.Payments.Add(payment);
                                try
                                {
                                    db.SaveChanges();
                                    ViewBag.Message = "ممنون از خرید شما ، پرداخت با موفقیت انجام شد .";
                                    return View("Result");
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.Message = "خطایی رخ داده است لطفا مجدد تلاش فرمایید ." + ex + "2";
                                    return View("Result");
                                }
                                #endregion AddPayment
                            }
                            else
                            {
                                succeedMsg = "خطایی رخ داده است لطفا مجدد تلاش فرمایید ." + "3";
                                isError = true;
                                ViewBag.Message = succeedMsg;
                                return View("Result");
                            }
                        }
                        else
                        {
                            TransactionChecking((int)result);
                            isError = true;
                            errorMsg = "خطایی رخ داده است 1 ." + TransactionChecking((int)result);
                            ViewBag.Message = errorMsg;
                            return View("Result");
                        }
                    }
                    else
                    {
                        isError = true;
                        errorMsg = "متاسفانه بانک خريد شما را تاييد نکرده است";
                        if (transactionState.Equals("Canceled By User") || transactionState.Equals(string.Empty))
                        {
                            // Transaction was canceled by user
                            errorMsg = "تراكنش توسط خريدار كنسل شد .";
                        }
                        else if (transactionState.Equals("Invalid Amount"))
                        {
                            // Amount of revers teransaction is more than teransaction
                        }
                        else if (transactionState.Equals("Invalid Transaction"))
                        {
                            // Can not find teransaction
                        }
                        else if (transactionState.Equals("Invalid Card Number"))
                        {
                            // Card number is wrong
                        }
                        else if (transactionState.Equals("No Such Issuer"))
                        {
                            // Issuer can not find
                        }
                        else if (transactionState.Equals("Expired Card Pick Up"))
                        {
                            // The card is expired
                        }
                        else if (transactionState.Equals("Allowable PIN Tries Exceeded Pick Up"))
                        {
                            // For third time user enter a wrong PIN so card become invalid
                        }
                        else if (transactionState.Equals("Incorrect PIN"))
                        {
                            // Pin card is wrong
                        }
                        else if (transactionState.Equals("Exceeds Withdrawal Amount Limit"))
                        {
                            // Exceeds withdrawal from amount limit
                        }
                        else if (transactionState.Equals("Transaction Cannot Be Completed"))
                        {
                            // PIN and PAD are currect but Transaction Cannot Be Completed
                        }
                        else if (transactionState.Equals("Response Received Too Late"))
                        {
                            // Timeout occur
                        }
                        else if (transactionState.Equals("Suspected Fraud Pick Up"))
                        {
                            // User did not insert cvv2 & expiredate or they are wrong.
                        }
                        else if (transactionState.Equals("No Sufficient Funds"))
                        {
                            // there are not suficient funds in the account
                        }
                        else if (transactionState.Equals("Issuer Down Slm"))
                        {
                            // The bank server is down
                        }
                        else if (transactionState.Equals("TME Error"))
                        {
                            // unknown error occur
                        }
                        ViewBag.Message = errorMsg;
                        return View("Result");
                    }
                }
                else
                {
                    isError = true;
                    errorMsg = "خطایی رخ داده است 2 .";
                    ViewBag.Message = errorMsg;
                    return View("Result");
                }
            }
            catch (Exception ex)
            {
                isError = true;
                errorMsg = "خطایی رخ داده است 3 .";
                ViewBag.Message = errorMsg;
                return View("Result");
            }
        }

        public ActionResult Result()
        {
            return View();
        }

        #region SamanBank
        private string refrenceNumber = string.Empty;
        private string reservationNumber = string.Empty;
        private string transactionState = string.Empty;
        bool isError = false;
        string errorMsg = "";
        string succeedMsg = "";

        private bool RequestUnpack()
        {
            if (RequestFeildIsEmpty()) return false;

            refrenceNumber = Request.Form["RefNum"].ToString();
            reservationNumber = Request.Form["ResNum"].ToString();
            transactionState = Request.Form["state"].ToString();

            return true;
        }
        private bool RequestFeildIsEmpty()
        {
            if (Request.Form["state"].ToString().Equals(string.Empty))
            {
                isError = true;
                errorMsg = "خريد شما توسط بانک تاييد شده است اما رسيد ديجيتالي شما تاييد نگشت! مشکلي در فرايند رزرو خريد شما پيش آمده است";
                return true;
            }

            if (Request.Form["RefNum"].ToString().Equals(string.Empty) && Request.Form["state"].ToString().Equals(string.Empty))
            {
                isError = true;
                errorMsg = "فرايند انتقال وجه با موفقيت انجام شده است اما فرايند تاييد رسيد ديجيتالي با خطا مواجه گشت";
                return true;
            }

            if (Request.Form["ResNum"].ToString().Equals(string.Empty) && Request.Form["state"].ToString().Equals(string.Empty))
            {
                isError = true;
                errorMsg = "خطا در برقرار ارتباط با بانک";
                return true;
            }
            return false;
        }

        private string TransactionChecking(int i)
        {
            bool isError = false;
            string errorMsg = "";
            switch (i)
            {

                case -1:        //TP_ERROR
                    isError = true;
                    errorMsg = "بروز خطا درهنگام بررسي صحت رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-1";
                    break;
                case -2:        //ACCOUNTS_DONT_MATCH
                    isError = true;
                    errorMsg = "بروز خطا در هنگام تاييد رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-2";
                    break;
                case -3:        //BAD_INPUT
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-3";
                    break;
                case -4:        //INVALID_PASSWORD_OR_ACCOUNT
                    isError = true;
                    errorMsg = "خطاي دروني سيستم درهنگام بررسي صحت رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-4";
                    break;
                case -5:        //DATABASE_EXCEPTION
                    isError = true;
                    errorMsg = "خطاي دروني سيستم درهنگام بررسي صحت رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-5";
                    break;
                case -7:        //ERROR_STR_NULL
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-7";
                    break;
                case -8:        //ERROR_STR_TOO_LONG
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-8";
                    break;
                case -9:        //ERROR_STR_NOT_AL_NUM
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-9";
                    break;
                case -10:   //ERROR_STR_NOT_BASE64
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-10";
                    break;
                case -11:   //ERROR_STR_TOO_SHORT
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-11";
                    break;
                case -12:       //ERROR_STR_NULL
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-12";
                    break;
                case -13:       //ERROR IN AMOUNT OF REVERS TRANSACTION AMOUNT
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-13";
                    break;
                case -14:   //INVALID TRANSACTION
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-14";
                    break;
                case -15:   //RETERNED AMOUNT IS WRONG
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-15";
                    break;
                case -16:   //INTERNAL ERROR
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-16";
                    break;
                case -17:   // REVERS TRANSACTIN FROM OTHER BANK
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-17";
                    break;
                case -18:   //INVALID IP
                    isError = true;
                    errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-18";
                    break;

            }
            return errorMsg;
        }
        #endregion SamanBank
    }
}
