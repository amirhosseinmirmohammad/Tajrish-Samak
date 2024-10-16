//using DataLayer.Models;
//using GladcherryShopping.Models;
//using GladCherryShopping.Helpers;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using SmsIrRestful;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text.RegularExpressions;
//using System.Web;
//using System.Web.Http;
//using DataLayer.ViewModels.UserServiceViewModel;
//using Newtonsoft.Json;
//using System.IO;
//using static DataLayer.ViewModels.UserServiceViewModel.UserServiceViewModel;
//using System.Data.Entity.Core.Objects;
//using Microsoft.Ajax.Utilities;
//using System.Security.Cryptography.X509Certificates;
//using System.Net.Security;

//namespace GladcherryShopping.API_Services
//{
//    public class UserController : ApiController
//    {
//        private ApplicationDbContext db = new ApplicationDbContext();
//        string BaseUrl = "http://TajrishSamak.com/";

//        [AcceptVerbs("GET")]
//        [Route("Categories/List")]
//        public IHttpActionResult GetCategories()
//        {

//            #region Validation
//            var Categories = db.Categories.Where(current => current.SmallImage != null && current.ParentId == null).Include(current => current.SubCategories).ToList();
//            if (Categories.Count == 0)
//                return Ok(new { Status = 0, Text = "هنوز دسته بندی در سیستم وجود ندارد .", Count = 0 });
//            #endregion Validation

//            #region FindUser
//            List<dynamic> Services = new List<dynamic>();
//            foreach (var item in Categories)
//            {
//                Services.Add(new
//                {
//                    Id = item.Id,
//                    Title = item.PersianName,
//                    Image = BaseUrl + item.SmallImage,
//                    SubCount = item.SubCategories.Count
//                });
//            }
//            return Ok(new { Status = 1, Object = Services, Count = Services.Count });
//            #endregion FindUser
//        }

//        [AcceptVerbs("GET")]
//        [Route("SubCategories/List")]
//        public IHttpActionResult GetSubCategories(int? ParentId)
//        {
//            #region Validation
//            if (ParentId == null)
//                return Ok(new { Status = 0, Text = "لطفا شناسه دسته بندی والد را وارد نمایید .", Count = 0 });
//            var Categories = db.Categories.Where(current => current.SmallImage != null && current.ParentId == ParentId).Include(current => current.SubCategories).ToList();
//            if (Categories.Count == 0)
//                return Ok(new { Status = 0, Text = "هنوز دسته بندی زیر شاخه ای وجود ندارد .", Count = 0 });
//            #endregion Validation

//            #region FindUser
//            List<dynamic> Services = new List<dynamic>();
//            foreach (var item in Categories)
//            {
//                Services.Add(new
//                {
//                    Id = item.Id,
//                    Title = item.PersianName,
//                    Image = BaseUrl + item.SmallImage,
//                    SubCount = item.SubCategories.Count
//                });
//            }
//            return Ok(new { Status = 1, Object = Services, Count = Services.Count });
//            #endregion FindUser
//        }

//        [AcceptVerbs("GET")]
//        [Route("LatestProducts/List")]
//        public IHttpActionResult GetLatestProducts()
//        {

//            #region Intialize
//            var Products = db.Products.OrderByDescending(current => current.CreateDate).Where(current => current.AppSmallImage != null).Take(8);
//            #endregion Intialize

//            #region FindUser
//            List<dynamic> productList = new List<dynamic>();
//            foreach (var item in Products)
//            {
//                int pricewithdiscount = item.UnitPrice - (item.UnitPrice) * (item.DiscountPercent) / 100;
//                if (item.Stock > 0)
//                {
//                    productList.Add(new
//                    {
//                        Id = item.Id,
//                        Name = item.PersianName,
//                        Price = item.UnitPrice,
//                        DiscountPrice = pricewithdiscount,
//                        Available = true,
//                        Unit = "ریال",
//                        Image = BaseUrl + item.AppSmallImage ?? ""
//                    });
//                }
//                if (item.Stock == 0)
//                {
//                    productList.Add(new
//                    {
//                        Id = item.Id,
//                        Name = item.PersianName,
//                        Price = item.UnitPrice,
//                        DiscountPrice = pricewithdiscount,
//                        Available = false,
//                        Unit = "ریال",
//                        Image = BaseUrl + item.AppSmallImage ?? ""
//                    });
//                }

//            }
//            return Ok(new { Status = 1, Object = productList, Count = productList.Count });
//            #endregion FindUser
//        }

//        //Main Api
//        [AcceptVerbs("GET")]
//        [Route("Categories/GetServices")]
//        public IHttpActionResult GetServices(string UserId, string GuId)
//        {

//            #region Validation
//            var Categories = db.Categories.Where(current => current.SmallImage != null && current.ParentId == null).OrderByDescending(current => current.PersianName).ToList();
//            if (Categories.Count == 0)
//                return Ok(new { Status = 0, Text = "هنوز دسته بندی اصلی در سیستم وجود ندارد .", Count = 0 });
//            #endregion Validation

//            #region FindUser
//            List<dynamic> Services = new List<dynamic>();
//            foreach (var item in Categories)
//            {
//                Services.Add(new
//                {
//                    id = item.Id,
//                    title = item.PersianName,
//                    image = BaseUrl + item.SmallImage ?? "images/pransa_logo_web.png",
//                });
//            }
//            return Ok(new { Status = 1, Text = "", Categories = Services, Count = Services.Count, BasketCount = BasketCount(UserId, GuId) });
//            #endregion FindUser
//        }

//        [AcceptVerbs("POST")]
//        [Route("VerificationCode/Generate")]
//        public IHttpActionResult GenerateVerificationCode(HttpRequestMessage requset)
//        {

//            #region Request
//            var body = requset.Content;
//            var jsonContent = body.ReadAsStringAsync().Result;
//            UserServiceViewModel.RegisterUser requestParams = JsonConvert.DeserializeObject<UserServiceViewModel.RegisterUser>(jsonContent);
//            #endregion Request

//            #region Validation
//            if (string.IsNullOrWhiteSpace(requestParams.PhoneNumber))
//                return Ok(new { Status = 0, Text = "لطفا شماره موبایل خود را وارد نمایید ." });
//            if (!Regex.Match(requestParams.PhoneNumber, @"\b\d{4}[\s-.]?\d{3}[\s-.]?\d{4}\b").Success)
//                return Ok(new { Status = 0, Text = "لطفا شماره موبایل معتبری را وارد نمایید ." });
//            City defaultCity = db.Cities.FirstOrDefault();
//            State defaultState = db.States.FirstOrDefault();
//            if (defaultCity == null || defaultState == null)
//                return Ok(new { Status = 0, Text = "تنظیمات اپلیکیشن با خطا رو به رو است ." });
//            IQueryable<ApplicationUser> users = db.Users.Where(current => current.Mobile == requestParams.PhoneNumber);
//            if (users.Count() > 0)
//                return Ok(new { Status = 0, Text = "این شماره قبلا ثبت نام کرده است .", UserId = users.FirstOrDefault().Id });
//            var FriendUser = new ApplicationUser();
//            if (!string.IsNullOrEmpty(requestParams.IntroCode))
//            {
//                FriendUser = db.Users.Where(current => current.AccessCode == requestParams.IntroCode).FirstOrDefault();
//                if (FriendUser == null)
//                    return Ok(new { Status = 0, Text = "کاربری با کد معرف شما پیدا نشد .", UserId = "" });
//            }
//            #endregion Validation

//            #region SMS
//            Application application = db.Applications.FirstOrDefault();
//            Random random = new Random();
//            var randomNumber = random.Next(10000, 99999);
//            if (application != null && application.FromNumber != null)
//            {

//                //sms.ir
//                string SMSDescription = "کد تایید شما در کلینیک شنوایی و سمعک شکوه تجریش : " + randomNumber;
//                bool smsResult = UltraFastSms(requestParams.PhoneNumber, randomNumber.ToString());
//                if (smsResult == false)
//                    return Ok(new { Status = 0, Text = "سامانه پیامکی با خطا رو به رو است لطفا مجدد تلاش فرمایید ." });

//            }
//            else
//            {
//                return Ok(new { Status = 0, Text = "تنظیمات کلی اپلیکیشن هنوز تعیین نشده است ." });
//            }
//            #endregion SMS

//            #region HashPassword
//            PasswordHasher hashPassword = new PasswordHasher();
//            var hashedPassword = hashPassword.HashPassword(requestParams.PhoneNumber);
//            var security = Guid.NewGuid().ToString();
//            //نام کاربری و رمز عبور کاربر همان شماره موبایل است .
//            #endregion Hash Password

//            #region NewUser
//            ApplicationUser user = new ApplicationUser
//            {
//                Mobile = requestParams.PhoneNumber,
//                UserName = requestParams.PhoneNumber,
//                UserScore = 0,
//                AccessCode = "TajrishSamak_" + randomNumber,
//                //AccessCode = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8),
//                Credit = 0,
//                IsActive = true,
//                BirthDate = DateTime.Now,
//                CityId = defaultCity.Id,
//                StateId = defaultState.Id,
//                SecurityStamp = security,
//                PasswordHash = hashedPassword,
//                //Mobile = موبایل کاربر
//                //PhoneNumber = تلفن ثابت کاربر
//                //تایید شماره کاربر
//                VerificationCode = FunctionsHelper.Encrypt(randomNumber.ToString(), "Gladcherry"),
//                PhoneNumberConfirmed = false,
//                RegistrationDate = DateTime.Now,
//                FirstName = requestParams.FirstName,
//                LastName = requestParams.LastName
//            };
//            #endregion NewUser

//            #region Create
//            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
//            IdentityResult result;
//            try
//            {
//                result = UserManager.Create(user);
//                #region IntroCode
//                if (!string.IsNullOrEmpty(requestParams.IntroCode) && FriendUser != null)
//                {
//                    //افزایش امتیاز و اعتبار معرف
//                    if (application != null && application.IntroPercent > 0)
//                    {
//                        FriendUser.Credit += application.IntroPercent;
//                    }
//                    if (application != null && application.IntroScore > 0)
//                    {
//                        FriendUser.UserScore += application.IntroScore;
//                    }
//                    db.Entry(FriendUser).State = EntityState.Modified;
//                    db.SaveChanges();
//                    //کد معرفی کاربر جدید
//                    user.IntroCode = FriendUser.AccessCode;
//                    db.Entry(user).State = EntityState.Modified;
//                    db.SaveChanges();
//                }
//                #endregion IntroCode
//            }
//            catch (Exception)
//            {
//                return Ok(new
//                {
//                    Status = 0,
//                    Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .",
//                    UserId = users.FirstOrDefault().Id
//                });
//            }
//            if (!result.Succeeded)
//                return Ok(new { Status = 0, Text = "متاسفانه ثبت نام شما انجام نشد لطفا مجدد تلاش فرمایید .", UserId = users.FirstOrDefault().Id, Errors = result.Errors, });
//            UserManager.AddToRole(user.Id, "User");
//            if (application != null && application.SuccessfullRegisterationText != null)
//            {
//                IHtmlString htmlString = new HtmlString(application.SuccessfullRegisterationText);
//                string htmlResult = Regex.Replace(htmlString.ToString(), @"<[^>]*>", string.Empty).Replace("\r", "").Replace("\n", "").Replace("&nbsp", ""); ;
//                return Ok(new { Status = 1, Text = htmlResult, UserId = user.Id/*, Code = randomNumber.ToString()*/ });
//            }
//            else
//                //کد ارسالی از متد حذف شود .
//                return Ok(new { Status = 1, Text = "ثبت نام شما با موفقیت انجام شد .", UserId = user.Id/*, Code = randomNumber.ToString()*/ });
//            #endregion Create
//        }

//        [AcceptVerbs("GET")]
//        [Route("VerificationCode/Confirmation")]
//        public IHttpActionResult CodeConfirmation(string PhoneNumber, string Code, string GuId, bool IsRegistered)
//        {

//            #region Validation
//            if (string.IsNullOrWhiteSpace(PhoneNumber))
//                return Ok(new { Status = 0, Text = "لطفا شماره موبایل خود را وارد نمایید ." });
//            if (string.IsNullOrWhiteSpace(Code))
//                return Ok(new { Status = 0, Text = "لطفا کد را وارد نمایید ." });
//            if (!Regex.Match(PhoneNumber, @"\b\d{4}[\s-.]?\d{3}[\s-.]?\d{4}\b").Success)
//                return Ok(new { Status = 0, Text = "لطفا شماره موبایل معتبری را وارد نمایید ." });

//            IQueryable<ApplicationUser> users = db.Users.Where(current => current.Mobile == PhoneNumber);
//            if (users.Count() <= 0)
//                return Ok(new { Status = 0, Text = "شماره شما در سیستم ثبت نشده است ." });
//            #endregion Validation

