using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GladcherryShopping.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Notifications
        public ActionResult Index(int page = 1)
        {
            var notifications = db.Notifications;
            PagerViewModels<Notification> NotificationsViewModels = new PagerViewModels<Notification>();
            NotificationsViewModels.CurrentPage = page;
            NotificationsViewModels.data = notifications.OrderByDescending(current => current.ForwardDate).Skip((page - 1) * 10).Take(10).ToList();
            NotificationsViewModels.TotalItemCount = notifications.Count();
            return View(NotificationsViewModels);
        }


        [HttpGet]
        public ActionResult NewsletterSend(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            try
            {
                if (db.NewsLetters.Count() > 0)
                {
                    IHtmlString htmlString = new HtmlString(notification.Text);
                    foreach (var item in db.NewsLetters.Where(current => current.Email != null).ToList())
                    {
                        item.Notifications.Add(notification);
                        db.SaveChanges();
                    }
                    TempData["Success"] = " اطلاعیه شما با موفقیت به  " + db.NewsLetters.Count() + " عضو خبرنامه در سیستم ارسال شد . ";
                }
                else
                {
                    TempData["Error"] = " هنوز عضوی در خبرنامه سیستم ثبت نشده است . ";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .";
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
            Notification notification = db.Notifications.Find(id);
            try
            {
                var role = db.Roles.Where(p => p.Name == "User").FirstOrDefault();
                var query = db.Users.Include(current => current.State).Include(current => current.City).Include(current => current.Notifications).Where(p => p.Roles.Any(a => a.RoleId == role.Id) && p.IsActive == true /*&& p.PlayerId != "null" && !string.IsNullOrEmpty(p.PlayerId)*/).ToList();
                //list query = db.Users.Include(current => current.State).Include(current => current.City).Include(current => current.Notifications).Where(p => p.Roles.Any(a => a.RoleId == role.Id) && p.IsActive == true && p.PlayerId != null);
                if (query.Count() > 0)
                {
                    string[] arr = new string[query.Count()];
                    int i = 0;
                    IHtmlString htmlString = new HtmlString(notification.Text);
                    string htmlResult = Regex.Replace(htmlString.ToString(), @"<[^>]*>", string.Empty).Replace("\r", "").Replace("\n", "").Replace("&nbsp", ""); ;
                    foreach (var item in query.Where(current => current.IsActive == true).ToList())
                    {
                        item.Notifications.Add(notification);
                        db.SaveChanges();
                        if (item.PlayerId != "null" && !string.IsNullOrEmpty(item.PlayerId))
                        {
                            arr[i] = item.PlayerId;
                            i++;
                        }
                    }
                    //OneSignal Push Notification
                    PushNotification(htmlResult, arr);
                    TempData["Success"] = " اطلاعیه شما با موفقیت به  " + query.Count() + " کاربر فعال در سیستم ارسال شد . ";
                }
                else
                {
                    TempData["Error"] = " هنوز کاربری در سیستم ثبت نشده است . ";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .";
            }

            return RedirectToAction("Index");
        }

        public void PushNotification(string text, string[] UserId)
        {
            #region OneSignal
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            //Basic + User Rest Api Key
            request.Headers.Add("authorization", "Basic YzliYjM1NWUtYjU5ZC00ZmUzLTgyNDYtYWE5ZTAwYTUwODM4");

            var serializer = new JavaScriptSerializer();
            //ApplicationUser user = db.Users.Find(UserId);
            //string userPlayerId = user.PlayerId;
            //var x = new string[] { };
            //x = new string[] { userPlayerId };
            var obj = new
            {
                app_id = "348d8195-162e-4e05-8184-735e6b53a383",
                contents = new { en = text },
                include_player_ids = UserId,
                headings = new { en = "وی آی پی ایرانیان" },
                small_icon = "http://vipiranian.org/images/vip_iranian_logo.png",
                large_icon = "http://vipiranian.org/images/vip_iranian_logo.png"
            };

            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
            #endregion OneSignal
        }

        // GET: Administrator/Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Administrator/Notifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ForwardDate,Text")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.ForwardDate = DateTime.Now;
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(notification);
        }

        // GET: Administrator/Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Administrator/Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ForwardDate,Text")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(notification);
        }

        // GET: Administrator/Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Administrator/Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
            try
            {
                db.SaveChanges();
                TempData["Success"] = "اعلانیه مورد نظر از لیست حذف شد .";
            }
            catch (Exception)
            {
                TempData["Error"] = "به دلیل وجود وابستگی ها در سیستم امکان حذف این اعلانیه وجود ندارد .";
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
