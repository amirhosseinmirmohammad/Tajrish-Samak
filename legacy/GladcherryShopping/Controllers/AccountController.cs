using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GladcherryShopping.Models;
using DataLayer.ViewModels;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DataLayer.Models;
using GladCherryShopping.Helpers;
using System.Data.Entity;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Security;
using SmsIrRestful;

namespace GladcherryShopping.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult AdminPanel()
        {
            return View();
        }

        public bool DeleteImages(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var list = db.Files.ToList();
            foreach (var image in list)
            {
                if (image.Id == id)
                {
                    db.Files.Remove(image);
                }
                db.SaveChanges();
            }
            return true;
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult UserAccount(string UserId)
        //{
        //    if (string.IsNullOrWhiteSpace(UserId))
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    return View();
        //}

        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult UserAccount()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult GenerateVerificationCode(string PhoneNumber, string Name, string Family)
        {

            #region Validation
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                TempData["Error"] = "لطفا شماره موبایل خود را وارد نمایید .";
                return RedirectToAction("Index", "Home");
            }
            if (!Regex.Match(PhoneNumber, @"\b\d{4}[\s-.]?\d{3}[\s-.]?\d{4}\b").Success)
            {
                TempData["Error"] = "لطفا شماره موبایل معتبری را وارد نمایید .";
                return RedirectToAction("Index", "Home");
            }
            IQueryable<ApplicationUser> users = db.Users.Where(current => current.Mobile == PhoneNumber || current.UserName == PhoneNumber);
            if (users.Count() > 0)
            {
                TempData["Error"] = "این شماره قبلا ثبت نام کرده است .";
                return RedirectToAction("Index", "Home");
            }
            #endregion Validation

            #region SMS
            Application application = db.Applications.FirstOrDefault();
            Random random = new Random();
            var randomNumber = random.Next(10000, 99999);
            if (application != null && application.FromNumber != null)
            {
                //sms.ir
                string SMSDescription = "کد تایید شما در کلینیک شنوایی و سمعک شکوه تجریش : " + randomNumber;
                bool smsResult = UltraFastSms(PhoneNumber, randomNumber.ToString());
            }
            else
            {
                TempData["Error"] = "تنظیمات کلی اپلیکیشن هنوز تعیین نشده است .";
                return RedirectToAction("Index", "Home");
            }
            #endregion SMS

            #region HashPassword
            PasswordHasher hashPassword = new PasswordHasher();
            var hashedPassword = hashPassword.HashPassword(PhoneNumber + "_GladCherryCo");
            var security = Guid.NewGuid().ToString();
            #endregion Hash Password

            #region NewUser
            ApplicationUser user = new ApplicationUser
            {
                Mobile = PhoneNumber,
                FirstName = Name ?? "",
                LastName = Family ?? "",
                UserName = PhoneNumber,
                UserScore = 0,
                AccessCode = "TajrishSamak_" + randomNumber,
                Credit = 0,
                IsActive = true,
                BirthDate = DateTime.Now,
                SecurityStamp = security,
                PasswordHash = hashedPassword,
                //تایید شماره کاربر
                VerificationCode = FunctionsHelper.Encrypt(randomNumber.ToString(), "Gladcherry"),
                PhoneNumberConfirmed = false,
                RegistrationDate = DateTime.Now
            };
            #endregion NewUser

            #region Create
            IdentityResult result;
            try
            {
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                result = UserManager.Create(user);
                UserManager.AddToRole(user.Id, "User");
            }
            catch (Exception)
            {
                TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .";
                return RedirectToAction("Index", "Home");
            }
            if (!result.Succeeded)
            {
                TempData["Error"] = "متاسفانه ثبت نام شما انجام نشد لطفا مجدد تلاش فرمایید .";
                return RedirectToAction("Index", "Home");
            }
            if (application != null && application.SuccessfullRegisterationText != null)
            {
                IHtmlString htmlString = new HtmlString(application.SuccessfullRegisterationText);
                string htmlResult = Regex.Replace(htmlString.ToString(), @"<[^>]*>", string.Empty).Replace("\r", "").Replace("\n", "").Replace("&nbsp", "");
                TempData["Success"] = htmlResult;
            }
            else
                TempData["Success"] = "ثبت نام شما با موفقیت انجام شد .";
            //SignIn
            return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/CreateAccount" });
            #endregion Create
        }

        public static bool UltraFastSms(string PhoneNumber, string Code)
        {
            var token = new Token().GetToken("e53a9b26a9aca9d5c6ae96e7", "it66)%18#teBC!@*&");

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = long.Parse(PhoneNumber),
                TemplateId = 16673,
                ParameterArray = new List<UltraFastParameters>()
        {
        new UltraFastParameters()
        {
            Parameter = "VerificationCode",
            ParameterValue = Code
        }
        }.ToArray()

            };

            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            if (ultraFastSendRespone.IsSuccessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LoginAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LoginUser(string PhoneNumber)
        {

            #region Validation
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                TempData["Error"] = "لطفا شماره موبایل خود را وارد نمایید .";
                return RedirectToAction("Index", "Home");
            }

            if (!Regex.Match(PhoneNumber, @"\b\d{4}[\s-.]?\d{3}[\s-.]?\d{4}\b").Success)
            {
                TempData["Error"] = "لطفا شماره موبایل معتبری را وارد نمایید .";
                return RedirectToAction("Index", "Home");
            }

            IQueryable<ApplicationUser> users = db.Users.Where(current => current.Mobile == PhoneNumber);
            if (users.Count() <= 0)
            {
                TempData["Error"] = "شماره شما در سیستم ثبت نشده است ، لطفا ابتدا ثبت نام کنید .";
                return RedirectToAction("Index", "Home");
            }

            if (users.Count() > 0 && users.FirstOrDefault().IsActive == false)
            {
                TempData["Error"] = "حساب کاربری شما غیر فعال شده است لطفا با پشتیبانی تماس حاصل فرمایید .";
                return RedirectToAction("Index", "Home");
            }
            #endregion Validation

            #region SMS
            Application application = db.Applications.FirstOrDefault();
            Random random = new Random();
            var randomNumber = random.Next(10000, 99999);
            if (application != null && application.FromNumber != null)
            {

                //sms.ir
                string SMSDescription = "کد تایید شما در کلینیک شنوایی و سمعک شکوه تجریش : " + randomNumber;
                bool smsResult = UltraFastSms(PhoneNumber, randomNumber.ToString());
                if (smsResult == false)
                {
                    TempData["Error"] = "سامانه پیامکی با خطا رو به رو است لطفا مجدد تلاش فرمایید .";
                    return View("ApproveNumber");
                }

                #region EditUserCode
                users.FirstOrDefault().VerificationCode = FunctionsHelper.Encrypt(randomNumber.ToString(), "Gladcherry");
                db.Entry(users.FirstOrDefault()).State = EntityState.Modified;
                db.SaveChanges();
                #endregion EditUserCodde

            }
            else
            {
                TempData["Error"] = "تنظیمات کلی اپلیکیشن هنوز تعیین نشده است .";
                return View("ApproveNumber");
            }
            #endregion SMS

            TempData["Success"] = "به حساب کاربری خود خوش آمدید .";
            return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Approve(string PhoneNumber, string ReturnUrl)
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ApproveNumber(string PhoneNumber, string Code, string ReturnUrl)
        {
            #region Validation
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                TempData["Error"] = "لطفا شماره موبایل خود را وارد نمایید .";
                return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
            }
            if (string.IsNullOrWhiteSpace(Code))
            {
                TempData["Error"] = "لطفا کد را وارد نمایید .";
                return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
            }
            if (!Regex.Match(PhoneNumber, @"\b\d{4}[\s-.]?\d{3}[\s-.]?\d{4}\b").Success)
            {
                TempData["Error"] = "لطفا شماره موبایل معتبری را وارد نمایید .";
                return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
            }
            IQueryable<ApplicationUser> users = db.Users.Where(current => current.Mobile == PhoneNumber);
            if (users.Count() <= 0)
            {
                TempData["Error"] = "شماره شما در سیستم ثبت نشده است .";
                return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
            }
            #endregion Validation

            #region Login
            string UserVerificationCode = FunctionsHelper.Decrypt(users.FirstOrDefault().VerificationCode, "Gladcherry");
            if (UserVerificationCode != null)
            {
                if (UserVerificationCode != Code)
                {
                    TempData["Error"] = "کد وارد شده صحیح نمیباشد .";
                    return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
                }
                if (UserVerificationCode == Code)
                {
                    users.FirstOrDefault().PhoneNumberConfirmed = true;
                    db.Entry(users.FirstOrDefault()).State = EntityState.Modified;
                    db.SaveChanges();
                    Application application = db.Applications.FirstOrDefault();
                    if (application != null && application.VerifyPhoneNumberText != null)
                    {
                        IHtmlString htmlString = new HtmlString(application.VerifyPhoneNumberText);
                        string htmlResult = Regex.Replace(htmlString.ToString(), @"<[^>]*>", string.Empty).Replace("\r", "").Replace("\n", "").Replace("&nbsp", "");
                        {
                            TempData["Success"] = htmlResult;
                            var result = SignInManager.PasswordSignIn(PhoneNumber, PhoneNumber + "_GladCherryCo", false, shouldLockout: false);
                            switch (result)
                            {
                                case SignInStatus.Success:
                                    //return RedirectToAction("UserAccount", "Account");
                                    return RedirectToAction("Index", "Home");
                                case SignInStatus.LockedOut:
                                    TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .";
                                    return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
                                case SignInStatus.RequiresVerification:
                                    TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .";
                                    return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
                                case SignInStatus.Failure:
                                default:
                                    TempData["Error"] = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .";
                                    return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
                            }
                            //return RedirectToAction("UserAccount", new { UserId = users.FirstOrDefault().Id });
                        }
                    }
                    else
                    {
                        TempData["Success"] = "شماره شما با موفقیت تایید شد .";
                        //return RedirectToAction("UserAccount", new { UserId = users.FirstOrDefault().Id });
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                TempData["Error"] = "کد کاربر خالی است .";
                return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
            }
            #endregion Login

            TempData["Error"] = "لطفا مجدد تلاش فرمایید .";
            return RedirectToAction("Approve", new { PhoneNumber = PhoneNumber, ReturnUrl = "Account/LoginAccount" });
        }

        [AllowAnonymous]
        public ActionResult LogOutUser(string UserId)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            List<HttpCookie> lst = new List<HttpCookie>();
            for (int i = Request.Cookies.Count - 1; i >= 0; i--)
            {
                if (lst.Where(p => p.Name == Request.Cookies[i].Name).Any() == false)
                {
                    lst.Add(Request.Cookies[i]);
                }
            }
            foreach (var item in lst.Where(p => p.Name.StartsWith("Ajor_")))
            {
                var id = item.Name.Substring(5).ToString();
                if (id.Contains(UserId))
                {
                    Response.Cookies["Ajor_" + UserId].Expires = DateTime.Now.AddDays(-1);
                    Request.Cookies.Remove("Ajor_" + UserId);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public void UserLoginCookie(string UserId)
        {
            //string userId = FunctionsHelper.Encrypt(UserId, "Gladcherry");
            var cookie = new HttpCookie("Ajor_" + UserId, 1.ToString());
            cookie.Expires = DateTime.Now.AddHours(2);
            cookie.HttpOnly = true;
            Response.Cookies.Add(cookie);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    //return RedirectToLocal(returnUrl);
                    return RedirectToAction("AdminPanel", "Account");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = "Glad", LastName = "Cherry", RegistrationDate = DateTime.Now };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        public static bool SendSms(string SmsServiceNumber, string PhoneNumber, string Description)
        {
            if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(SmsServiceNumber))
                return false;
            SmsIrRestful.Token tokenInstance = new SmsIrRestful.Token();
            var token = tokenInstance.GetToken("9b2a827d86a88a4dac1670f2", "it66)%#teBC!@*&");
            SmsIrRestful.MessageSend messageInstace = new SmsIrRestful.MessageSend();
            var res = messageInstace.Send(token, new SmsIrRestful.MessageSendObject()
            {
                MobileNumbers = new List<string>() { PhoneNumber }.ToArray(),
                Messages = new List<string>() { Description }.ToArray(),
                LineNumber = SmsServiceNumber,
                SendDateTime = DateTime.Now,
                CanContinueInCaseOfError = false
            });
            if (res.IsSuccessful == true)
                return true;
            else
                return false;
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}