//            #region Login
//            string UserVerificationCode = FunctionsHelper.Decrypt(users.FirstOrDefault().VerificationCode, "Gladcherry");
//            if (UserVerificationCode != null)
//            {
//                if (UserVerificationCode != Code)
//                    return Ok(new { Status = 0, Text = "کد وارد شده صحیح نمیباشد .", UserId = users.FirstOrDefault().Id });
//                if (UserVerificationCode == Code)
//                {
//                    users.FirstOrDefault().PhoneNumberConfirmed = true;
//                    db.Entry(users.FirstOrDefault()).State = EntityState.Modified;
//                    db.SaveChanges();

//                    #region Basket
//                    //بعد از ثبت نام 
//                    if (IsRegistered == true)
//                    {
//                        Basket basket = db.Baskets.Where(current => current.GuId == GuId && current.GuId != null && current.GuId != "").OrderByDescending(current => current.CreateDate).FirstOrDefault();
//                        DateTime now = new DateTime();
//                        if (basket != null && now < basket.CreateDate.AddHours(12))
//                        {
//                            basket.GuId = string.Empty;
//                            basket.UserId = users.FirstOrDefault().Id;
//                            db.Entry(basket).State = EntityState.Modified;
//                            db.SaveChanges();
//                        }
//                    }
//                    //بعد از ورود 
//                    else
//                    {
//                        Basket ubasket = db.Baskets.Where(current => current.UserId == users.FirstOrDefault().Id && current.UserId != null && current.UserId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//                        Basket gbasket = db.Baskets.Where(current => current.GuId == GuId && current.GuId != null && current.GuId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//                        DateTime now = new DateTime();
//                        if (gbasket != null && now < gbasket.CreateDate.AddHours(12))
//                        {
//                            if (ubasket != null && now < ubasket.CreateDate.AddHours(12))
//                            {
//                                //ادغام کردن
//                                foreach (var item in gbasket.ProductInBaskets.ToList())
//                                {
//                                    item.BasketId = ubasket.Id;
//                                    db.Entry(item).State = EntityState.Modified;
//                                    db.SaveChanges();
//                                }
//                            }
//                        }
//                    }
//                    #endregion Basket

//                    Application application = db.Applications.FirstOrDefault();
//                    if (application != null && application.VerifyPhoneNumberText != null)
//                    {
//                        IHtmlString htmlString = new HtmlString(application.VerifyPhoneNumberText);
//                        string htmlResult = Regex.Replace(htmlString.ToString(), @"<[^>]*>", string.Empty).Replace("\r", "").Replace("\n", "").Replace("&nbsp", ""); ;
//                        return Ok(new { Status = 1, Text = htmlResult, UserId = users.FirstOrDefault().Id });
//                    }
//                    else
//                        return Ok(new { Status = 1, Text = "شماره شما با موفقیت تایید شد .", UserId = users.FirstOrDefault().Id });
//                }
//            }
//            else
//                return Ok(new { Status = 0, Text = "کد کاربر خالی است .", UserId = users.FirstOrDefault().Id });
//            #endregion Login

//            return Ok(new { Status = 0, Text = "لطفا مجدد تلاش فرمایید .", UserId = users.FirstOrDefault().Id });
//        }

//        [AcceptVerbs("GET")]
//        [Route("Login/Authentication")]
//        public IHttpActionResult Login(string PhoneNumber)
//        {
//            #region Validation
//            if (string.IsNullOrWhiteSpace(PhoneNumber))
//                return Ok(new { Status = 0, Text = "لطفا شماره موبایل خود را وارد نمایید ." });
//            if (!Regex.Match(PhoneNumber, @"\b\d{4}[\s-.]?\d{3}[\s-.]?\d{4}\b").Success)
//                return Ok(new { Status = 0, Text = "لطفا شماره موبایل معتبری را وارد نمایید ." });
//            IQueryable<ApplicationUser> users = db.Users.Where(current => current.Mobile == PhoneNumber);
//            if (users.Count() <= 0)
//                return Ok(new { Status = 0, Text = "شماره شما در سیستم ثبت نشده است ." });
//            if (users.Count() > 0 && users.FirstOrDefault().IsActive == false)
//                return Ok(new { Status = 0, Text = "حساب کاربری شما غیر فعال شده است لطفا با پشتیبانی تماس حاصل فرمایید ." });
//            #endregion Validation

//            #region SMS
//            Application application = db.Applications.FirstOrDefault();
//            Random random = new Random();
//            var randomNumber = random.Next(10000, 99999);
//            if (application != null && application.FromNumber != null)
//            {

//                //sms.ir
//                string SMSDescription = "کد تایید شما در کلینیک شنوایی و سمعک شکوه تجریش : " + randomNumber;
//                bool smsResult = UltraFastSms(PhoneNumber, randomNumber.ToString());
//                //Task b = sendSMSAsync(application.FromNumber, PhoneNumber, SMSDescription);
//                if (smsResult == false)
//                    return Ok(new { Status = 0, Text = "سامانه پیامکی با خطا رو به رو است لطفا مجدد تلاش فرمایید ." });
//                #region EditUserCode
//                users.FirstOrDefault().VerificationCode = FunctionsHelper.Encrypt(randomNumber.ToString(), "Gladcherry");
//                db.Entry(users.FirstOrDefault()).State = EntityState.Modified;
//                db.SaveChanges();
//                #endregion EditUserCodde
//                //await b;
//            }
//            else
//            {
//                return Ok(new { Status = 0, Text = "تنظیمات کلی اپلیکیشن هنوز تعیین نشده است ." });
//            }
//            #endregion SMS

//            //کد ارسالی از متد حذف شود .
//            return Ok(new { Status = 1, Text = "ورود اولیه به حساب کاربری با موفقیت انجام شد .", UserId = users.FirstOrDefault().Id, Code = randomNumber.ToString() });
//        }

//        [AcceptVerbs("GET")]
//        [Route("VerificationCode/Resend")]
//        public IHttpActionResult ResendVerificationCode(string PhoneNumber)
//        {

//            #region Validation
//            if (string.IsNullOrWhiteSpace(PhoneNumber))
//                return Ok(new { Status = 0, Text = "لطفا شماره موبایل خود را وارد نمایید ." });
//            if (!Regex.Match(PhoneNumber, @"\b\d{4}[\s-.]?\d{3}[\s-.]?\d{4}\b").Success)
//                return Ok(new { Status = 0, Text = "لطفا شماره موبایل معتبری را وارد نمایید ." });
//            IQueryable<ApplicationUser> users = db.Users.Where(current => current.Mobile == PhoneNumber);
//            if (users.Count() <= 0)
//                return Ok(new { Status = 0, Text = "شماره شما در سیستم ثبت نشده است ." });
//            if (users.Count() > 0 && users.FirstOrDefault().IsActive == false)
//                return Ok(new { Status = 0, Text = "حساب کاربری شما غیر فعال شده است لطفا با پشتیبانی تماس حاصل فرمایید ." });
//            #endregion Validation

//            #region SMS
//            Application application = db.Applications.FirstOrDefault();
//            Random random = new Random();
//            var randomNumber = random.Next(10000, 99999);
//            if (application != null && application.FromNumber != null)
//            {


//                //sms.ir
//                string SMSDescription = "کد مجدد تایید شما در کلینیک شنوایی و سمعک شکوه تجریش : " + randomNumber;
//                bool smsResult = UltraFastSms(PhoneNumber, randomNumber.ToString());
//                if (smsResult == false)
//                    return Ok(new { Status = 0, Text = "سامانه پیامکی با خطا رو به رو است لطفا مجدد تلاش فرمایید ." });


//                #region EditUserCode
//                users.FirstOrDefault().VerificationCode = FunctionsHelper.Encrypt(randomNumber.ToString(), "Gladcherry");
//                db.Entry(users.FirstOrDefault()).State = EntityState.Modified;
//                db.SaveChanges();
//                #endregion EditUserCodde

//            }
//            else
//            {
//                return Ok(new { Status = 0, Text = "تنظیمات کلی اپلیکیشن هنوز تعیین نشده است ." });
//            }
//            #endregion SMS

//            //کد ارسالی از متد حذف شود .
//            return Ok(new { Status = 1, Text = "ورود شما به حساب کاربری با موفقیت تکمیل شد .", UserId = users.FirstOrDefault().Id, Code = randomNumber.ToString() });
//        }

//        [AcceptVerbs("GET")]
//        [Route("UserInformation/GetUser")]
//        public IHttpActionResult UserInformation(string UserId, string GuId)
//        {

//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId))
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            #endregion Validation

//            #region FindUser
//            //UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
//            //ApplicationUser User = UserManager.FindById(UserId);
//            ApplicationUser User = db.Users.Where(current => current.Id == UserId).Include(current => current.City).Include(current => current.State).FirstOrDefault();
//            if (User == null)
//                return Ok(" کاربری با این شماره یافت نشد . ");
//            Application app = db.Applications.FirstOrDefault();
//            if (User.State != null && User.City != null)
//            {
//                return Ok(new { FirstName = User.FirstName, LastName = User.LastName, ProfileImage = User.ProfileImage, Credit = User.Credit, AccessCode = User.AccessCode, Email = User.Email, IntroCode = User.IntroCode, Phone = User.PhoneNumber, Mobile = User.Mobile, Address = User.AddressLine, StateId = User.StateId, CityId = User.CityId, Gender = User.getGender, GenderId = User.Gender, Birhtdate = User.BirthDate.Value.Date.Year + "-" + User.BirthDate.Value.Date.Month + "-" + User.BirthDate.Value.Date.Day, cityName = User.City.Name ?? "", stateName = User.State.Name ?? "", Menu = "https://homekar.com/images/Menu.jpg", EditProfile = "https://homekar.com/images/EditProfile.jpg", Messages = "https://homekar.com/images/Messages.png", AboutUs = "https://homekar.com/images/AboutUs.png", Logout = "https://homekar.com/images/Logout.png", Payments = "https://homekar.com/images/Payments.png", Profile = "https://homekar.com/images/Profile.png", Rate = "https://homekar.com/images/Rate.png", Rules = "https://homekar.com/images/Rules.png", SelectedAdresses = "https://homekar.com/images/SelectedAdresses.png", Support = "https://homekar.com/images/Support.png", BasketCount = BasketCount(UserId, GuId) });
//            }
//            else
//            {
//                return Ok(new { FirstName = User.FirstName, LastName = User.LastName, ProfileImage = User.ProfileImage, Credit = User.Credit, AccessCode = User.AccessCode, Email = User.Email, IntroCode = User.IntroCode, Phone = User.PhoneNumber, Mobile = User.Mobile, Address = User.AddressLine, StateId = 1, CityId = 1, Gender = User.getGender, GenderId = User.Gender, Birhtdate = User.BirthDate.Value.Date.Year + "-" + User.BirthDate.Value.Date.Month + "-" + User.BirthDate.Value.Date.Day, cityName = "", stateName = "", Menu = "https://homekar.com/images/Menu.jpg", EditProfile = "https://homekar.com/images/EditProfile.jpg", Messages = "https://homekar.com/images/Messages.png", AboutUs = "https://homekar.com/images/AboutUs.png", Logout = "https://homekar.com/images/Logout.png", Payments = "https://homekar.com/images/Payments.png", Profile = "https://homekar.com/images/Profile.png", Rate = "https://homekar.com/images/Rate.png", Rules = "https://homekar.com/images/Rules.png", SelectedAdresses = "https://homekar.com/images/SelectedAdresses.png", Support = "https://homekar.com/images/Support.png", BasketCount = BasketCount(UserId, GuId) });
//            }
//            #endregion FindUser
//        }

//        [AcceptVerbs("POST", "PUT")]
//        [Route("UserInformation/EditUser")]
//        public IHttpActionResult EditUser(HttpRequestMessage requset)
//        {

//            #region Request
//            var body = requset.Content;
//            var jsonContent = body.ReadAsStringAsync().Result;
//            EditUser requestParams = JsonConvert.DeserializeObject<EditUser>(jsonContent);
//            #endregion Request

//            #region Validation
//            if (string.IsNullOrWhiteSpace(requestParams.UserId))
//                return Ok(new { Status = 0, Text = " لطفا شناسه کاربری را وارد نمایید . " });
//            if (requestParams.Phone != null && requestParams.Phone.Length != 11)
//                return Ok(new { Status = 0, Text = " شماره ثابت وارد شده معتبر نمیباشد . " });
//            if (requestParams.StateId == null)
//                return Ok(new { Status = 0, Text = " لطفا استان کاربر را وارد نمایید . " });
//            if (requestParams.CityId == null)
//                return Ok(new { Status = 0, Text = " لطفا شهرستان کاربر را وارد نمایید . " });
//            #endregion Validation

//            #region FindUser
//            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
//            ApplicationUser User = UserManager.FindById(requestParams.UserId);
//            if (User == null)
//                return Ok(new { Status = 0, Text = " کاربری با این شماره یافت نشد . " });
//            #endregion FindUser

