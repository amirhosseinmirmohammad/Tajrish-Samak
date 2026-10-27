using GladcherryShopping.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models
{
    public partial class Comment
    {

        [Key]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("متن نظر")]
        [Required(ErrorMessage="لطفا متن نظر خود را وارد نمایید.")]
        public string Text { get; set; }

        [DisplayName("تاریخ ثبت نظر")]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [DisplayName("تایید شده")]
        public bool IsApprove { get; set; }

        [DisplayName("ایمیل")]
        [Required(ErrorMessage = "لطفا ایمیل خود را وارد نمایید .")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "لطفا ایمیل معتبری را وارد نمایید")]
        public string Email { get; set; }

        [DisplayName("نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا نام و نام خانوادگی را وارد نمایید .")]
        public string Fullname { get; set; }

        [DisplayName("نام کاربری")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [DisplayName("نام محصول")]
        [Required(ErrorMessage = "لطفا محصول مرتبط را تعیین نمایید .")]
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
