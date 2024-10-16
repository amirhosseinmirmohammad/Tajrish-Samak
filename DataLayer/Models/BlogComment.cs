using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class BlogComment
    {
        [Key]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("نام کامل")]
        [Required(ErrorMessage = "لطفا نام کامل خود را وارد نمایید .")]
        public string FullName { get; set; }

        [DisplayName("متن نظر")]
        [Required(ErrorMessage = "لطفا متن نظر خود را وارد نمایید.")]
        public string Text { get; set; }

        [DisplayName("تاریخ ثبت نظر")]
        [DataType(DataType.DateTime)]
        public DateTime? DateTime { get; set; }

        [DisplayName("تایید شده")]
        public bool IsApprove { get; set; }
        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }
    }
}
