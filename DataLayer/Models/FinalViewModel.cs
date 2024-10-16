using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models
{
    public class FinalViewModel
    {
        [DisplayName("نام و نام خانوادگی کاربر")]
        [Display(Name = "نام و نام خانوادگی کاربر")]
        [Required(ErrorMessage = "لطفا نام و نام خانوادگی خود را وارد نمایید .")]
        public string FullName { get; set; }

        [DisplayName("شماره موبایل فعال")]
        [Display(Name = "شماره موبایل فعال")]
        [RegularExpression(@"(\+98|0)?9\d{9}", ErrorMessage = "لطفا شماره موبایل معتبری را وارد وارد کنید")]
        [Required(ErrorMessage = "لطفا شماره موبایل فعالی را وارد نمایید .")]
        public string Phone { get; set; }

        [DisplayName("آدرس کاربر")]
        [Display(Name = "آدرس کاربر")]
        [Required(ErrorMessage = "لطفا آدرس کامل خود را وارد نمایید .")]
        [DataType(DataType.MultilineText)]
        public string UserAddress { get; set; }

        [DisplayName("توضیحات کاربر هنگام ثبت سفارش")]
        [Display(Name = "توضیحات کاربر هنگام ثبت سفارش")]
        [DataType(DataType.MultilineText)]
        public string UserOrderDescription { get; set; }

        [DisplayName("کد تخفیف")]
        [Display(Name = "کد تخفیف")]
        public string Discount { get; set; }
    }
}
