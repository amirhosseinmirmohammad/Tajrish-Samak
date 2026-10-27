using DataLayer.Models;
using GladcherryShopping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DataLayer.Models
{
    public class Service
    {
        public Service()
        {
            Images = new List<File>();
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("تعداد بازدید")]
        public int? Survey { get; set; }

        public int? Like { get; set; }

        public int? DissLike { get; set; }

        [Required]
        [DisplayName("نمایش داده شود؟")]
        public bool IsVisible { get; set; }

        [DisplayName("تاریخ ثبت")]
        public DateTime? CreateDate { get; set; }

        [StringLength(50)]
        [DisplayName("عنوان سرویس")]
        [MinLength(3, ErrorMessage = "عنوان سرویس باید حداقل شامل 3 حرف باشد .")]
        [Required(ErrorMessage = " لطفا عنوان سرویس را وارد نمایید.")]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [StringLength(120)]
        [DisplayName("توضیحات کوتاه")]
        [MinLength(10, ErrorMessage = "توضیحات کوتاه سرویس باید حداقل شامل 10 حرف باشد .")]
        [Required(ErrorMessage = " لطفا توضیحات کوتاه سرویس را وارد نمایید.")]
        [DataType(DataType.MultilineText)]
        public string ShortDesc { get; set; }

        [AllowHtml]
        [DisplayName("متن اصلی")]
        [MinLength(20, ErrorMessage = "توضیحات سرویس باید حداقل شامل 20 حرف باشد .")]
        [Required(ErrorMessage = "لطفا متن اصلی سرویس را وارد نمایید.")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [StringLength(160)]
        [DisplayName("آدرس SEF")]
        [MinLength(5, ErrorMessage = "سف یو آر ال سرویس باید حداقل شامل 5 حرف باشد .")]
        [Required(ErrorMessage = " لطفا آدرس SEF Url را وارد نمایید")]
        [DataType(DataType.Text)]
        public string SefUrl { get; set; }

        [StringLength(160)]
        [DisplayName("کلمات کلیدی")]
        [MinLength(5, ErrorMessage = "کلمات کلیدی سرویس باید حداقل شامل 5 حرف باشد .")]
        [MaxLength(160, ErrorMessage = "کلمات کلیدی متا باید حداکثر شامل 160 حرف باشد .")]
        [Required(ErrorMessage = "لطفا کلمات کلیدی متا را وارد نمایید .")]
        [DataType(DataType.Text)]
        public string MetaKey { get; set; }

        [StringLength(160)]
        [DisplayName("توضیحات متا")]
        [MinLength(10, ErrorMessage = "توضیحات متا سرویس باید حداقل شامل 10 حرف باشد .")]
        [MaxLength(160, ErrorMessage = "توضیحات متا سرویس باید حداکثر شامل 160 حرف باشد .")]
        [Required(ErrorMessage = "لطفا توضیحات متا را وارد نمایید .")]
        [DataType(DataType.MultilineText)]
        public string MetaDesc { get; set; }
        public virtual ICollection<File> Images { get; set; }

        [DisplayName("نام کاربری")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [DisplayName("دسته بندی")]
        public int? CategoryId { get; set; }
        public virtual ServiceCategory Category { get; set; }
    }
}