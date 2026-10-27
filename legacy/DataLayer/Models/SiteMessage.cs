using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class SiteMessage
    {
        public SiteMessage() { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [DisplayName("نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا نام و نام خانوادگی خود را تعیین نمایید")]
        public string FullName { get; set; }

        [Display(Name = "متن پیام")]
        [DisplayName("متن پیام")]
        [Required(ErrorMessage = "لطفا متن پیام خود را تعیین نمایید")]
        [DataType(dataType: DataType.MultilineText)]
        public string Body { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
