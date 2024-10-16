using GladcherryShopping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DataLayer.Models
{
    public class Notification
    {
        public Notification()
        {
            this.Users = new List<ApplicationUser>();
            this.Images = new List<Image>();
            NewsLetters = new List<NewsLetter>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("تاریخ ارسال")]
        [Display(Name = "تاریخ ارسال")]
        public DateTime ForwardDate { get; set; }

        [AllowHtml]
        [DisplayName("متن پیام")]
        [Display(Name = "متن پیام")]
        [Required(ErrorMessage ="لطفا متن پیام خود را تعیین نمایید .")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }     
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<NewsLetter> NewsLetters { get; set; }
    }
}