//            #region Edit
//            ApplicationUser EdittedUser = db.Users.Find(User.Id);
//            EdittedUser.FirstName = requestParams.FirstName;
//            EdittedUser.LastName = requestParams.LastName;
//            EdittedUser.PhoneNumber = requestParams.Phone;
//            EdittedUser.AddressLine = requestParams.Address;
//            EdittedUser.StateId = (int)requestParams.StateId;
//            EdittedUser.CityId = (int)requestParams.CityId;
//            if (requestParams.Gender != null)
//                EdittedUser.Gender = (byte)requestParams.Gender;
//            db.Entry(EdittedUser).State = EntityState.Modified;

//            try
//            {
//                db.SaveChanges();
//                return Ok(new { Status = 1, Text = " کاربر گرامی اطلاعات شما با موفقیت ویرایش شد . " });
//            }
//            catch (Exception)
//            {
//                return Ok(new { Status = 0, Text = " متاسفانه ویرایش انجام نشد مجدد تلاش فرمایید . " });
//            }
//            #endregion Edit
//        }

//        [AcceptVerbs("GET")]
//        [Route("UserInformation/UserPermision")]
//        public IHttpActionResult UserPermision(string UserId)
//        {

//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId))
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            #endregion Validation

//            #region FindUser
//            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
//            ApplicationUser User = UserManager.FindById(UserId);
//            if (User == null)
//                return Ok(" کاربری با این شماره یافت نشد . ");
//            if (User.FirstName == null || User.LastName == null)
//            {
//                return Ok(new { Status = 0, Text = "لطفا برای درخواست خدمات ابتدا نام و نام خانوادگی خود را در قسمت ویرایش پروفایل تکمیل نمایید ." });
//            }
//            else
//            {
//                return Ok(new { Status = 1, Text = "کاربر گرامی شما مجوز درخواست خدمات را دارید ." });
//            }
//            #endregion FindUser
//        }

//        [AcceptVerbs("GET")]
//        [Route("UserInformation/Notification")]
//        public IHttpActionResult GetNotification(string UserId, string GuId)
//        {

//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId))
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            var UserInNotifications = db.Users.Include(current => current.Notifications).Where(current => current.Id == UserId && current.Notifications.Count > 0).FirstOrDefault();
//            if (UserInNotifications == null)
//                return Ok(new { Status = 0, Text = "پیامی برای این کاربر وجود ندارد .", NotificationList = 0 });
//            #endregion Validation

//            #region FindNotification
//            List<dynamic> NotificationList = new List<dynamic>();
//            foreach (var item in UserInNotifications.Notifications.OrderByDescending(current => current.ForwardDate).Take(20))
//            {
//                IHtmlString htmlString = new HtmlString(item.Text);
//                string htmlResult = Regex.Replace(htmlString.ToString(), @"<[^>]*>", string.Empty).Replace("\r", "").Replace("\n", "").Replace("&nbsp", ""); ;
//                NotificationList.Add(new
//                {
//                    Id = item.Id,
//                    DateTime = FunctionsHelper.GetPersianDateTime(item.ForwardDate, true, true),
//                    Text = htmlResult
//                });
//            }
//            return Ok(new { Status = 1, Object = NotificationList, NotificationCount = NotificationList.Count, BasketCount = BasketCount(UserId, GuId) });
//            #endregion FindNotification
//        }

//        [AcceptVerbs("GET")]
//        [Route("UserInformation/Payment")]
//        public IHttpActionResult GetPayments(string UserId, string GuId)
//        {

//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId))
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            var User = db.Users.Find(UserId);
//            if (User == null)
//                return Ok(new { Status = 0, Text = "کاربر مورد نظر پیدا نشد ." });
//            var Payments = db.Payments.Where(current => current.UserId == UserId).OrderByDescending(current => current.CreateDate).Take(20);
//            if (Payments.Count() == 0)
//                return Ok(new { Status = 0, Text = "هنوز سند مالی برای این کاربر ثبت نشده است .", PaymentCount = 0 });
//            #endregion Validation

//            #region FindPayments

//            List<dynamic> PaymentList = new List<dynamic>();
//            foreach (var item in Payments)
//            {
//                if (item.ActionType == 1)
//                {
//                    PaymentList.Add(new
//                    {
//                        PaymentId = item.Id,
//                        Title = item.getActionType,
//                        Amount = -item.Amount,
//                        Description = item.Description,
//                        DateTime = FunctionsHelper.GetPersianDateTime(item.CreateDate, true, true)
//                    });
//                }
//                if (item.ActionType == 2)
//                {
//                    PaymentList.Add(new
//                    {
//                        PaymentId = item.Id,
//                        Title = item.getActionType,
//                        Amount = item.Amount,
//                        Description = item.Description,
//                        DateTime = FunctionsHelper.GetPersianDateTime(item.CreateDate, true, true)
//                    });
//                }

//            }
//            return Ok(new { Status = 1, Object = PaymentList, PaymentCount = PaymentList.Count, BasketCount = BasketCount(UserId, GuId) });
//            #endregion FindPayments

//        }

//        [AcceptVerbs("GET")]
//        [Route("GIS/GetStates")]
//        public IHttpActionResult GetState()
//        {

//            #region Validation
//            var States = db.States;
//            if (States.Count() == 0)
//                return Ok(new { Status = 0, Text = "هنوز استانی وجود ندارد .", StatesCount = 0 });
//            #endregion Validation

//            var jsonResult = States.Select(result => new { id = result.Id, name = result.Name });
//            return Ok(new { Status = 1, Object = jsonResult, StatesCount = jsonResult.Count() });
//        }

//        [AcceptVerbs("GET")]
//        [Route("GIS/GetCities")]
//        public IHttpActionResult GetCity(int? StateId)
//        {

//            #region Validation
//            if (StateId == null)
//                return Ok(new { Status = 0, Text = "لطفا شناسه استان را وارد نمایید ." });
//            var Cities = db.Cities.Where(current => current.StateId == StateId);
//            if (Cities.Count() == 0)
//                return Ok(new { Status = 0, Text = "هنوز شهری در این استان وجود ندارد .", CitiesCount = 0 });
//            #endregion Validation

//            var jsonResult = Cities.Select(result => new { id = result.Id, name = result.Name });
//            return Ok(new { Status = 1, Object = jsonResult, CitiesCount = jsonResult.Count() });
//        }

//        [AcceptVerbs("POST")]
//        [Route("UserInformation/UploadActivityImage")]
//        public IHttpActionResult OrderPost()
//        {
//            string imgPath = string.Empty;
//            try
//            {
//                var httpRequest = HttpContext.Current.Request;
//                var x = httpRequest.Params["image"];
//                string path = HttpContext.Current.Server.MapPath("~/Content/Activities"); //Path
//                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
//                string imageName = Path.GetRandomFileName() + ".jpg";
//                imgPath = Path.Combine(path, imageName);
//                byte[] imageBytes = Convert.FromBase64String(x);
//                System.IO.File.WriteAllBytes(imgPath, imageBytes);
//                return Ok(new { Status = 1, path = "http://TajrishSamak.com/Content/Activities/" + imageName, Text = "عکس مورد نظر با موفقیت آپلود شد ." });
//            }
//            catch (Exception)
//            {
//                return Ok(new { Status = 0, path = string.Empty, Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید ." });
//            }
//        }

//        [AcceptVerbs("POST", "GET")]
//        [Route("UserInformation/ProfileImage")]
//        public IHttpActionResult UserPost(string UserId)
//        {
//            string imgPath = string.Empty;
//            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
//            ApplicationUser User = UserManager.FindById(UserId);
//            if (User == null)
//                return Ok(new { Status = 0, path = string.Empty, Text = " کاربری با این شماره یافت نشد . " });
//            try
//            {
//                var httpRequest = HttpContext.Current.Request;
//                var x = httpRequest.Params["image"];
//                string path = HttpContext.Current.Server.MapPath("~/Content/UserProfiles"); //Path
//                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
//                string imageName = Path.GetRandomFileName() + ".jpg";
//                imgPath = Path.Combine(path, imageName);
//                byte[] imageBytes = Convert.FromBase64String(x);
//                System.IO.File.WriteAllBytes(imgPath, imageBytes);
//                User.ProfileImage = "http://TajrishSamak.com/Content/UserProfiles/" + imageName;
//                db.Entry(User).State = EntityState.Modified;
//                db.SaveChanges();
//                return Ok(new { Status = 1, path = "http://TajrishSamak.com/Content/UserProfiles/" + imageName, Text = "عکس مورد نظر با موفقیت آپلود شد ." });
//            }
//            catch (Exception)
//            {
//                return Ok(new { Status = 0, path = string.Empty, Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید ." });
//            }
//        }

//        [AcceptVerbs("GET")]
//        [Route("SelectedAddress/List")]
//        public IHttpActionResult GetAddresses(string UserId, string GuId)
//        {

//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId))
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            var User = db.Users.Find(UserId);
//            if (User == null)
//                return Ok(new { Status = 0, Text = "کاربر مورد نظر پیدا نشد ." });
//            var addresses = db.Addresses.Where(current => current.UserId == UserId);
//            if (addresses.Count() == 0)
//                return Ok(new { Status = 0, Text = "آدرس منتخبی برای این کاربر ثبت نشده است .", AddressCount = 0 });
//            #endregion Validation

//            #region FindLocation
//            //var add = addresses.FirstOrDefault();
//            //IGeocoder geocoder = new GoogleGeocoder();
//            //IEnumerable<Geocoding.Address> addressesMap = geocoder.ReverseGeocode(Convert.ToDouble(add.latitude), Convert.ToDouble(add.longitude));
//            #endregion FindLocation

//            #region FindAddress

//            List<dynamic> AddressList = new List<dynamic>();
//            foreach (var item in addresses)
//            {
//                string imagepath = "https://maps.googleapis.com/maps/api/staticmap?center=" + item.latitude + "," + item.longitude + "&zoom=16&size=400x300&markers=color:pink%7Clabel:S%7C" + item.latitude + "," + item.longitude + "&key=AIzaSyBu-916DdpKAjTmJNIgngS6HL_kDIKU0aU";
//                AddressList.Add(new
//                {
//                    AddressId = item.Id,
//                    Lat = item.latitude ?? "",
//                    Long = item.longitude ?? "",
//                    Plaque = item.plaque,
//                    Unit = item.unit,
//                    Title = item.Title,
//                    FullAddress = item.Addrress.ToString(),
//                    //MapAddress = addressesMap.FirstOrDefault().FormattedAddress ?? "NULL",
//                    MapImage = imagepath ?? ""
//                });
//            }
//            return Ok(new { Status = 1, Object = AddressList, AddressCount = AddressList.Count, BasketCount = BasketCount(UserId, GuId) });
//            #endregion FindAddress
//        }

//        [AcceptVerbs("POST")]
//        [Route("SelectedAddress/Insert")]
//        public IHttpActionResult InsertAddress(HttpRequestMessage requset)
//        {

//            #region Request
//            var body = requset.Content;
//            var jsonContent = body.ReadAsStringAsync().Result;
//            InsertAddress requestParams = JsonConvert.DeserializeObject<InsertAddress>(jsonContent);
//            #endregion Request

//            #region Validation
//            if (string.IsNullOrWhiteSpace(requestParams.UserId))
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            //if (string.IsNullOrWhiteSpace(requestParams.Title))
//            //    return Ok(new { Status = 0, Text = "لطفا نوع ساختمان را تعیین نمایید ." });
//            if (string.IsNullOrWhiteSpace(requestParams.latitude) || string.IsNullOrWhiteSpace(requestParams.longitude))
//                return Ok(new { Status = 0, Text = "لطفا موقعیت جغرافیایی را تعیین نمایید ." });
//            #endregion Validation

//            #region NewAddress
//            DataLayer.Models.Address address = new DataLayer.Models.Address();
//            address.IsConfirm = true;
//            address.Addrress = requestParams.Address;
//            address.latitude = requestParams.latitude;
//            address.longitude = requestParams.longitude;
//            address.plaque = requestParams.plaque;
//            address.unit = requestParams.unit;
//            address.Title = "منتخب";
//            address.UserId = requestParams.UserId;
//            db.Addresses.Add(address);
//            try
//            {
//                db.SaveChanges();
//                return Ok(new { Status = 1, Text = "آدرس شما با موفقیت ثبت شد ." });
//            }
//            catch (Exception)
//            {
//                return Ok(new { Status = 0, Text = "خطایی رخ داده است مجدد تلاش فرمایید ." });
//            }
//            #endregion NewAddress
//        }

//        [AcceptVerbs("PUT")]
//        [Route("SelectedAddress/Edit")]
//        public IHttpActionResult EditAddress(HttpRequestMessage requset)
//        {

//            #region Request
//            var body = requset.Content;
//            var jsonContent = body.ReadAsStringAsync().Result;
//            EditAddress requestParams = JsonConvert.DeserializeObject<EditAddress>(jsonContent);
//            #endregion Request

