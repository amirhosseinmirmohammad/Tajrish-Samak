using DataLayer.Models;
using GladcherryShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class invoiceController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
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
        // GET: invoice
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(CartPrice()))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.Price = CartPrice();
            return View();
        }
    }
}