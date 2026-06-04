using DataLayer.Models;
using GladcherryShopping.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(int? id, string sefUrl)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Service service = db.Services
                .Include(current => current.Category)
                .Include(current => current.Images)
                .Include(current => current.User)
                .Include(current => current.User.Roles)
                .FirstOrDefault(current =>
                    current.Id == id.Value &&
                    current.IsVisible == true &&
                    current.Images.Any());

            if (service == null)
            {
                return HttpNotFound();
            }

            service.Survey++;
            db.Entry(service).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.RelatedServices = db.Services
                .AsNoTracking()
                .Include(current => current.Images)
                .Where(current =>
                    current.IsVisible == true &&
                    current.Id != service.Id &&
                    current.CategoryId == service.CategoryId &&
                    current.Images.Any())
                .OrderByDescending(current => current.Survey)
                .Take(4)
                .ToList();

            return View(service);
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
