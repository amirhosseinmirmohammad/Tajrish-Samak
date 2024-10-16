using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataLayer.Models;
using GladcherryShopping.Models;
using System;
using DataLayer.ViewModels.PagerViewModel;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Transactions
        public ActionResult Index(int page = 1)
        {
            var transactions = db.Transactions.Include(c => c.Order);
            PagerViewModels<Transaction> TransactionViewModels = new PagerViewModels<Transaction>();
            TransactionViewModels.CurrentPage = page;
            TransactionViewModels.data = transactions.OrderByDescending(current => current.Date).Skip((page - 1) * 10).Take(10).ToList();
            TransactionViewModels.TotalItemCount = transactions.Count();
            return View(TransactionViewModels);
        }

        // GET: Administrator/Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Administrator/Transactions/Create
        public ActionResult Create()
        {
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "InvoiceNumber");
            return View();
        }

        // POST: Administrator/Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Number,Date,InvoiceNumber,BankName,RecievedDocumentNumber,RecievedDocumentDate,CardNumber,TerminalNumber,Acceptor,OperationResult,AcceptorPostalCode,AcceptorPhoneNumber,OrderId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "Id", "InvoiceNumber", transaction.OrderId);
            return View(transaction);
        }

        // GET: Administrator/Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "InvoiceNumber", transaction.OrderId);
            return View(transaction);
        }

        // POST: Administrator/Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Number,Date,InvoiceNumber,BankName,RecievedDocumentNumber,RecievedDocumentDate,CardNumber,TerminalNumber,Acceptor,OperationResult,AcceptorPostalCode,AcceptorPhoneNumber,OrderId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "InvoiceNumber", transaction.OrderId);
            return View(transaction);
        }

        // GET: Administrator/Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Administrator/Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها امکان حذف این تراکنش از سیستم وجود ندارد .";
                return View();
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
