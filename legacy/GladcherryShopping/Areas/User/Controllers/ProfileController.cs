using GladcherryShopping.Models;
using GladCherryShopping.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GladcherryShopping.Areas.User.Controllers
{
    [Authorize(Roles = "User")]
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: User/Profile
        public ActionResult Edit()
        {
            string UserId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(UserId))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            ApplicationUser user = db.Users.Find(UserId);
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", user.CityId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", user.StateId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Gender,BirthDate,RegistrationDate,UserScore,ProfileImage,AddressLine,StateId,CityId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,IsActive,Credit,VerificationCode,AccessCode,IntroCode,Mobile")] ApplicationUser applicationUser, string birthDateString)
        {
            if (ModelState.IsValid)
            {
                applicationUser.BirthDate = FunctionsHelper.ConvertToGregorian(birthDateString);
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { UserId = Request.QueryString["UserId"] });
            }
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Name", applicationUser.CityId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", applicationUser.StateId);
            return RedirectToAction("Edit", new { UserId = Request.QueryString["UserId"] });
        }
    }
}