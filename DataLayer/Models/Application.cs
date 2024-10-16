using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DataLayer.Models
{
    public class Application
    {
       public Application() { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("عنوان اپلیکیشن")]
        [Display(Name = "عنوان اپلیکیشن")]
        [Required(ErrorMessage = "لطفا عنوان اپلیکیشن خود را وارد نمایید")]
        public string Title { get; set; }

        [DisplayName("ایمیل تجاری")]
        [Display(Name = "ایمیل تجاری")]
        [Required(ErrorMessage = "لطفا ایمیل تجاری خود را وارد نمایید")]
        public string BussinessEmailAddress { get; set; }

        [DisplayName("تاریخ میلادی اجرای اپلیکیشن")]
        [Display(Name = "تاریخ میلادی اجرای اپلیکیشن")]
        public DateTime RunDate { get; set; }

        [DisplayName("تاریخ شمسی اجرای اپلیکیشن")]
        [Display(Name = "تاریخ شمسی اجرای اپلیکیشن")]
        public string PersianRunDate { get; set; }

        [DisplayName("نام کاربری سرویس پیامکی")]
        [Display(Name = "نام کاربری سرویس پیامکی")]
        public string UserName { get; set; }

        [DisplayName("رمز عبور سرویس پیامکی")]
        [Display(Name = "رمز عبور سرویس پیامکی")]
        public string Password { get; set; }

        [DisplayName("شماره تلفن مبدا")]
        [Display(Name = "شماره تلفن مبدا")]
        public string FromNumber { get; set; }

        [DisplayName("سرور Smtp")]
        [Display(Name = "سرور Smtp")]
        [Required(ErrorMessage = "لطفا سرور Smtp خود را وارد نمایید")]
        public string SmtpServer { get; set; }

        [DisplayName("شماره ی پورت")]
        [Display(Name = "شماره ی پورت")]
        [Required(ErrorMessage = "لطفا شماره ی پورت خود را وارد نمایید")]
        public int PortNumber { get; set; }

        [DisplayName("آدرس ایمیل")]
        [Display(Name = "آدرس ایمیل")]
        [Required(ErrorMessage = "لطفا آدرس ایمیل خود را وارد نمایید")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DisplayName("نام کاربری ایمیل")]
        [Display(Name = "نام کاربری ایمیل")]
        [Required(ErrorMessage = "لطفا نام کاربری ایمیل خود را وارد نمایید")]
        public string EmailUserName { get; set; }

        [DisplayName("رمز ورود ایمیل")]
        [Display(Name = "رمز ورود ایمیل")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "لطفا رمز ورود ایمیل خود را وارد نمایید")]
        public string EmailPassword { get; set; }

        [AllowHtml]
        [DisplayName("متن ثبت نام موفق")]
        [Display(Name = "متن ثبت نام موفق ")]
        [Required(ErrorMessage = "لطفا متن ثبت نام موفق خود را وارد نمایید .")]
        [DataType(DataType.MultilineText)]
        public string SuccessfullRegisterationText { get; set; }

        [AllowHtml]
        [DisplayName("متن تایید شماره تلفن")]
        [Display(Name = "متن تایید شماره تلفن")]
        [Required(ErrorMessage = "لطفا متن تایید شماره تلفن را وارد نمایید .")]
        [DataType(DataType.MultilineText)]
        public string VerifyPhoneNumberText { get; set; }

        [AllowHtml]
        [DisplayName("متن پس از ثبت سفارش")]
        [Display(Name = "متن پس از ثبت سفارش")]
        [Required(ErrorMessage = "لطفا متن پس از ثبت سفارش را وارد نمایید .")]
        [DataType(DataType.MultilineText)]
        public string AfterUserOrderText { get; set; }

        [AllowHtml]
        [DisplayName("درباره ما")]
        [Display(Name = "درباره ما")]
        [Required(ErrorMessage = "لطفا متن درباره ما را وارد نمایید .")]
        [DataType(DataType.MultilineText)]
        public string AboutUs { get; set; }

        [AllowHtml]
        [DisplayName("تماس با ما")]
        [Display(Name = "تماس با ما")]
        [Required(ErrorMessage = "لطفا متن تماس با ما را وارد نمایید .")]
        [DataType(DataType.MultilineText)]
        public string ContactUs { get; set; }

        [DisplayName("افزایش اعتبار کد معرف ریال")]
        [Display(Name = "افزایش اعتبار کد معرف ریال")]
        public long IntroPercent { get; set; }

        [DisplayName("افزایش امتیاز کد معرف")]
        [Display(Name = "افزایش امتیاز کد معرف")]
        public long IntroScore { get; set; }
    }
}
