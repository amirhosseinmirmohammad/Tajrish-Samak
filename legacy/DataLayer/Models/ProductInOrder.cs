using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models
{
    public class ProductInOrder : object
    {
        public ProductInOrder() { }

        public int Id { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        [DisplayName("تعداد سفارش ها")]
        [Display(Name = "تعداد سفارش ها")]
        public int Count { get; set; }
    }
}