//            #region Validation
//            DataLayer.Models.Address address = db.Addresses.Find(requestParams.AddressId);
//            if (address == null)
//                return Ok(new { Status = 0, Text = "آدرس مورد نظر پیدا نشد ." });
//            if (string.IsNullOrWhiteSpace(requestParams.Type))
//                return Ok(new { Status = 0, Text = "لطفا نوع ساختمان را تعیین نمایید ." });
//            if (string.IsNullOrWhiteSpace(requestParams.latitude) || string.IsNullOrWhiteSpace(requestParams.longitude))
//                return Ok(new { Status = 0, Text = "لطفا موقعیت جغرافیایی را تعیین نمایید ." });
//            #endregion Validation

//            #region NewAddress
//            address.Addrress = requestParams.Address;
//            address.latitude = requestParams.latitude;
//            address.longitude = requestParams.longitude;
//            address.plaque = requestParams.plaque;
//            address.unit = requestParams.unit;
//            address.Type = requestParams.Type;
//            db.Entry(address).State = EntityState.Modified;
//            try
//            {
//                db.SaveChanges();
//                return Ok(new { Status = 1, Text = "آدرس شما با موفقیت ویرایش شد ." });
//            }
//            catch (Exception)
//            {
//                return Ok(new { Status = 0, Text = "خطایی رخ داده است مجدد تلاش فرمایید ." });
//            }
//            #endregion NewAddress
//        }

//        [AcceptVerbs("GET")]
//        [Route("SelectedAddress/Delete")]
//        public IHttpActionResult DeleteAddress(int? Id)
//        {

//            #region Validation
//            if (Id == null)
//                return Ok(new { Status = 0, Text = "لطفا شناسه آدرس را وارد نمایید ." });
//            DataLayer.Models.Address address = db.Addresses.Find(Id);
//            if (address == null)
//                return Ok(new { Status = 0, Text = "آدرس مورد نظر پیدا نشد ." });
//            #endregion Validation

//            #region Delete
//            db.Addresses.Remove(address);
//            try
//            {
//                db.SaveChanges();
//                return Ok(new { Status = 1, Text = "آدرس مورد نظر حذف شد ." });
//            }
//            catch (Exception)
//            {
//                return Ok(new { Status = 0, Text = "امکان حذف این آدرس برای شما وجود ندارد ." });
//            }
//            #endregion Delete
//        }

//        [AcceptVerbs("GET")]
//        [Route("Contractor/AddFavorite")]
//        public IHttpActionResult AddFavorite(string UserId, int? ContractorId)
//        {

//            #region Validation
//            if (string.IsNullOrEmpty(UserId))
//            {
//                return Ok(new { Status = 0, Text = " لطفا شناسه کاربری را وارد نمایید . " });
//            }
//            ApplicationUser User = db.Users.Find(UserId);
//            if (User == null)
//                return Ok(new { Status = 0, Text = " کاربری با این شماره یافت نشد . " });
//            if (ContractorId == null)
//                return Ok(new { Status = 0, Text = " لطفا شناسه محصول را وارد نمایید . " });
//            Product prod = db.Products.Where(current => current.Id == ContractorId).Include(current => current.category).FirstOrDefault();
//            if (prod == null)
//            {
//                return Ok(new { Status = 0, Text = " محصولی با این شماره یافت نشد . " });
//            }
//            #endregion Validation

//            #region Favorite
//            Favorite favorite = db.Favorites.Where(current => current.UserId == UserId && current.ProductId == ContractorId).FirstOrDefault();
//            string IsFavorite = string.Empty;
//            if (favorite != null)
//            {
//                return Ok(new { Status = 0, Text = " شما قبلا این محصول را به لیست مورد علاقه های خود اضافه کرده بودید . " });
//            }
//            #endregion Favorite

//            Favorite fav = new Favorite();
//            fav.ProductId = Convert.ToInt64(ContractorId);
//            fav.UserId = UserId;
//            fav.Date = DateTime.Now;
//            db.Favorites.Add(fav);
//            try
//            {
//                db.SaveChanges();
//                return Ok(new { Status = 1, Text = " محصول مورد نظر به لیست منتخب های شما اضافه شد . " });
//            }
//            catch (Exception)
//            {
//                return Ok(new { Status = 0, Text = " خطایی رخ داده است لطفا مجدد تلاش بفرمایید . " });
//            }

//        }

//        [AcceptVerbs("GET")]
//        [Route("Contractor/DeleteFavorite")]
//        public IHttpActionResult DeleteFavorite(string UserId, int? ContractorId)
//        {

//            #region Validation
//            if (string.IsNullOrEmpty(UserId))
//            {
//                return Ok(new { Status = 0, Text = " لطفا شناسه کاربری را وارد نمایید . " });
//            }
//            ApplicationUser User = db.Users.Find(UserId);
//            if (User == null)
//                return Ok(new { Status = 0, Text = " کاربری با این شماره یافت نشد . " });
//            if (ContractorId == null)
//                return Ok(new { Status = 0, Text = " لطفا شناسه محصول را وارد نمایید . " });
//            Product prod = db.Products.Where(current => current.Id == ContractorId).Include(current => current.category).FirstOrDefault();
//            if (prod == null)
//            {
//                return Ok(new { Status = 0, Text = " سرویس دهنده ای با این شماره یافت نشد . " });
//            }
//            #endregion Validation
//            Favorite fav = db.Favorites.Where(current => current.UserId == UserId && current.ProductId == ContractorId).FirstOrDefault();
//            if (fav == null)
//            {
//                return Ok(new { Status = 0, Text = " این محصول در لیست علاقه مندی های شما وجود ندارد . " });
//            }
//            db.Favorites.Remove(fav);
//            try
//            {
//                db.SaveChanges();
//                return Ok(new { Status = 1, Text = "محصول مورد نظر از لیست علاقه مندی های شما حذف گردید ." });
//            }
//            catch (Exception)
//            {
//                return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//            }
//        }

//        [AcceptVerbs("GET")]
//        [Route("Products/GetFavorites")]
//        public IHttpActionResult GetFavorites(int? PageSize, int? PageIndex, int loopCount, string UserId, string Latitude, string Longitude, string GuId)
//        {

//            #region Validation
//            if (string.IsNullOrEmpty(UserId))
//            {
//                return Ok(new { Status = 0, Text = " لطفا شناسه کاربری را وارد نمایید . " });
//            }
//            ApplicationUser User = db.Users.Where(current => current.Id == UserId).Include(current => current.Favorites).FirstOrDefault();
//            if (User == null)
//                return Ok(new { Status = 0, Text = " کاربری با این شماره یافت نشد . " });
//            var favorites = db.Favorites.Where(current => current.UserId == UserId).Include(current => current.Product);
//            List<Product> adds = new List<Product>();
//            List<Product> orderedAdds = new List<Product>();
//            foreach (var item in favorites.OrderByDescending(current => current.Product.PersianName))
//            {
//                adds.Add(item.Product);
//            }
//            #endregion Validation

//            #region Query
//            var y = loopCount;
//            if (PageIndex == 1)
//            {
//                db.UserTransactions.Add(new UserTransactions() { userId = UserId.ToString(), visitDateTime = DateTime.Now });
//                try
//                {
//                    db.SaveChanges();
//                }
//                catch (Exception)
//                {
//                    return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید ." });
//                }
//            }
//            if (PageIndex >= 2)
//            {
//                var transaction = db.UserTransactions.Where(current => current.userId == UserId.ToString())
//                    .OrderByDescending(current => current.visitDateTime).FirstOrDefault();
//                adds = adds.Where(current => current.CreateDate <= transaction.visitDateTime).ToList();
//            }

//            adds = adds.Skip((y - 1) * (int)PageSize).Take((int)PageSize)
//                   .AsEnumerable().ToList();

//            #endregion Query

//            #region FindData
//            List<dynamic> AddsList = new List<dynamic>();
//            foreach (var pr in adds)
//            {
//                AddsList.Add(new
//                {
//                    Id = pr.Id.ToString(),
//                    PersianName = pr.PersianName,
//                    UnitPrice = pr.UnitPrice,
//                    PriceWithDiscount = pr.UnitPrice - (pr.UnitPrice) * (pr.DiscountPercent) / 100,
//                    Discount = pr.DiscountPercent,
//                    Image = BaseUrl + pr.AppSmallImage,
//                    Stock = pr.Stock
//                });
//            }
//            if (AddsList.Count() == 0)
//            {
//                return Ok(new { Status = 0, LoopsCount = y, pageIndex = PageIndex, pageSize = PageSize, Object = AddsList, AddCount = AddsList.Count(), Text = "متاسفانه محصولی یافت نشد .", BasketCount = BasketCount(UserId, GuId) });
//            }
//            else
//            {
//                return Ok(new { Status = 1, LoopsCount = y, pageIndex = PageIndex, pageSize = PageSize, Object = AddsList, AddCount = AddsList.Count(), Text = "اطلاعات با موفقیت دریافت شدند .", BasketCount = BasketCount(UserId, GuId) });
//            }
//            #endregion FindData
//        }

//        [AcceptVerbs("POST")]
//        [Route("Products/List")]
//        public IHttpActionResult ProductsList(string FromPrice, string ToPrice, int? CategoryId, string UserId, string GuId, HttpRequestMessage requset)
//        {

//            #region Request
//            var body = requset.Content;
//            var jsonContent = body.ReadAsStringAsync().Result;
//            SearchViewModel requestParams = JsonConvert.DeserializeObject<SearchViewModel>(jsonContent);
//            #endregion Request

//            IQueryable<Product> finalProducts = db.Products
//           .Include(current => current.category)
//           .OrderByDescending(current => current.CreateDate).Where(currrent => currrent.SiteFirstImage != null);
//            if (CategoryId != null)
//            {
//                finalProducts = finalProducts.Where(current => current.CategoryId == CategoryId);
//            }
//            if (!string.IsNullOrEmpty(requestParams.entryText))
//            {
//                finalProducts = finalProducts.Where(current => current.PersianName.Contains(requestParams.entryText) || current.Description.Contains(requestParams.entryText) || current.EnglishName.Contains(requestParams.entryText));
//            }
//            if (!string.IsNullOrEmpty(FromPrice))
//            {
//                int fromPrice = Convert.ToInt32(FromPrice);
//                finalProducts = finalProducts.Where(current => current.UnitPrice >= fromPrice);
//            }
//            if (!string.IsNullOrEmpty(ToPrice))
//            {
//                int toPrice = Convert.ToInt32(ToPrice);
//                finalProducts = finalProducts.Where(current => current.UnitPrice <= toPrice);
//            }

//            #region FindProducts
//            List<dynamic> ProductList = new List<dynamic>();
//            foreach (var pr in finalProducts.ToList())
//            {
//                ProductList.Add(new
//                {
//                    Id = pr.Id.ToString(),
//                    PersianName = pr.PersianName,
//                    UnitPrice = pr.UnitPrice,
//                    PriceWithDiscount = pr.UnitPrice - (pr.UnitPrice) * (pr.DiscountPercent) / 100,
//                    Discount = pr.DiscountPercent,
//                    Image = BaseUrl + pr.AppSmallImage,
//                    Stock = pr.Stock
//                });
//            }

//            List<dynamic> SubList = new List<dynamic>();
//            if (CategoryId != null)
//            {
//                Category cat = db.Categories.Where(current => current.Id == CategoryId).Include(current => current.SubCategories).FirstOrDefault();
//                if (cat != null)
//                {
//                    foreach (var ct in cat.SubCategories.ToList())
//                    {
//                        SubList.Add(new
//                        {
//                            Id = ct.Id,
//                            Title = ct.PersianName,
//                            Image = BaseUrl + ct.SmallImage
//                        });
//                    }
//                    return Ok(new { Status = 1, SubCount = cat.SubCategories.Count(), SubCategories = SubList, Object = ProductList, ProductCount = ProductList.Count, BasketCount = BasketCount(UserId, GuId) });
//                }
//                else
//                {
//                    return Ok(new { Status = 1, SubCount = 0, SubCategories = SubList, Object = ProductList, ProductCount = ProductList.Count, BasketCount = BasketCount(UserId, GuId) });
//                }
//            }
//            return Ok(new { Status = 1, SubCount = 0, SubCategories = SubList, Object = ProductList, ProductCount = ProductList.Count, BasketCount = BasketCount(UserId, GuId) });
//            #endregion FindProducts

//        }

//        [AcceptVerbs("GET")]
//        [Route("VIP/Main")]
//        public IHttpActionResult Main(string UserId, string GuId, string PlayerId)
//        {

//            if (!string.IsNullOrEmpty(UserId))
//            {
//                ApplicationUser User = db.Users.Find(UserId);
//                if (User != null)
//                {
//                    User.PlayerId = PlayerId;
//                    db.Entry(User).State = EntityState.Modified;
//                    try
//                    {
//                        db.SaveChanges();
//                    }
//                    catch (Exception)
//                    {
//                        return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                    }
//                }
//            }

//            IQueryable<Product> newestProducts = db.Products
//           .Include(current => current.category)
//           .OrderByDescending(current => current.CreateDate).Where(currrent => currrent.SiteFirstImage != null);

