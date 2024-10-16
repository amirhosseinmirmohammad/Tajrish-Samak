using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using Microsoft.AspNet.Identity;
using GladcherryShopping.Models;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SentEmailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/SentEmails
        public ActionResult Index(int page = 1)
        {
            var sentEmail = db.SentEmails;
            PagerViewModels<SentEmail> SentEmailViewModels = new PagerViewModels<SentEmail>();
            SentEmailViewModels.CurrentPage = page;
            SentEmailViewModels.data = sentEmail.OrderByDescending(current => current.SendDate).Skip((page - 1) * 10).Take(10).ToList();
            SentEmailViewModels.TotalItemCount = sentEmail.Count();
            return View(SentEmailViewModels);
        }

        [HttpGet]
        public ActionResult NewsLetterSend(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.FirstOrDefault();
            if (application != null)
            {
                try
                {
                    if (db.NewsLetters.Count() > 0)
                    {
                        SentEmail model = db.SentEmails.Find(id);
                        IHtmlString htmlString = new HtmlString(model.Text);
                        foreach (var item in db.NewsLetters.Where(current => current.Email != null).ToList())
                        {
                            var emailService = new EmailService();
                            var message = new IdentityMessage();
                            message.Body = model.Text;
                            message.Destination = item.Email;
                            message.Subject = model.Title;
                            emailService.SendAsync(message);
                        }
                        TempData["Success"] = " ایمیل شما با موفقیت به  " + db.NewsLetters.Count() + " عضو فعال در خبرنامه ارسال شد . ";
                    }
                    else
                    {
                        TempData["Error"] = " هنوز عضوی در خبرنامه سیستم ثبت نشده است . ";
                    }
                }
                catch (Exception)
                {
                    TempData["Error"] = "لطفا تنظیمات کلی نرم افزار خود را بررسی نمایید .";
                }
            }
            else
            {
                TempData["Error"] = " لطفا تنطیمات کلی برنامه خود را تعیین نمایید . ";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult UserSend(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.FirstOrDefault();
            if (application != null)
            {
                try
                {
                    var role = db.Roles.Where(p => p.Name == "User").FirstOrDefault();
                    IQueryable<ApplicationUser> query = db.Users.Include(current => current.State).Include(current => current.City).Where(p => p.Roles.Any(a => a.RoleId == role.Id));
                    if (query.Count() > 0)
                    {
                        SentEmail model = db.SentEmails.Find(id);
                        IHtmlString htmlString = new HtmlString(model.Text);
                        foreach (var item in query.Where(current => current.IsActive == true).ToList())
                        {
                            var emailService = new EmailService();
                            var message = new IdentityMessage();
                            message.Body = model.Text;
                            message.Destination = item.Email;
                            message.Subject = model.Title;
                            emailService.SendAsync(message);
                        }
                        TempData["Success"] = " ایمیل شما با موفقیت به  " + query.Count() + " کاربر فعال در سیستم ارسال شد . ";
                    }
                    else
                    {
                        TempData["Error"] = " هنوز کاربری در سیستم ثبت نشده است . ";
                    }
                }
                catch (Exception)
                {
                    TempData["Error"] = "لطفا تنظیمات کلی نرم افزار خود را بررسی نمایید .";
                }
            }
            else
            {
                TempData["Error"] = " لطفا تنطیمات کلی برنامه خود را تعیین نمایید . ";
            }

            return RedirectToAction("Index");
        }
        // GET: Administrator/SentEmails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SentEmail sentEmail = db.SentEmails.Find(id);
            if (sentEmail == null)
            {
                return HttpNotFound();
            }
            return View(sentEmail);
        }

        // GET: Administrator/SentEmails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/SentEmails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SendDate,Text,FromEmail,Title")] SentEmail sentEmail)
        {
            if (ModelState.IsValid)
            {
                sentEmail.SendDate = DateTime.Now;
                db.SentEmails.Add(sentEmail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sentEmail);
        }

        // GET: Administrator/SentEmails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SentEmail sentEmail = db.SentEmails.Find(id);
            if (sentEmail == null)
            {
                return HttpNotFound();
            }
            return View(sentEmail);
        }

        // POST: Administrator/SentEmails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SendDate,Text,FromEmail")] SentEmail sentEmail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sentEmail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sentEmail);
        }

        // GET: Administrator/SentEmails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SentEmail sentEmail = db.SentEmails.Find(id);
            if (sentEmail == null)
            {
                return HttpNotFound();
            }
            return View(sentEmail);
        }

        // POST: Administrator/SentEmails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SentEmail sentEmail = db.SentEmails.Find(id);
            db.SentEmails.Remove(sentEmail);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها در سیستم امکان حذف این ایمیل وجود ندارد .";
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
