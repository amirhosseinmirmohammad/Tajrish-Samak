using GladcherryShopping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DataLayer.Models
{
    public class SentEmail : object
    {
        public SentEmail()
        {
            Recipients = new List<ApplicationUser>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("تاریخ ارسال")]
        [Display(Name = "تاریخ ارسال")]
        public DateTime SendDate { get; set; }

        [DisplayName("عنوان ایمیل")]
        [Display(Name = "عنوان ایمیل")]
        [Required(ErrorMessage = "لطفا عنوان ایمیل خود را تعیین نمایید .")]
        public string Title { get; set; }

        [AllowHtml]
        [DisplayName("متن ایمیل")]
        [Display(Name = "متن ایمیل")]
        [Required(ErrorMessage = "لطفا متن ایمیل خود را تعیین نمایید .")]
        [DataType(dataType: DataType.MultilineText)]
        public string Text { get; set; }

        [DisplayName("از طرف")]
        [Display(Name = "از طرف")]
        public string FromEmail { get; set; }
        public virtual ICollection<ApplicationUser> Recipients { get; set; }
    }
}