//            IQueryable<Product> specialProducts = db.Products
//           .Include(current => current.category)
//           .OrderByDescending(current => current.CreateDate).Where(currrent => currrent.SiteFirstImage != null && currrent.IsSpecial == true);

//            #region FindProducts
//            List<dynamic> NewProductList = new List<dynamic>();
//            List<dynamic> SpecialProductList = new List<dynamic>();
//            List<dynamic> Sliders = new List<dynamic>();
//            foreach (var pr in newestProducts.Take(10).ToList())
//            {
//                NewProductList.Add(new
//                {
//                    Id = pr.Id.ToString(),
//                    PersianName = pr.PersianName,
//                    UnitPrice = pr.UnitPrice,
//                    PriceWithDiscount = pr.UnitPrice - (pr.UnitPrice) * (pr.DiscountPercent) / 100,
//                    Discount = pr.DiscountPercent,
//                    Image = BaseUrl + pr.AppSmallImage,
//                    Stock = pr.Stock
//                });
//            }
//            foreach (var pr2 in specialProducts.Take(10).ToList())
//            {
//                SpecialProductList.Add(new
//                {
//                    Id = pr2.Id.ToString(),
//                    PersianName = pr2.PersianName,
//                    UnitPrice = pr2.UnitPrice,
//                    PriceWithDiscount = pr2.UnitPrice - (pr2.UnitPrice) * (pr2.DiscountPercent) / 100,
//                    Discount = pr2.DiscountPercent,
//                    Image = BaseUrl + pr2.AppSmallImage,
//                    Stock = pr2.Stock
//                });
//            }
//            Slider slider = db.Sliders.Include(current => current.Images).FirstOrDefault();
//            if (slider != null && slider.Images.Count > 0)
//            {
//                foreach (var item in slider.Images.Take(6).ToList())
//                {
//                    Sliders.Add(new
//                    {
//                        Id = item.Id.ToString(),
//                        Image = BaseUrl + item.Link
//                    });
//                }
//            }
//            else
//            {
//                Sliders.Add(new
//                {
//                    Id = "",
//                    Image = BaseUrl + "images/pransa_logo_web.png"
//                });
//            }
//            return Ok(new { Status = 1, NewestProducts = NewProductList, NewestProductsCount = NewProductList.Count, SpecialProducts = SpecialProductList, SpecialProductsCount = SpecialProductList.Count, Sliders = Sliders, SlidersCount = Sliders.Count, BasketCount = BasketCount(UserId, GuId) });
//            #endregion FindProducts
//        }

//        [AcceptVerbs("GET")]
//        [Route("Products/Details")]
//        public IHttpActionResult ProductDetails(long? ProductId, string UserId, string GuId)
//        {

//            #region Validation
//            if (ProductId == null)
//                return Ok(new { Status = 0, Text = "لطفا شناسه محصول را وارد نمایید ." });
//            Product product = db.Products.Where(current => current.Id == ProductId).Include(current => current.category).Include(current => current.RelatedProducts).FirstOrDefault();
//            if (product == null)
//                return Ok(" محصولی با این شماره یافت نشد . ");
//            #endregion Validation

//            #region FindProduct
//            IHtmlString htmlString = new HtmlString(product.Description);
//            string htmlResult = Regex.Replace(htmlString.ToString(), @"<[^>]*>", string.Empty).Replace("&nbsp;", " ").Replace("&zwnjd", " ").Replace("zwnjd&", " ").Replace("&zwnjd;", " ").Replace("&idquo", " ").Replace("ldquo&", " ").Replace("&ldquo;", " ").Replace(";", "").Replace("&", "").Replace("zwnj", " ").Replace("idquo", " ");
//            List<dynamic> ImagesList = new List<dynamic>();
//            string EditImage = string.Empty;
//            if (product.AppLargeImage != null)
//            {
//                EditImage += product.AppLargeImage + ",";
//                ImagesList.Add(new
//                {
//                    Id = product.Id,
//                    Path = product.AppLargeImage.ToString() ?? "NULL"
//                });
//            }
//            if (product.AppSmallImage != null)
//            {
//                EditImage += product.AppSmallImage + ",";
//                ImagesList.Add(new
//                {
//                    Id = product.Id,
//                    Path = product.AppSmallImage.ToString() ?? "NULL"
//                });
//            }
//            else
//            {
//                ImagesList.Add(new
//                {
//                    Id = "NULL",
//                    Path = "/images/pransa_logo_web.png",
//                });
//            }

//            List<dynamic> RelatedList = new List<dynamic>();
//            foreach (var pr in product.RelatedProducts.ToList())
//            {
//                RelatedList.Add(new
//                {
//                    Id = pr.Id.ToString(),
//                    PersianName = pr.PersianName,
//                    UnitPrice = pr.UnitPrice,
//                    PriceWithDiscount = pr.UnitPrice - (pr.UnitPrice) * (pr.DiscountPercent) / 100,
//                    Discount = pr.DiscountPercent,
//                    Image = BaseUrl + pr.AppSmallImage,
//                    Stock = pr.Stock
//                });
//            }
//            return Ok(new
//            {
//                Status = "1",
//                Text = "اطلاعات با موفقیت دریافت شدند .",
//                PersianName = product.PersianName,
//                EnglishName = product.EnglishName,
//                UnitPrice = product.UnitPrice,
//                PriceWithDiscount = product.UnitPrice - (product.UnitPrice) * (product.DiscountPercent) / 100,
//                Stock = product.Stock,
//                DiscountPercent = product.DiscountPercent,
//                Description = htmlResult,
//                Category = product.category.PersianName,
//                CreateDate = FunctionsHelper.GetPersianDateTime(product.CreateDate, false, false),
//                UserImages = ImagesList,
//                EditImages = EditImage != "" ? EditImage.Remove(EditImage.Length - 1) : "",
//                BasketCount = BasketCount(UserId, GuId),
//                RelatedCount = product.RelatedProducts.Count(),
//                RelatedProduct = RelatedList
//            });
//            #endregion FindProduct
//        }

//        public static bool SendSms(string SmsServiceNumber, string PhoneNumber, string Description)
//        {
//            if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(SmsServiceNumber))
//                return false;
//            SmsIrRestful.Token tokenInstance = new SmsIrRestful.Token();
//            var token = tokenInstance.GetToken("9b2a827d86a88a4dac1670f2", "it66)%#teBC!@*&");
//            SmsIrRestful.MessageSend messageInstace = new SmsIrRestful.MessageSend();
//            var res = messageInstace.Send(token, new SmsIrRestful.MessageSendObject()
//            {
//                MobileNumbers = new List<string>() { PhoneNumber }.ToArray(),
//                Messages = new List<string>() { Description }.ToArray(),
//                LineNumber = SmsServiceNumber,
//                SendDateTime = DateTime.Now,
//                CanContinueInCaseOfError = false
//            });
//            if (res.IsSuccessful == true)
//                return true;
//            else
//                return false;
//        }
//        public static bool FastSms(string PhoneNumber, string Code)
//        {
//            if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Code))
//                return false;
//            Token tokenInstance = new Token();
//            var token = new Token().GetToken("e53a9b26a9aca9d5c6ae96e7", "it66)%18#teBC!@*&");
//            var restVerificationCode = new RestVerificationCode()
//            {
//                Code = Code,
//                MobileNumber = PhoneNumber
//            };
//            var restVerificationCodeRespone = new VerificationCode().Send(token, restVerificationCode);
//            if (restVerificationCodeRespone.IsSuccessful)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
//        public static bool UltraFastSms(string PhoneNumber, string Code)
//        {
//            var token = new Token().GetToken("e53a9b26a9aca9d5c6ae96e7", "it66)%18#teBC!@*&");

//            var ultraFastSend = new UltraFastSend()
//            {
//                Mobile = long.Parse(PhoneNumber),
//                TemplateId = 16673,
//                ParameterArray = new List<UltraFastParameters>()
//        {
//        new UltraFastParameters()
//        {
//            Parameter = "VerificationCode",
//            ParameterValue = Code
//        }
//        }.ToArray()

//            };

//            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

//            if (ultraFastSendRespone.IsSuccessful)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
//        [AcceptVerbs("GET")]
//        [Route("Server/Connection")]
//        public IHttpActionResult Connection(string UserId, string GuId)
//        {
//            Guid g;
//            string guid = string.Empty;
//            if (string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(GuId))
//            {
//                g = Guid.NewGuid();
//                guid = g.ToString();
//            }
//            return Ok(new { status = true, GuId = guid });
//        }

//        [AcceptVerbs("GET")]
//        [Route("Order/MyActivities")]
//        public IHttpActionResult GetActivity(string UserId, string GuId)
//        {
//            #region Validation
//            if (string.IsNullOrEmpty(UserId))
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            var Orders = db.Orders.Where(current => current.UserId == UserId).OrderByDescending(current => current.OrderDate).Take(10).ToList();
//            if (Orders.Count == 0)
//                return Ok(new { Status = 0, Text = "هنوز سفارشی برای این کاربر وجود ندارد .", OrdersCount = 0 });
//            #endregion Validation

//            #region FindActivities
//            List<dynamic> OrdersList = new List<dynamic>();
//            foreach (var item in Orders)
//            {
//                OrdersList.Add(new
//                {
//                    Id = item.Id,
//                    DateTime = FunctionsHelper.GetPersianDateTime(item.OrderDate, true, true),
//                    Done = item.Done.ToString(),
//                    Price = item.TotalPrice ?? "NULL",
//                    Cancel = item.IsCanceled.ToString(),
//                    ImagePath = BaseUrl + "/images/pransa_logo_web.png",
//                    Type = item.getType
//                });
//            }
//            return Ok(new
//            {
//                Status = 1,
//                DoneOrders = Orders.Where(curren => curren.Done == true).Count(),
//                CancelOrders = Orders.Where(current => current.IsCanceled == true).Count(),
//                AllOrders = Orders.Count(),
//                Object = OrdersList,
//                OrdersCount = OrdersList.Count,
//                BasketCount = BasketCount(UserId, GuId)
//            });
//            #endregion FindActivities
//        }

//        [AcceptVerbs("GET")]
//        [Route("Order/Specific")]
//        public IHttpActionResult SpecialOrder(int? OrderId, string UserId, string GuId)
//        {

//            #region Validation
//            if (OrderId == null)
//                return Ok(new { Status = 0, Text = "لطفا شناسه سفارش را وارد نمایید ." });
//            var Orders = db.Orders.Where(current => current.Id == OrderId).Include(current => current.User).Include(current => current.Address).Include(current => current.Transactions).FirstOrDefault();
//            if (Orders == null)
//                return Ok(new { Status = 0, Text = "سفارش مورد نظر پیدا نشد .", OrdersCount = 0 });
//            #endregion Validation

//            List<dynamic> OrdersList = new List<dynamic>();

//            List<dynamic> ImagesList = new List<dynamic>();

//            List<dynamic> Categories = new List<dynamic>();

//            string EditImage = string.Empty;

//            #region Categories
//            var cates = db.ProductInOrders.Where(current => current.OrderId == OrderId).Include(current => current.Product).ToList();
//            foreach (var item in cates)
//            {
//                Categories.Add(new
//                {
//                    PersianName = item.Product.PersianName,
//                    Count = item.Count
//                });
//            }
//            #endregion Categories

//            #region FindUserImages
//            var x = db.ProductInOrders.Where(current => current.OrderId == OrderId).Include(current => current.Product);
//            if (x.Count() > 0)
//            {
//                foreach (var img in x)
//                {
//                    if (img.Product.AppLargeImage != null)
//                    {
//                        EditImage += img.Product.AppLargeImage + ",";
//                        ImagesList.Add(new
//                        {
//                            Id = img.ProductId,
//                            Path = img.Product.AppLargeImage ?? ""
//                        });
//                    }
//                    else
//                    {
//                        EditImage += img.Product.AppLargeImage + ",";
//                        ImagesList.Add(new
//                        {
//                            Id = img.ProductId,
//                            Path = img.Product.AppLargeImage ?? ""
//                        });
//                    }
//                }
//            }
//            else
//            {
//                EditImage += BaseUrl + "/images/pransa_logo_web.png";
//                ImagesList.Add(new
//                {
//                    Id = "",
//                    Path = BaseUrl + "/images/pransa_logo_web.png"
//                });
//            }
//            #endregion FindUserImages

//            string IsPayed = string.Empty;
//            if (Orders.Transactions.Count() > 0)
//            {
//                IsPayed = "True";
//            }
//            else
//            {
//                IsPayed = "False";
//            }
//            OrdersList.Add(new
//            {
//                Id = Orders.Id,
//                DateTime = FunctionsHelper.GetPersianDateTime(Orders.OrderDate, true, true),
//                Category = Categories,
//                Done = Orders.Done.ToString(),
//                ReceivertName = Orders.getReceiver,
//                Type = Orders.getType,
//                PaymentType = Orders.getPaymentType,
//                InvoiceNumber = Orders.InvoiceNumber ?? "",
//                Price = Orders.TotalPrice ?? "",
//                UserDescription = Orders.UserOrderDescription,
//                FullName = Orders.FullName ?? "",
//                Phone = Orders.Phone ?? "",
//                UserAddress = Orders.UserAddress ?? "",
//                FactorNumber = Orders.FactorNumber,
//                Cancel = Orders.IsCanceled.ToString(),
//                UserImages = ImagesList,
//                EditImages = EditImage != "" ? EditImage.Remove(EditImage.Length - 1) : "",
//                UserId = Orders.UserId.ToString() ?? "",
//                IsPayed = IsPayed,
//                BasketCount = BasketCount(UserId, GuId)
//            });

