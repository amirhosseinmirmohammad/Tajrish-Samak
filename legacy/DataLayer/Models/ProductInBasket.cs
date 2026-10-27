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
   public class ProductInBasket
    {
        public ProductInBasket()
        {
        }
        public int Id { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public long BasketId { get; set; }
        public virtual Basket Basket { get; set; }

        [DisplayName("تعداد محصولات")]
        [Display(Name = "تعداد محصولات")]
        public int Count { get; set; }
    }
}
