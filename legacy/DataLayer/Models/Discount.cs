using GladcherryShopping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models{
public class Discount
    {
        public Discount()
        {
            Users = new List<ApplicationUser>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("کد تخفیف")]
        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = "لطفا کد تخفیف را تعیین نمایید .")]
        public string Code { get; set; }

        [DisplayName("عنوان")]
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا عنوان را تعیین نمایید .")]
        public string Title { get; set; }

        [DisplayName("درصد تخفیف")]
        [Display(Name = "درصد تخفیف")]
        public float? Percent { get; set; }

        [DisplayName("این تخفیف درصدی محاسبه میشود ؟")]
        [Display(Name = "این تخفیف درصدی محاسبه میشود ؟")]
        public bool IsPercentage { get; set; }

        [DisplayName("مقدار تخفیف به ریال")]
        [Display(Name = "مقدار تخفیف به ریال")]
        public string Amount { get; set; }

        [DisplayName("سقف سفارش به ریال")]
        [Display(Name = "سقف سفارش به ریال")]
        public string MaxOrder { get; set; }

        [DisplayName("تعداد استفاده شده")]
        [Display(Name = "تعداد استفاده شده")]
        public int Count { get; set; }

        [DisplayName("حداکثر تعداد")]
        [Display(Name = "حداکثر تعداد")]
        public int? MaxCount { get; set; }

        [DisplayName("فعال یا غیر فعال ؟")]
        [Display(Name = "فعال یا غیر فعال ؟")]
        public bool IsActived { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        [DisplayName("تخفیف ویترین میباشد ؟")]
        [Display(Name = "تخفیف ویترین میباشد ؟")]
        public bool IsPublic { get; set; }

        [DisplayName("تاریخ انقضا")]
        [Display(Name = "تاریخ انقضا")]
        public DateTime? ExpireDate { get; set; }

        [DisplayName("تاریخ انقضا")]
        [Display(Name = "تاریخ انقضا")]
        public string ExDate { get; set; }

        [DisplayName("درصد تخفیف برای تعداد سفارشات مشخص")]
        [Display(Name = "درصد تخفیف برای تعداد سفارشات مشخص")]
        public float? DiscountPercent { get; set; }

        [DisplayName("تخفیف برای تعداد سفارش مشخص")]
        [Display(Name = "تخفیف برای تعداد سفارش مشخص")]
        public int? DiscountCount { get; set; }

        [DisplayName("مکرر میباشد ؟")]
        [Display(Name = "مکرر میباشد ؟")]
        public bool IsRepeated { get; set; }

        [DisplayName("تصویر کوچک")]
        [Display(Name = "تصویر کوچک")]
        public string Image { get; set; }

        [DisplayName("تاریخ اتمام ویترین")]
        [Display(Name = "تاریخ اتمام ویترین")]
        public DateTime? ShowcaseDate { get; set; }

        [DisplayName("تاریخ اتمام ویترین")]
        [Display(Name = "تاریخ اتمام ویترین")]
        public string ShDate { get; set; }

        [DisplayName("تخفیف برای تعداد معرفی مشخص اپلیکیشن")]
        [Display(Name = "تخفیف برای تعداد معرفی مشخص اپلیکیشن")]
        public int? IntroductionCount { get; set; }
    }
}