//            #region FindActivities

//            return Ok(new { Status = 1, Object = OrdersList, OrdersCount = OrdersList.Count });
//            #endregion FindActivities
//        }

//        [AcceptVerbs("GET")]
//        [Route("Order/Basket")]
//        public IHttpActionResult GetBasket(string UserId, string GuId)
//        {
//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId) && string.IsNullOrWhiteSpace(GuId))
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            DateTime now = DateTime.Now;
//            Basket userBasket = db.Baskets.Where(current => current.UserId == UserId && current.UserId != null && current.UserId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            var uBasket = new List<ProductInBasket>();
//            if (userBasket != null && now < userBasket.CreateDate.AddHours(12))
//            {
//                uBasket = db.ProductInBaskets.Where(p => p.BasketId == userBasket.Id).Include(current => current.Product).ToList();
//            }
//            Basket guestBasket = db.Baskets.Where(current => current.GuId == GuId && current.GuId != null && current.GuId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            var gBasket = new List<ProductInBasket>();
//            if (guestBasket != null && now < guestBasket.CreateDate.AddHours(12))
//            {
//                gBasket = db.ProductInBaskets.Where(p => p.BasketId == guestBasket.Id).Include(current => current.Product).ToList();
//            }
//            #endregion Validation

//            #region FindBasket
//            List<dynamic> BasketList = new List<dynamic>();
//            if (uBasket != null)
//            {
//                foreach (var item in uBasket)
//                {
//                    BasketList.Add(new
//                    {
//                        BasketId = userBasket.Id,
//                        Image = BaseUrl + item.Product.AppSmallImage,
//                        PersianName = item.Product.PersianName,
//                        ProductId = item.ProductId,
//                        Count = item.Count
//                    });
//                }
//            }

//            if (gBasket != null)
//            {
//                foreach (var item in gBasket)
//                {
//                    BasketList.Add(new
//                    {
//                        BasketId = guestBasket.Id,
//                        Image = BaseUrl + item.Product.AppSmallImage,
//                        PersianName = item.Product.PersianName,
//                        ProductId = item.ProductId,
//                        Count = item.Count
//                    });
//                }
//            }

//            if (BasketList.Count == 0)
//            {
//                return Ok(new { Status = 1, Object = BasketList.DistinctBy(current => current.ProductId), BasketCount = BasketList.Count, BasketPrice = BasketPrice(UserId, GuId), Text = "محصولی در سبد خرید شما وجود ندارد ." });
//            }
//            else
//            {
//                return Ok(new { Status = 1, Object = BasketList.DistinctBy(current => current.ProductId), BasketCount = BasketList.Count, BasketPrice = BasketPrice(UserId, GuId), Text = "اطلاعات با موفقیت دریافت شدند ." });
//            }
//            #endregion FindBasket
//        }

//        [AcceptVerbs("GET")]
//        [Route("Order/AddToCard")]
//        public IHttpActionResult AddToCard(long? ProductId, string UserId, string GuId)
//        {

//            #region Validation
//            if (string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(GuId))
//            {
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            }
//            if (ProductId == null)
//            {
//                return Ok(new { Status = 0, Text = "لطفا شناسه محصول را وارد نمایید ." });
//            }
//            Product pro = db.Products.Find(ProductId);
//            if (pro == null)
//            {
//                return Ok(new { Status = 0, Text = "محصول مورد نظر یافت نشد ." });
//            }
//            #endregion Validation

//            DateTime now = DateTime.Now;
//            Basket userBasket = db.Baskets.Where(current => current.UserId == UserId && current.UserId != null).Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            Basket guestBasket = db.Baskets.Where(current => current.GuId == GuId && current.GuId != null).Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();

//            #region UserBasket
//            if (!string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(GuId))
//            {
//                //سبد خرید در 12 ساعت اخیر به عنوان کاربر داشت
//                if (userBasket != null && now < userBasket.CreateDate.AddHours(12))
//                {
//                    ProductInBasket userIn = userBasket.ProductInBaskets.Where(current => current.ProductId == ProductId).FirstOrDefault();
//                    //محصول قبلا اضافه شده بود
//                    if (userIn != null)
//                    {
//                        userIn.Count++;
//                        db.Entry(userIn).State = EntityState.Modified;
//                        try
//                        {
//                            db.SaveChanges();
//                            return Ok(new { Status = 1, Text = "محصول مورد نظر به سبد خرید شما اضافه شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                        catch (Exception)
//                        {
//                            return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                        }
//                    }
//                    //محصول در سبد نبود
//                    else
//                    {
//                        ProductInBasket prBasket = new ProductInBasket()
//                        {
//                            BasketId = userBasket.Id,
//                            Count = 1,
//                            ProductId = (long)ProductId
//                        };
//                        db.ProductInBaskets.Add(prBasket);
//                        try
//                        {
//                            db.SaveChanges();
//                            return Ok(new { Status = 1, Text = "محصول مورد نظر به سبد خرید شما اضافه شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                        catch (Exception)
//                        {
//                            return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                        }
//                    }
//                }
//                //سبد خرید در 12 ساعت اخیر به عنوان کاربر نداشت
//                else
//                {
//                    Basket bsk = new Basket()
//                    {
//                        UserId = UserId,
//                        GuId = string.Empty,
//                        CreateDate = DateTime.Now
//                    };
//                    db.Baskets.Add(bsk);
//                    try
//                    {
//                        db.SaveChanges();
//                        ProductInBasket prBasket = new ProductInBasket()
//                        {
//                            BasketId = bsk.Id,
//                            Count = 1,
//                            ProductId = (long)ProductId
//                        };
//                        db.ProductInBaskets.Add(prBasket);
//                        try
//                        {
//                            db.SaveChanges();
//                            return Ok(new { Status = 1, Text = "محصول مورد نظر به سبد خرید شما اضافه شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                        catch (Exception)
//                        {
//                            return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                        }
//                    }
//                    catch (Exception)
//                    {
//                        return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                    }
//                }
//            }

//            #endregion UserBasket

//            #region GuestBasket
//            if (string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(GuId))
//            {
//                //سبد خرید در 12 ساعت اخیر به عنوان مهمان داشت
//                if (guestBasket != null && now < guestBasket.CreateDate.AddHours(12))
//                {
//                    ProductInBasket guestIn = guestBasket.ProductInBaskets.Where(current => current.ProductId == ProductId).FirstOrDefault();
//                    //محصول قبلا اضافه شده بود
//                    if (guestIn != null)
//                    {
//                        guestIn.Count++;
//                        db.Entry(guestIn).State = EntityState.Modified;
//                        try
//                        {
//                            db.SaveChanges();
//                            return Ok(new { Status = 1, Text = "محصول مورد نظر به سبد خرید شما اضافه شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                        catch (Exception)
//                        {
//                            return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                        }
//                    }
//                    //محصول در سبد نبود
//                    else
//                    {
//                        ProductInBasket prBasket = new ProductInBasket()
//                        {
//                            BasketId = guestBasket.Id,
//                            Count = 1,
//                            ProductId = (long)ProductId
//                        };
//                        db.ProductInBaskets.Add(prBasket);
//                        try
//                        {
//                            db.SaveChanges();
//                            return Ok(new { Status = 1, Text = "محصول مورد نظر به سبد خرید شما اضافه شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                        catch (Exception)
//                        {
//                            return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                        }
//                    }
//                }
//                //سبد خرید در 12 ساعت اخیر به عنوان مهمان نداشت
//                else
//                {
//                    Basket bsk = new Basket()
//                    {
//                        GuId = GuId,
//                        UserId = string.Empty,
//                        CreateDate = DateTime.Now
//                    };
//                    db.Baskets.Add(bsk);
//                    try
//                    {
//                        db.SaveChanges();
//                        ProductInBasket prBasket = new ProductInBasket()
//                        {
//                            BasketId = bsk.Id,
//                            Count = 1,
//                            ProductId = (long)ProductId
//                        };
//                        db.ProductInBaskets.Add(prBasket);
//                        try
//                        {
//                            db.SaveChanges();
//                            return Ok(new { Status = 1, Text = "محصول مورد نظر به سبد خرید شما اضافه شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                        catch (Exception)
//                        {
//                            return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                        }
//                    }
//                    catch (Exception)
//                    {
//                        return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                    }
//                }
//            }

//            #endregion GuestBasket

//            return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//        }

//        [AcceptVerbs("GET")]
//        [Route("Order/UpdateCard")]
//        public IHttpActionResult UpdateCard(long? ProductId, bool Increase, string UserId, string GuId)
//        {

//            #region Validation
//            if (string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(GuId))
//            {
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            }
//            if (ProductId == null)
//            {
//                return Ok(new { Status = 0, Text = "لطفا شناسه محصول را وارد نمایید ." });
//            }
//            Product pro = db.Products.Find(ProductId);
//            if (pro == null)
//            {
//                return Ok(new { Status = 0, Text = "محصول مورد نظر یافت نشد ." });
//            }
//            #endregion Validation

//            DateTime now = DateTime.Now;
//            Basket userBasket = db.Baskets.Where(current => current.UserId == UserId && current.UserId != null).Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            Basket guestBasket = db.Baskets.Where(current => current.GuId == GuId && current.GuId != null).Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();

//            #region UserBasket
//            if (Increase == true)
//            {
//                if (!string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(GuId))
//                {
//                    //سبد خرید در 12 ساعت اخیر به عنوان کاربر داشت
//                    if (userBasket != null && now < userBasket.CreateDate.AddHours(12))
//                    {
//                        ProductInBasket userIn = userBasket.ProductInBaskets.Where(current => current.ProductId == ProductId).FirstOrDefault();
//                        //محصول قبلا اضافه شده بود
//                        if (userIn != null)
//                        {
//                            userIn.Count++;
//                            db.Entry(userIn).State = EntityState.Modified;
//                            try
//                            {
//                                db.SaveChanges();
//                                return Ok(new { Status = 1, Text = "محصول مورد نظر به سبد خرید شما اضافه شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                            }
//                            catch (Exception)
//                            {
//                                return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                            }
//                        }
//                        //محصول در سبد نبود
//                        else
//                        {
//                            return Ok(new { Status = 0, Text = "محصول مورد نظر در سبد خرید شما موجود نیست .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                    }
//                }
//            }

//            else
//            {
//                if (!string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(GuId))
//                {
//                    //سبد خرید در 12 ساعت اخیر به عنوان کاربر داشت
//                    if (userBasket != null && now < userBasket.CreateDate.AddHours(12))
//                    {
//                        ProductInBasket userIn = userBasket.ProductInBaskets.Where(current => current.ProductId == ProductId).FirstOrDefault();
//                        //محصول قبلا اضافه شده بود
//                        if (userIn != null && userIn.Count > 1)
//                        {
//                            userIn.Count--;
//                            db.Entry(userIn).State = EntityState.Modified;
//                            try
//                            {
//                                db.SaveChanges();
//                                return Ok(new { Status = 1, Text = "محصول مورد نظر از سبد خرید شما کم شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                            }
//                            catch (Exception)
//                            {
//                                return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                            }
//                        }
//                        //محصول در سبد نبود
//                        else
//                        {
//                            return Ok(new { Status = 0, Text = "محصول مورد نظر در سبد خرید شما موجود نیست .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                    }
//                }
//            }

//            #endregion UserBasket

//            #region GuestBasket
//            if (Increase == true)
//            {
//                if (string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(GuId))
//                {
//                    //سبد خرید در 12 ساعت اخیر به عنوان مهمان داشت
//                    if (guestBasket != null && now < guestBasket.CreateDate.AddHours(12))
//                    {
//                        ProductInBasket guestIn = guestBasket.ProductInBaskets.Where(current => current.ProductId == ProductId).FirstOrDefault();
//                        //محصول قبلا اضافه شده بود
//                        if (guestIn != null)
//                        {
//                            guestIn.Count++;
//                            db.Entry(guestIn).State = EntityState.Modified;
//                            try
//                            {
//                                db.SaveChanges();
//                                return Ok(new { Status = 1, Text = "محصول مورد نظر به سبد خرید شما اضافه شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId), });
//                            }
//                            catch (Exception)
//                            {
//                                return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                            }
//                        }
//                        //محصول در سبد نبود
//                        else
//                        {
//                            return Ok(new { Status = 0, Text = "محصول مورد نظر در سبد خرید شما موجود نیست .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                    }
//                }
//            }
//            else
//            {
//                if (string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(GuId))
//                {
//                    //سبد خرید در 12 ساعت اخیر به عنوان مهمان داشت
//                    if (guestBasket != null && now < guestBasket.CreateDate.AddHours(12))
//                    {
//                        ProductInBasket guestIn = guestBasket.ProductInBaskets.Where(current => current.ProductId == ProductId).FirstOrDefault();
//                        //محصول قبلا اضافه شده بود
//                        if (guestIn != null && guestIn.Count > 1)
//                        {
//                            guestIn.Count--;
//                            db.Entry(guestIn).State = EntityState.Modified;
//                            try
//                            {
//                                db.SaveChanges();
//                                return Ok(new { Status = 1, Text = "محصول مورد نظر از سبد خرید شما کم شد .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId), });
//                            }
//                            catch (Exception)
//                            {
//                                return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//                            }
//                        }
//                        //محصول در سبد نبود
//                        else
//                        {
//                            return Ok(new { Status = 0, Text = "محصول مورد نظر در سبد خرید شما موجود نیست .", BasketCount = BasketCount(UserId, GuId), BasketPrice = BasketPrice(UserId, GuId) });
//                        }
//                    }
//                }
//            }


