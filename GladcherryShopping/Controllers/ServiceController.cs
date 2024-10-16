using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class ServiceController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpGet]
        public ActionResult Index(int? id, string sefUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service blog = db.Services.Where(current => current.Id == id && current.Images.Count > 0).Include(current => current.Category).Include(current => current.Images).Include(current => current.User).Include(current => current.User.Roles).FirstOrDefault();
            blog.Survey++;
            db.Entry(blog).State = EntityState.Modified;
            db.SaveChanges();
            return View(blog);
        }

    }
}