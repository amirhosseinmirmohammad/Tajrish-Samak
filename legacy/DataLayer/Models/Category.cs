using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class Category
    {
        public Category()
        {
            SubCategories = new List<Category>();
            Products = new List<Product>();
            Blogs = new List<Blog>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("نام فارسی")]
        [Display(Name = "نام فارسی")]
        [Required(ErrorMessage = "لطفا نام فارسی دسته را تعیین نمایید .")]
        public string PersianName { get; set; }

        [DisplayName("نام لاتین")]
        [Display(Name = "نام لاتین")]
        [Required(ErrorMessage = "لطفا نام لاتین دسته را تعیین نمایید .")]
        public string EnglishName { get; set; }

        [DisplayName("آیکن دسته")]
        [Display(Name = "آیکن دسته")]
        public string Icon { get; set; }

        [DisplayName("تصویر کوچک دسته")]
        [Display(Name = "تصویر کوچک دسته")]
        public string SmallImage { get; set; }

        [DisplayName("تصویر بزرگ دسته")]
        [Display(Name = "تصویر بزرگ دسته")]
        public string LargeImage { get; set; }

        [DisplayName("تصویر پس زمینه دسته")]
        [Display(Name = "تصویر پس زمینه دسته")]
        public string BackgroundProImage { get; set; }

        [DisplayName("دسته ی پدر")]
        [Display(Name = "دسته ی پدر")]
        public int? ParentId { get; set; }
        public virtual Category Parent { get; set; }

        [DisplayName("دسته بندی بلاگ است ؟")]
        [Display(Name = "دسته بندی بلاگ است ؟")]
        public bool IsBlog { get; set; }

        [DisplayName("لیست زیر دسته ها")]
        [Display(Name = "لیست زیر دسته ها")]
        public virtual ICollection<Category> SubCategories { get; set; }

        [DisplayName("لیست محصولات")]
        [Display(Name = "لیست محصولات")]
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