//            #endregion GuestBasket

//            return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش بفرمایید ." });
//        }

//        [AcceptVerbs("GET")]
//        [Route("Order/DeleteBasket")]
//        public IHttpActionResult DeleteBasket(long? ProductId, int? BasketId, string UserId, string GuId)
//        {

//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId) && string.IsNullOrWhiteSpace(GuId))
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            if (BasketId == null)
//            {
//                return Ok(new { Status = 0, Text = "لطفا شناسه سبد خرید را وارد نمایید ." });
//            }
//            Basket bsk = db.Baskets.Where(current => current.Id == BasketId).Include(current => current.ProductInBaskets).FirstOrDefault();
//            if (bsk == null)
//            {
//                return Ok(new { Status = 0, Text = "سبد مورد نظر یافت نشد ." });
//            }
//            if (ProductId == null)
//            {
//                return Ok(new { Status = 0, Text = "لطفا شناسه محصول را وارد نمایید ." });
//            }
//            Product pro = db.Products.Find(ProductId);
//            if (pro == null)
//            {
//                return Ok(new { Status = 0, Text = "محصول مورد نظر یافت نشد ." });
//            }
//            #endregion Validation

//            ProductInBasket x = bsk.ProductInBaskets.Where(current => current.ProductId == ProductId).FirstOrDefault();

//            #region Delete
//            if (x == null)
//            {
//                return Ok(new { Status = 0, Text = "محصول انتخابی در سبد خرید شما وجود ندارد ." });
//            }
//            foreach (var item in bsk.ProductInBaskets.ToList())
//            {
//                if (item.ProductId == ProductId)
//                {
//                    db.ProductInBaskets.Remove(item);
//                    try
//                    {
//                        db.SaveChanges();
//                    }
//                    catch (Exception)
//                    {
//                        return Ok(new { Status = 0, Text = "امکان حذف سبد برای شما در حال حاضر وجود ندارد ." });
//                    }
//                }
//            }
//            if (bsk.ProductInBaskets.Count() == 0)
//            {
//                db.Baskets.Remove(bsk);
//                try
//                {
//                    db.SaveChanges();
//                }
//                catch (Exception)
//                {
//                    return Ok(new { Status = 0, Text = "امکان حذف سبد برای شما در حال حاضر وجود ندارد ." });
//                }
//            }
//            #endregion Delete

//            return Ok(new { Status = 1, Text = "سبد خرید شما حذف شد .", BasketPrice = BasketPrice(UserId, GuId), BasketCount = BasketCount(UserId, GuId) });
//        }

//        public int BasketCount(string UserId, string GuId)
//        {
//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId) && string.IsNullOrWhiteSpace(GuId))
//                return (0);
//            DateTime now = DateTime.Now;
//            Basket userBasket = db.Baskets.Where(current => current.UserId == UserId && current.UserId != null && current.UserId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            var uBasket = new List<ProductInBasket>();
//            if (userBasket != null && now < userBasket.CreateDate.AddHours(12))
//            {
//                uBasket = db.ProductInBaskets.Where(p => p.BasketId == userBasket.Id).Include(current => current.Product).ToList();
//            }
//            Basket guestBasket = db.Baskets.Where(current => current.GuId == GuId && current.GuId != null && current.GuId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            var gBasket = new List<ProductInBasket>();
//            if (guestBasket != null && now < guestBasket.CreateDate.AddHours(12))
//            {
//                gBasket = db.ProductInBaskets.Where(p => p.BasketId == guestBasket.Id).Include(current => current.Product).ToList();
//            }
//            #endregion Validation

//            #region FindBasket
//            List<dynamic> BasketList = new List<dynamic>();
//            if (uBasket != null)
//                foreach (var item in uBasket)
//                {
//                    BasketList.Add(new
//                    {
//                        BasketId = userBasket.Id,
//                        Image = BaseUrl + item.Product.AppSmallImage,
//                        PersianName = item.Product.PersianName,
//                        ProductId = item.ProductId,
//                        Count = item.Count
//                    });
//                }
//            if (gBasket != null)
//                foreach (var item in gBasket)
//                {
//                    BasketList.Add(new
//                    {
//                        BasketId = guestBasket.Id,
//                        Image = BaseUrl + item.Product.AppSmallImage,
//                        PersianName = item.Product.PersianName,
//                        ProductId = item.ProductId,
//                        Count = item.Count
//                    });
//                }
//            if (BasketList.Count == 0)
//            {
//                return (0);
//            }
//            else
//            {
//                return (BasketList.Count);
//            }
//            #endregion FindBasket
//        }

//        public int BasketPrice(string UserId, string GuId)
//        {
//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId) && string.IsNullOrWhiteSpace(GuId))
//                return (0);
//            DateTime now = DateTime.Now;
//            Basket userBasket = db.Baskets.Where(current => current.UserId == UserId && current.UserId != null && current.UserId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            var uBasket = new List<ProductInBasket>();
//            if (userBasket != null && now < userBasket.CreateDate.AddHours(12))
//            {
//                uBasket = db.ProductInBaskets.Where(p => p.BasketId == userBasket.Id).Include(current => current.Product).ToList();
//            }
//            Basket guestBasket = db.Baskets.Where(current => current.GuId == GuId && current.GuId != null && current.GuId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            var gBasket = new List<ProductInBasket>();
//            if (guestBasket != null && now < guestBasket.CreateDate.AddHours(12))
//            {
//                gBasket = db.ProductInBaskets.Where(p => p.BasketId == guestBasket.Id).Include(current => current.Product).ToList();
//            }
//            #endregion Validation

//            #region FindBasket
//            List<dynamic> BasketList = new List<dynamic>();
//            if (uBasket != null)
//                foreach (var item in uBasket)
//                {
//                    BasketList.Add(new
//                    {
//                        BasketId = userBasket.Id,
//                        Image = BaseUrl + item.Product.AppSmallImage,
//                        PersianName = item.Product.PersianName,
//                        ProductId = item.ProductId,
//                        Count = item.Count
//                    });
//                }
//            if (gBasket != null)
//                foreach (var item in gBasket)
//                {
//                    BasketList.Add(new
//                    {
//                        BasketId = guestBasket.Id,
//                        Image = BaseUrl + item.Product.AppSmallImage,
//                        PersianName = item.Product.PersianName,
//                        ProductId = item.ProductId,
//                        Count = item.Count
//                    });
//                }
//            if (BasketList.Count == 0)
//            {
//                return (0);
//            }
//            else
//            {
//                int finalPrice = 0;
//                foreach (var price in BasketList)
//                {
//                    Product prod = db.Products.Find(price.ProductId);
//                    if (prod == null)
//                    {
//                        return (0);
//                    }
//                    finalPrice += (prod.UnitPrice - (prod.UnitPrice) * (prod.DiscountPercent) / 100) * price.Count;
//                }
//                return (finalPrice);
//            }
//            #endregion FindBasket
//        }

//        public List<ProductInCount> BasketItems(string UserId, string GuId)
//        {
//            #region Validation
//            if (string.IsNullOrWhiteSpace(UserId) && string.IsNullOrWhiteSpace(GuId))
//                return (new List<ProductInCount>());
//            DateTime now = DateTime.Now;
//            Basket userBasket = db.Baskets.Where(current => current.UserId == UserId && current.UserId != null && current.UserId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            var uBasket = new List<ProductInBasket>();
//            if (userBasket != null && now < userBasket.CreateDate.AddHours(12))
//            {
//                uBasket = db.ProductInBaskets.Where(p => p.BasketId == userBasket.Id).Include(current => current.Product).ToList();
//            }
//            Basket guestBasket = db.Baskets.Where(current => current.GuId == GuId && current.GuId != null && current.GuId != "").Include(current => current.ProductInBaskets).OrderByDescending(current => current.CreateDate).FirstOrDefault();
//            var gBasket = new List<ProductInBasket>();
//            if (guestBasket != null && now < guestBasket.CreateDate.AddHours(12))
//            {
//                gBasket = db.ProductInBaskets.Where(p => p.BasketId == guestBasket.Id).Include(current => current.Product).ToList();
//            }
//            #endregion Validation

//            #region FindBasket
//            List<dynamic> BasketList = new List<dynamic>();
//            if (uBasket != null)
//                foreach (var item in uBasket)
//                {
//                    BasketList.Add(new
//                    {
//                        BasketId = userBasket.Id,
//                        Image = BaseUrl + item.Product.AppSmallImage,
//                        PersianName = item.Product.PersianName,
//                        ProductId = item.ProductId,
//                        Count = item.Count
//                    });
//                }
//            if (gBasket != null)
//                foreach (var item in gBasket)
//                {
//                    BasketList.Add(new
//                    {
//                        BasketId = guestBasket.Id,
//                        Image = BaseUrl + item.Product.AppSmallImage,
//                        PersianName = item.Product.PersianName,
//                        ProductId = item.ProductId,
//                        Count = item.Count
//                    });
//                }
//            if (BasketList.Count == 0)
//            {
//                return (new List<ProductInCount>());
//            }
//            else
//            {
//                List<ProductInCount> list = new List<ProductInCount>();
//                foreach (var price in BasketList.DistinctBy(currrent => currrent.ProductId))
//                {
//                    Product prod = db.Products.Find(price.ProductId);
//                    if (prod == null)
//                    {
//                        return (new List<ProductInCount>());
//                    }
//                    ProductInCount item = new ProductInCount()
//                    {
//                        Id = price.ProductId,
//                        Count = price.Count
//                    };
//                    list.Add(item);
//                }
//                return (list);
//            }
//            #endregion FindBasket
//        }
//        void BypassCertificateError()
//        {
//            ServicePointManager.ServerCertificateValidationCallback +=
//                delegate (
//                    object sender1,
//                    X509Certificate certificate,
//                    X509Chain chain,
//                    SslPolicyErrors sslPolicyErrors)
//                {
//                    return true;
//                };
//        }

//        private string GetDate()
//        {
//            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') +
//            DateTime.Now.Day.ToString().PadLeft(2, '0');
//        }
//        private string GetTime()
//        {
//            return DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') +
//            DateTime.Now.Second.ToString().PadLeft(2, '0');
//        }

//        [AcceptVerbs("Post", "PUT")]
//        [Route("Orders/SubmitActivity")]
//        public IHttpActionResult SubmitOrder(HttpRequestMessage requset)
//        {

//            #region Request
//            var body = requset.Content;
//            var jsonContent = body.ReadAsStringAsync().Result;
//            SubmitOrder requestParams = JsonConvert.DeserializeObject<SubmitOrder>(jsonContent);
//            if (string.IsNullOrEmpty(requestParams.UserId))
//            {
//                return Ok(new { Status = 0, Text = "لطفا شناسه کاربری را وارد نمایید ." });
//            }
//            ApplicationUser User = db.Users.Where(current => current.Id == requestParams.UserId).Include(current => current.Discounts).FirstOrDefault();
//            if (User == null)
//            {
//                return Ok(new { Status = 0, Text = "کاربر مورد نظر پیدا نشد ." });
//            }
//            #endregion Request

//            //if (requestParams.PaymentType == null || requestParams.PaymentType == 0 || requestParams.Type == null || requestParams.Type == 0)
//            //{
//            //    return Ok(new { Status = 0, Text = "لطفا نوع پرداخت و نوع سفارش را تعیین نمایید ." });
//            //}

//            if (BasketCount(requestParams.UserId, requestParams.GuId) == 0)
//            {
//                return Ok(new { Status = 0, Text = "سبد خرید شما خالی است و امکان ثبت سفارش وجود ندارد ." });
//            }

//            if (string.IsNullOrEmpty(requestParams.FullName))
//            {
//                return Ok(new { Status = 0, Text = "لطفا نام و نام خانوادگی گیرنده را وارد نمایید ." });
//            }

//            //if (string.IsNullOrEmpty(requestParams.Phone))
//            //{
//            //    return Ok(new { Status = 0, Text = "لطفا شماره موبایل گیرنده را وارد نمایید ." });
//            //}

