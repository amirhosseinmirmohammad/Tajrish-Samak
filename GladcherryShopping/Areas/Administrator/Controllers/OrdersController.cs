using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataLayer.Models;
using GladcherryShopping.Models;
using System;
using DataLayer.ViewModels.PagerViewModel;
using System.Collections.Generic;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Orders
        public ActionResult Index(int page = 1)
        {
            var orders = db.Orders.Include(c => c.Address).Include(current => current.User).Include(c => c.Transactions);
            PagerViewModels<Order> OrderViewModels = new PagerViewModels<Order>();
            OrderViewModels.CurrentPage = page;
            OrderViewModels.data = orders.OrderByDescending(current => current.OrderDate).Skip((page - 1) * 10).Take(10).ToList();
            OrderViewModels.TotalItemCount = orders.Count();
            return View(OrderViewModels);
        }

        public ActionResult SearchOrder()
        {
            List<Order> Orders = new List<Order>();
            ViewBag.ReturnUrl = "/Administrator/Orders/SearchOrder";
            return View(Orders);
        }

        public ActionResult OrderResult(int? OrderId, string returnUrl)
        {
            if (OrderId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.Orders.Where(current => current.Id == OrderId).Include(c => c.User).Include(c => c.Transactions).ToList();
            return View("SearchOrder", order);
        }

        [HttpGet]
        public ActionResult OrderFound(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.Orders.Where(current => current.Id == id).Include(c => c.User).Include(c => c.Transactions).ToList();
            var orders = db.Orders.Include(c => c.User).Include(c => c.Transactions);
            PagerViewModels<Order> OrderViewModels = new PagerViewModels<Order>();
            OrderViewModels.data = order;
            OrderViewModels.CurrentPage = 1;
            OrderViewModels.TotalItemCount = 1;
            return View("index", OrderViewModels);
        }

        public ActionResult DoneOrders(int page = 1)
        {
            var orders = db.Orders.Include(c => c.User).Include(c => c.Transactions);
            PagerViewModels<Order> OrderViewModels = new PagerViewModels<Order>();
            OrderViewModels.data = orders.OrderByDescending(current => current.OrderDate).Skip((page - 1) * 10).Take(10).Where(current => current.Done == true).ToList();
            OrderViewModels.CurrentPage = page;
            OrderViewModels.TotalItemCount = orders.Count();
            return View("Index", OrderViewModels);
        }

        public ActionResult CanceledOrders(int page = 1)
        {
            var orders = db.Orders.Include(c => c.User).Include(c => c.Transactions);
            PagerViewModels<Order> OrderViewModels = new PagerViewModels<Order>();
            OrderViewModels.data = orders.OrderByDescending(current => current.OrderDate).Skip((page - 1) * 10).Take(10).Where(current => current.IsCanceled == true).ToList();
            OrderViewModels.CurrentPage = page;
            OrderViewModels.TotalItemCount = orders.Count();
            return View("Index", OrderViewModels);
        }

        // GET: Administrator/Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Where(current => current.Id == id).Include(current => current.Products).Include(current => current.Transactions).FirstOrDefault();
            ViewBag.Products = db.ProductInOrders.Where(current => current.OrderId == order.Id).Include(current => current.Product).ToList();
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        //// GET: Administrator/Orders/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AddressId = new SelectList(db.Addresses, "Id", "Addrress");
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
        //    return View();
        //}

        //// POST: Administrator/Orders/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Receiver,Type,PaymentType,OrderDate,Done,InvoiceNumber,TotalPrice,Latitude,Longitude,UserId,IsCanceled,CancelDescription,FullName,Phone,UserAddress,FactorNumber,AddressId")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Orders.Add(order);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.AddressId = new SelectList(db.Addresses, "Id", "Addrress", order.AddressId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", order.UserId);
        //    return View(order);
        //}

        //// GET: Administrator/Orders/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Order order = db.Orders.Find(id);
        //    if (order == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.AddressId = new SelectList(db.Addresses, "Id", "Addrress", order.AddressId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", order.UserId);
        //    return View(order);
        //}

        //// POST: Administrator/Orders/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Receiver,Type,PaymentType,OrderDate,Done,InvoiceNumber,TotalPrice,Latitude,Longitude,UserId,IsCanceled,CancelDescription,FullName,Phone,UserAddress,FactorNumber,AddressId")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(order).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.AddressId = new SelectList(db.Addresses, "Id", "Addrress", order.AddressId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", order.UserId);
        //    return View(order);
        //}

        [HttpGet]
        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            order.IsCanceled = true; ;
            order.CancelDescription = " این سفارش توسط مدیریت لغو شده است . ";
            db.Entry(order).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                TempData["Success"] = "سفارش مورد نظر با موفقیت لغو شد .";
            }
            catch (Exception)
            {
                TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .";
            }

            return RedirectToAction("Index");
        }

        // GET: Administrator/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            ViewBag.Products = db.ProductInOrders.Where(current => current.OrderId == order.Id).Include(current => current.Product).ToList();
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Administrator/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            try
            {
                db.SaveChanges();
                TempData["Success"] = "سفارش مورد نظر با موفقیت حذف شد .";
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها امکان حذف این سفارش از سیستم وجود ندارد .";
                return View();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Transaction(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            Transaction transaction = db.Transactions.Where(current => current.OrderId == id).FirstOrDefault();
            if (transaction == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(transaction);
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
