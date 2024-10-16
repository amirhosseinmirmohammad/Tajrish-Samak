using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
   public class Basket
    {
        public Basket()
        {
            ProductInBaskets = new List<ProductInBasket>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        [Key]
        public long Id { get; set; }

        [DisplayName("شناسه کاربر")]
        [Display(Name = "شناسه کاربر")]
        public string UserId { get; set; }

        [DisplayName("شناسه مهمان")]
        [Display(Name = "شناسه مهمان")]
        public string GuId { get; set; }

        [DisplayName("تاریخ ایجاد سبد")]
        [Display(Name = "تاریخ ایجاد سبد")]
        public DateTime CreateDate { get; set; }
        public virtual ICollection<ProductInBasket> ProductInBaskets { get; set; }

    }
}
