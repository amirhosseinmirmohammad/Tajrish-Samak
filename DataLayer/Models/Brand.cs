using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class Brand
    {
        public Brand()
        {
            SubCategories = new List<Brand>();
            Products = new List<Product>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("نام فارسی")]
        [Display(Name = "نام فارسی")]
        [Required(ErrorMessage = "لطفا نام فارسی برند را تعیین نمایید .")]
        public string PersianName { get; set; }

        [DisplayName("نام لاتین")]
        [Display(Name = "نام لاتین")]
        [Required(ErrorMessage = "لطفا نام لاتین برند را تعیین نمایید .")]
        public string EnglishName { get; set; }

        [DisplayName("آیکن برند")]
        [Display(Name = "آیکن برند")]
        public string Icon { get; set; }

        [DisplayName("تصویر کوچک برند")]
        [Display(Name = "تصویر کوچک برند")]
        public string SmallImage { get; set; }

        [DisplayName("تصویر بزرگ برند")]
        [Display(Name = "تصویر بزرگ برند")]
        public string LargeImage { get; set; }

        [DisplayName("تصویر پس زمینه برند")]
        [Display(Name = "تصویر پس زمینه برند")]
        public string BackgroundProImage { get; set; }

        [DisplayName("برند پدر")]
        [Display(Name = "برند پدر")]
        public int? ParentId { get; set; }
        public virtual Brand Parent { get; set; }

        [DisplayName("لیست زیر برند ها")]
        [Display(Name = "لیست زیر برند ها")]
        public virtual ICollection<Brand> SubCategories { get; set; }

        [DisplayName("لیست محصولات")]
        [Display(Name = "لیست محصولات")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
