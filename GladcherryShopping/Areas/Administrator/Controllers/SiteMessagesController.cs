using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SiteMessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/SiteMessages
        public ActionResult Index(int page = 1)
        {
            var siteMessages = db.SiteMessages;
            PagerViewModels<SiteMessage> SiteMessageViewModels = new PagerViewModels<SiteMessage>();
            SiteMessageViewModels.CurrentPage = page;
            SiteMessageViewModels.data = siteMessages.OrderByDescending(current => current.RegisterDate).Skip((page - 1) * 10).Take(10).ToList();
            SiteMessageViewModels.TotalItemCount = siteMessages.Count();
            return View(SiteMessageViewModels);
        }

        // GET: Administrator/SiteMessages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteMessage siteMessage = db.SiteMessages.Find(id);
            if (siteMessage == null)
            {
                return HttpNotFound();
            }
            return View(siteMessage);
        }

        // GET: Administrator/SiteMessages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/SiteMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FullName,Body")] SiteMessage siteMessage)
        {
            if (ModelState.IsValid)
            {
                db.SiteMessages.Add(siteMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(siteMessage);
        }

        // GET: Administrator/SiteMessages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteMessage siteMessage = db.SiteMessages.Find(id);
            if (siteMessage == null)
            {
                return HttpNotFound();
            }
            return View(siteMessage);
        }

        // POST: Administrator/SiteMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FullName,Body")] SiteMessage siteMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siteMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(siteMessage);
        }

        // GET: Administrator/SiteMessages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteMessage siteMessage = db.SiteMessages.Find(id);
            if (siteMessage == null)
            {
                return HttpNotFound();
            }
            return View(siteMessage);
        }

        // POST: Administrator/SiteMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SiteMessage siteMessage = db.SiteMessages.Find(id);
            db.SiteMessages.Remove(siteMessage);
            db.SaveChanges();
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
