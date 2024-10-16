using System;
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
    public class StatesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/States
        public ActionResult Index(int page = 1)
        {
            var states = db.States;
            PagerViewModels<State> StateViewModels = new PagerViewModels<State>();
            StateViewModels.CurrentPage = page;
            StateViewModels.data = states.OrderByDescending(current => current.Name).Skip((page - 1) * 10).Take(10).ToList();
            StateViewModels.TotalItemCount = states.Count();
            return View(StateViewModels);
        }

        // GET: Administrator/States/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            return View(state);
        }

        // GET: Administrator/States/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/States/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] State state)
        {
            if (ModelState.IsValid)
            {
                db.States.Add(state);
                db.SaveChanges();
                TempData["Success"] = "استان مورد نظر با موفقیت ثبت گردید .";
                return RedirectToAction("Index");
            }

            return View(state);
        }

        // GET: Administrator/States/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            return View(state);
        }

        // POST: Administrator/States/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] State state)
        {
            if (ModelState.IsValid)
            {
                db.Entry(state).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "استان مورد نظر با موفقیت ویرایش گردید .";
                return RedirectToAction("Index");
            }
            return View(state);
        }

        // GET: Administrator/States/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            return View(state);
        }

        // POST: Administrator/States/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            State state = db.States.Find(id);
            db.States.Remove(state);
            try
            {
                TempData["Success"] = "استان مورد نظر با موفقیت حذف شد .";
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Error"] = " به دلیل وجود زیر شاخه ها امکان حذف این استان وجود ندارد . ";
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
