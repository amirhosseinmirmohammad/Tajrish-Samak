using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataLayer.Models;
using DataLayer.ViewModels;
using GladcherryShopping.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladCherryShopping.Helpers;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PaymentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Payments
        public ActionResult Index(int page = 1)
        {
            var payments = db.Payments.Where(current => current.Status == 1).Include(p => p.Transaction).Include(p => p.Order).Include(p => p.User);
            PagerViewModels<Payment> PaymentViewModels = new PagerViewModels<Payment>();
            PaymentViewModels.data = payments.OrderByDescending(current => current.CreateDate).Skip((page - 1) * 10).Take(10).ToList();
            PaymentViewModels.CurrentPage = page;
            PaymentViewModels.TotalItemCount = payments.Count();
            return View(PaymentViewModels);
        }

        // GET: Administrator/Payments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Amount,Status,ActionType,Mdate,Subject,Description,CreateDate,ContractorId,UserId,OrderId,TrackingCode")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.Status = 1;
                payment.ActionType = 1;
                payment.CreateDate = DateTime.Now;
                db.Payments.Add(payment);
                payment.MainDate = FunctionsHelper.ConvertToGregorian(payment.Mdate);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .";
                }
                return RedirectToAction("Index");
            }
            return View(payment);
        }

        // GET: Administrator/Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Administrator/Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Amount,Status,ActionType,Mdate,Subject,Description,CreateDate,ContractorId,UserId,OrderId,TrackingCode")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.MainDate = FunctionsHelper.ConvertToGregorian(payment.Mdate);
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(payment);
        }

        // GET: Administrator/Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Administrator/Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها در سیستم امکان حذف این سند پرداختی وجود ندارد .";
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
