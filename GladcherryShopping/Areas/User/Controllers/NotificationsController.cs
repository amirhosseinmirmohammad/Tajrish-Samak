using DataLayer.Models;
using DataLayer.ViewModels.PagerViewModel;
using GladcherryShopping.Models;
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
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Administrator/Notifications
        public ActionResult Index(int page = 1)
        {
            string UserId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(UserId))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            var UserInNotifications = db.Users.Include(current => current.Notifications).Where(current => current.Id == UserId && current.Notifications.Count > 0).FirstOrDefault();
            List<Notification> NotificationList = new List<Notification>();
            if (UserInNotifications != null)
            {
                foreach (var item in UserInNotifications.Notifications.OrderByDescending(current => current.ForwardDate))
                {
                    NotificationList.Add(item);
                }
            }
            PagerViewModels<Notification> NotificationsViewModels = new PagerViewModels<Notification>();
            NotificationsViewModels.CurrentPage = page;
            NotificationsViewModels.data = NotificationList.OrderByDescending(current => current.ForwardDate).Skip((page - 1) * 10).Take(10).ToList();
            NotificationsViewModels.TotalItemCount = NotificationList.Count();
            return View(NotificationsViewModels);
        }

    }
}