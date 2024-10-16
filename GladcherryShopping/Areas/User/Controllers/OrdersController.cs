using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataLayer.Models;
using GladcherryShopping.Models;
using System;
using DataLayer.ViewModels.PagerViewModel;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace GladcherryShopping.Areas.User.Controllers
{
    [Authorize(Roles = "User")]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Orders
        public ActionResult Index(int page = 1)
        {
            string UserId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(UserId))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            var orders = db.Orders.Where(current => current.UserId == UserId).Include(c => c.Address).Include(current => current.User).Include(c => c.Transactions);
            PagerViewModels<Order> OrderViewModels = new PagerViewModels<Order>();
            OrderViewModels.CurrentPage = page;
            OrderViewModels.data = orders.OrderByDescending(current => current.OrderDate).Skip((page - 1) * 10).Take(10).ToList();
            OrderViewModels.TotalItemCount = orders.Count();
            return View(OrderViewModels);
        }

        public ActionResult DoneOrders(int page = 1)
        {
            string UserId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(UserId))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            var orders = db.Orders.Where(current => current.UserId == UserId).Include(c => c.User).Include(c => c.Transactions);
            PagerViewModels<Order> OrderViewModels = new PagerViewModels<Order>();
            OrderViewModels.data = orders.OrderByDescending(current => current.OrderDate).Skip((page - 1) * 10).Take(10).Where(current => current.Done == true).ToList();
            OrderViewModels.CurrentPage = page;
            OrderViewModels.TotalItemCount = orders.Count();
            return View("Index", OrderViewModels);
        }

        public ActionResult CanceledOrders(int page = 1)
        {
            string UserId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(UserId))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            var orders = db.Orders.Where(current => current.UserId == UserId).Include(c => c.User).Include(c => c.Transactions);
            PagerViewModels<Order> OrderViewModels = new PagerViewModels<Order>();
            OrderViewModels.data = orders.OrderByDescending(current => current.OrderDate).Skip((page - 1) * 10).Take(10).Where(current => current.IsCanceled == true).ToList();
            OrderViewModels.CurrentPage = page;
            OrderViewModels.TotalItemCount = orders.Count();
            return View("Index", OrderViewModels);
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

        // GET: Administrator/Orders/Details/5
        public ActionResult Details(int? id)
        {
            string UserId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(UserId))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Where(current => current.UserId == UserId && current.Id == id).Include(current => current.Products).FirstOrDefault();
            ViewBag.Products = db.ProductInOrders.Where(current => current.OrderId == order.Id).Include(current => current.Product).ToList();
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
    }
}