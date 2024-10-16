using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class Upload
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("آدرس تصویر")]
        [Display(Name = "آدرس تصویر")]
        public string Image { get; set; }

        [DisplayName("تصویر موبایل")]
        [Display(Name = "تصویر موبایل")]
        public string MobImage { get; set; }

        [DisplayName("لینک")]
        [Display(Name = "لینک")]
        public string Link { get; set; }

        [DisplayName("تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }

        [DisplayName("تاریخ به روز رسانی")]
        public DateTime? LastDate { get; set; }

        [DisplayName("نمایش در اپلیکیشن دارد ؟")]
        [Display(Name = "نمایش در اپلیکیشن دارد ؟")]
        public bool IsApp { get; set; }
    }
}