//            if (!Regex.Match(requestParams.Phone, @"(\+98|0)?9\d{9}").Success)
//            {
//                return Ok(new { Status = 0, Text = "لطفا شماره موبایل معتبری را وارد نمایید ." });
//            }

//            if (string.IsNullOrEmpty(requestParams.UserAddress))
//            {
//                return Ok(new { Status = 0, Text = "لطفا آدرس گیرنده را وارد نمایید ." });
//            }

//            int TotalPrice = BasketPrice(requestParams.UserId, requestParams.GuId);

//            ApplicationUser user = db.Users.Where(current => current.Id == requestParams.UserId).Include(current => current.Discounts).Include(current => current.Notifications).FirstOrDefault();
//            if (user == null)
//            {
//                return Ok(new { Status = 0, Text = "شناسه کاربری شما یافت نشد ." });
//            }

//            #region Validate_Discount
//            if (!string.IsNullOrEmpty(requestParams.Discount))
//            {
//                Discount dis = db.Discounts.Where(currrent => currrent.Code == requestParams.Discount).FirstOrDefault();
//                if (dis == null)
//                {
//                    return Ok(new { Status = 0, Text = "کد تخفیف وارد شده معتبر نمیباشد ." });
//                }
//                if (dis.IsActived == false)
//                {
//                    return Ok(new { Status = 0, Text = "کد تخفیف وارد شده غیر فعال شده است ." });
//                }
//                if (dis.IsPercentage == true)
//                {
//                    if (dis.Percent == 0 || dis.Percent == null)
//                    {
//                        return Ok(new { Status = 0, Text = "کد تخفیف وارد شده مخدوش شده است ." });
//                    }
//                }
//                else
//                {
//                    if (string.IsNullOrEmpty(dis.Amount))
//                    {
//                        return Ok(new { Status = 0, Text = "کد تخفیف وارد شده مخدوش شده است ." });
//                    }
//                }
//                //اگر کد تخفیف سقف داشت .
//                if (!string.IsNullOrEmpty(dis.MaxOrder) && Convert.ToInt32(dis.MaxOrder) > 0)
//                {
//                    if (Convert.ToInt32(Convert.ToInt32(TotalPrice)) > Convert.ToInt32(dis.MaxOrder))
//                    {
//                        return Ok(new { Status = 0, Text = "مبلغ سفارش شما از کد تخفیف مورد نظر بیشتر است ." });
//                    }
//                }
//                if (dis.Count > 0 && dis.Count == dis.MaxCount)
//                {
//                    if (dis.IsActived == true)
//                    {
//                        dis.IsActived = false;
//                        db.Entry(dis).State = EntityState.Modified;
//                        db.SaveChanges();
//                        return Ok(new { Status = 0, Text = "ظرفیت استفاده از این کد تخفیف به پایان رسیده است ." });
//                    }
//                }
//                if (dis.ExpireDate != null && dis.ExpireDate <= DateTime.Now)
//                {
//                    if (dis.IsActived == true)
//                    {
//                        dis.IsActived = false;
//                        db.Entry(dis).State = EntityState.Modified;
//                        db.SaveChanges();
//                        return Ok(new { Status = 0, Text = "زمان استفاده از کد تخفیف مورد نظر شما به اتمام رسیده است ." });
//                    }
//                }
//                if (dis.ShowcaseDate != null && dis.ShowcaseDate <= DateTime.Now)
//                {
//                    if (dis.IsPublic == true)
//                    {
//                        dis.IsPublic = false;
//                        db.Entry(dis).State = EntityState.Modified;
//                        db.SaveChanges();
//                    }
//                }
//                if (user.Discounts.Contains(dis))
//                {
//                    return Ok(new { Status = 0, Text = "شما قبلا از این کد تخفیف استفاده کرده اید ." });
//                }
//            }
//            #endregion Validate_Discount

//            #region Order
//            Order newOrder = new Order();
//            //آنلاین
//            //اتصال به درگاه
//            Random rnd = new Random();
//            if (string.IsNullOrEmpty(requestParams.FullName))
//            {
//                newOrder.FullName = User.FirstName + " " + User.LastName;
//            }
//            else
//            {
//                newOrder.FullName = requestParams.FullName;
//            }
//            if (string.IsNullOrEmpty(requestParams.Phone))
//            {
//                newOrder.Phone = User.Mobile;
//            }
//            else
//            {
//                newOrder.Phone = requestParams.Phone;
//            }
//            if (string.IsNullOrEmpty(requestParams.UserAddress))
//            {
//                newOrder.UserAddress = User.AddressLine;
//            }
//            else
//            {
//                newOrder.UserAddress = requestParams.UserAddress;
//            }
//            string UserId = requestParams.UserId;
//            newOrder.FactorNumber = rnd.Next(11111, 99999).ToString();
//            newOrder.InvoiceNumber = rnd.Next(11111, 99999).ToString();
//            newOrder.PaymentType = 2;
//            newOrder.UserOrderDescription = requestParams.UserDescription;
//            newOrder.TotalPrice = TotalPrice.ToString();
//            newOrder.Receiver = 1;
//            newOrder.Type = 1;
//            newOrder.UserId = UserId;
//            newOrder.OrderDate = DateTime.Now;

//            try
//            {
//                db.Orders.Add(newOrder);
//                db.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید ." });
//            }

//            if (BasketItems(requestParams.UserId, requestParams.GuId).Count > 0)
//            {
//                foreach (var item in BasketItems(requestParams.UserId, requestParams.GuId))
//                {
//                    Product pro = db.Products.Find(item.Id);
//                    if (pro != null)
//                    {
//                        ProductInOrder productInOrder = new ProductInOrder();
//                        productInOrder.ProductId = item.Id;
//                        productInOrder.OrderId = newOrder.Id;
//                        productInOrder.Count = item.Count;
//                        try
//                        {
//                            Product product = db.Products.Find(Convert.ToInt64(item.Id));
//                            if (product != null && product.Stock > 0)
//                            {
//                                product.Stock -= Convert.ToInt32(item.Count);
//                                db.Entry(product).State = EntityState.Modified;
//                            }
//                            newOrder.Done = true;
//                            db.ProductInOrders.Add(productInOrder);
//                            db.Entry(newOrder).State = EntityState.Modified;
//                            db.SaveChanges();
//                        }
//                        catch (Exception)
//                        {
//                            db.Orders.Remove(newOrder);
//                            db.SaveChanges();
//                            return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید ." });
//                        }
//                    }
//                }

//                #region HasDiscount
//                if (!string.IsNullOrEmpty(requestParams.Discount))
//                {
//                    Discount dis = db.Discounts.Where(currrent => currrent.Code == requestParams.Discount).FirstOrDefault();
//                    dis.Count++;
//                    db.Entry(dis).State = EntityState.Modified;
//                    user.Discounts.Add(dis);
//                    db.Entry(user).State = EntityState.Modified;
//                    try
//                    {
//                        db.SaveChanges();
//                        if (dis.IsPercentage == true)
//                        {
//                            var discountPrice = int.Parse(newOrder.TotalPrice) - int.Parse(newOrder.TotalPrice) * (dis.Percent) / 100;
//                            newOrder.TotalPrice = Convert.ToInt32(discountPrice).ToString();
//                        }
//                        else
//                        {
//                            int cost = Convert.ToInt32(newOrder.TotalPrice) - Convert.ToInt32(dis.Amount);
//                            if (cost > 0)
//                                newOrder.TotalPrice = cost.ToString();
//                            else
//                                newOrder.TotalPrice = "0";
//                        }
//                        db.Entry(newOrder).State = EntityState.Modified;
//                        db.SaveChanges();
//                    }
//                    catch (Exception ex)
//                    {
//                        return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید . 5" });
//                    }
//                }
//                #endregion HasDiscount

//                #region Conditional_Discount
//                Discount conditionalDiscount = db.Discounts.Where(current => current.DiscountCount != null && current.DiscountCount > 0 && current.IsActived == true).FirstOrDefault();
//                if (conditionalDiscount != null)
//                {
//                    //مکرر هست
//                    if (conditionalDiscount.IsRepeated == true)
//                    {
//                        var catOrder = db.Orders.Where(current => current.UserId == newOrder.UserId && current.Done == true);
//                        int remain = catOrder.Count() % (int)conditionalDiscount.DiscountCount;
//                        if (remain == 0)
//                        {
//                            Notification notification = new Notification();
//                            notification.Text = "کد تخفیف جدید برای شما : " + conditionalDiscount.Code;
//                            notification.ForwardDate = DateTime.Now;
//                            db.Notifications.Add(notification);
//                            db.SaveChanges();
//                            user.Notifications.Add(notification);
//                            db.SaveChanges();
//                        }
//                    }
//                    //مکرر نیست
//                    else
//                    {
//                        var catOrder = db.Orders.Where(current => current.UserId == newOrder.UserId && current.Done == true);
//                        if (catOrder.Count() == 1 && conditionalDiscount.DiscountCount == 1)
//                        {
//                            if (conditionalDiscount.IsPercentage == true)
//                            {
//                                var discountPrice = int.Parse(newOrder.TotalPrice) - int.Parse(newOrder.TotalPrice) * (conditionalDiscount.Percent) / 100;
//                                newOrder.TotalPrice = Convert.ToInt32(discountPrice).ToString();
//                            }
//                            else
//                            {
//                                int cost = Convert.ToInt32(newOrder.TotalPrice) - Convert.ToInt32(conditionalDiscount.Amount);
//                                if (cost > 0)
//                                    newOrder.TotalPrice = cost.ToString();
//                                else
//                                    newOrder.TotalPrice = "0";
//                            }
//                            db.Entry(newOrder).State = EntityState.Modified;
//                            db.SaveChanges();
//                        }
//                        else
//                        {
//                            if (catOrder.Count() == conditionalDiscount.DiscountCount)
//                            {
//                                Notification notification = new Notification();
//                                notification.Text = "کد تخفیف جدید برای شما : " + conditionalDiscount.Code;
//                                notification.ForwardDate = DateTime.Now;
//                                db.Notifications.Add(notification);
//                                db.SaveChanges();
//                                user.Notifications.Add(notification);
//                                db.SaveChanges();
//                            }
//                        }
//                    }
//                }
//                #endregion Conditional_Discount

//                if (Convert.ToInt32(newOrder.TotalPrice) < 1000)
//                {
//                    return Ok(new { Status = 0, Text = "حداقل مبلغ تراکنش 1000 ریال است ." });
//                }

//                //sep
//                #region Saman_Sep
//                Sep.Init.Payment.PaymentIFBindingSoapClient bankInit = new Sep.Init.Payment.PaymentIFBindingSoapClient();
//                BypassCertificateError();
//                var callbackurl = "http://TajrishSamak.com/Home/Verify?OrderId=" + newOrder.Id;
//                Random random = new Random();
//                var randomNumber = random.Next(10000, 99999);
//                string result = bankInit.RequestToken("11522753",
//                    randomNumber.ToString(),
//                    Convert.ToInt64(newOrder.TotalPrice),
//                    0,
//                    0,
//                    0,
//                    0,
//                    0,
//                    0,
//                    "پرداخت هزینه سفارش در کلینیک شنوایی و سمعک شکوه تجریش",
//                    "",
//                    0
//                   );
//                if (result != null)
//                {
//                    string res = string.Empty;
//                    string[] ResultArray = result.Split(',');
//                    res = ResultArray[0].ToString();
//                    int n;
//                    bool isNumeric = int.TryParse(res, out n);
//                    if (isNumeric == false)
//                    {
//                        return Ok(new { Status = 1, Text = "http://TajrishSamak.com/home/samangateway?Token=" + res + "&RedirectURL=" + callbackurl, Online = "True" });
//                    }
//                    else
//                    {
//                        return Ok(new { Status = 0, Text = "امکان اتصال به درگاه بانک وجود ندارد" });
//                    }
//                }
//                return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید ." });
//                #endregion Saman_Sep

//            }
//            else
//            {
//                db.Orders.Remove(newOrder);
//                db.SaveChanges();
//                return Ok(new { Status = 0, Text = "سبد خرید شما خالی است لطفا مجدد تلاش بفرمایید ." });
//            }
//        }
//        #endregion Order

//        //Application app = db.Applications.FirstOrDefault();
//        //    if (app != null && app.AfterUserOrderText != null)
//        //    {
//        //        IHtmlString htmlString = new HtmlString(app.AfterUserOrderText);
//        //string htmlResult = Regex.Replace(htmlString.ToString(), @"<[^>]*>", string.Empty).Replace("\r", "").Replace("\n", "").Replace("&nbsp", ""); ;
//        //        return Ok(new { Status = 0, Text = htmlResult, Online = "False" });
//        //    }
//        //    else
//        //    {
//        //        return Ok(new { Status = 0, Text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید کاربر گرامی سفارش شما با کد " + newOrder.Id + " با موفقیت در سیستم ثبت گردید .", Online = "False" });
//        //    }
//    }
//}

