using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
using static GladCherryShopping.Helpers.FunctionsHelper;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Users
        public ActionResult Index(int page = 1)
        {
            var role = db.Roles.Where(p => p.Name == "User").FirstOrDefault();
            IQueryable<ApplicationUser> query = db.Users.Include(current => current.Roles).Include(current => current.State).Include(current => current.City).Where(p => p.Roles.Any(a => a.RoleId == role.Id));
            PagerViewModels<ApplicationUser> UserViewModels = new PagerViewModels<ApplicationUser>();
            UserViewModels.CurrentPage = page;
            UserViewModels.data = query.OrderByDescending(current => current.RegistrationDate).ThenByDescending(current => current.CityId).ThenByDescending(current => current.LastName).ThenByDescending(current => current.FirstName).Skip((page - 1) * 10).Take(10).ToList();
            UserViewModels.TotalItemCount = query.ToList().Count();
            return View(UserViewModels);
        }

        public ActionResult Orders(string id, int page = 1)
        {
            PagerViewModels<Order> OrderModels = new PagerViewModels<Order>();
            OrderModels.CurrentPage = page;
            OrderModels.data = db.Orders.Where(current => current.UserId == id).OrderByDescending(current => current.Id).Skip((page - 1) * 10).Take(10).ToList();
            OrderModels.TotalItemCount = db.Orders.Where(current => current.UserId == id).ToList().Count();
            return View(OrderModels);
        }

        public ActionResult Addresses(string UserId, int page = 1)
        {
            PagerViewModels<Address> AddressModels = new PagerViewModels<Address>();
            AddressModels.CurrentPage = page;
            AddressModels.data = db.Addresses.Where(current => current.UserId == UserId).OrderByDescending(current => current.Id).Skip((page - 1) * 10).Take(10).ToList();
            AddressModels.TotalItemCount = db.Addresses.Where(current => current.UserId == UserId).ToList().Count();
            return View(AddressModels);
        }

        public ActionResult AddressDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }


        public ActionResult SearchUser(int page = 1)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            ViewBag.ReturnUrl = "/Administrator/Users/SearchUser";
            PagerViewModels<ApplicationUser> UserViewModels = new PagerViewModels<ApplicationUser>();
            UserViewModels.CurrentPage = page;
            UserViewModels.data = users;
            UserViewModels.TotalItemCount = users.ToList().Count();
            return View(UserViewModels);
        }

        public ActionResult UserResult(string Name, string returnUrl, int page = 1)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var users = db.Users.Where(current => current.FirstName.Contains(Name) || current.LastName.Contains(Name)).OrderByDescending(current => current.StateId).ThenByDescending(current => current.CityId).ThenByDescending(current => current.LastName).ThenByDescending(current => current.FirstName).Skip((page - 1) * 10).Take(10).ToList();
            PagerViewModels<ApplicationUser> UserViewModels = new PagerViewModels<ApplicationUser>();
            UserViewModels.CurrentPage = page;
            UserViewModels.data = users.OrderByDescending(current => current.LastName).ThenByDescending(current => current.FirstName).Skip((page - 1) * 10).Take(10).ToList();
            UserViewModels.TotalItemCount = users.ToList().Count();
            return View("SearchUser", UserViewModels);
        }

        // GET: Administrator/Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Include(current => current.City).Include(current => current.State).FirstOrDefault(current => current.Id == id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: Administrator/Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", applicationUser.CityId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", applicationUser.StateId);
            return View(applicationUser);
        }

        // POST: Administrator/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Gender,BirthDate,RegistrationDate,UserScore,ProfileImage,AddressLine,StateId,CityId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,IsActive,Credit,VerificationCode,AccessCode,IntroCode,Mobile")] ApplicationUser applicationUser, string birthDateString)
        {
            if (ModelState.IsValid)
            {
                applicationUser.BirthDate = ConvertToGregorian(birthDateString);
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", applicationUser.CityId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", applicationUser.StateId);
            return View(applicationUser);
        }

        // GET: Administrator/Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Include(current => current.City).Include(current => current.State).FirstOrDefault(current => current.Id == id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Administrator/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            try
            {
                db.SaveChanges();
                TempData["Success"] = "کاربر مورد نظر با موفقیت از سیستم حذف شد .";
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها امکان حذف این کاربر از سیستم وجود ندارد .";
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
