using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DataLayer.Models
{
    public class Product
    {
        public Product()
        {
            Orders = new List<ProductInOrder>();
            Comments = new List<Comment>();
            RelatedProducts = new List<Product>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        [Key]
        public long Id { get; set; }

        [DisplayName("نام فارسی")]
        [Display(Name = "نام فارسی")]
        [Required(ErrorMessage = "لطفا نام فارسی دسته را تعیین نمایید .")]
        public string PersianName { get; set; }

        [DisplayName("نام لاتین")]
        [Display(Name = "نام لاتین")]
        [Required(ErrorMessage = "لطفا نام لاتین دسته را تعیین نمایید .")]
        public string EnglishName { get; set; }

        [DisplayName("قیمت فروش")]
        [Display(Name = "قیمت فروش")]
        [Required(ErrorMessage = "لطفا قیمت فروش را تعیین نمایید .")]
        public int UnitPrice { get; set; }

        [DisplayName("موجودی")]
        [Display(Name = "موجودی")]
        public int Stock { get; set; }

        [DisplayName("درصد تخفیف")]
        [Display(Name = "درصد تخفیف")]
        [Required(ErrorMessage = "لطفا درصد تخفیف را تعیین نمایید .")]
        public byte DiscountPercent { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "لطفا توضیحات محصول را وارد کنید")]
        [DisplayName(" توضیحات محصول")]
        [Display(Name = " توضیحات محصول")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("دسته بندی محصول")]
        [Display(Name = "دسته بندی محصول")]
        public int CategoryId { get; set; }
        public virtual Category category { get; set; }

        [DisplayName("برند محصول")]
        [Display(Name = "برند محصول")]
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }

        [DisplayName("تصویر اسلایدری محصول")]
        [Display(Name = "تصویر اسلایدری محصول")]
        public string SliderImage { get; set; }

        [DisplayName("تصویر کوچک محصول در اپلیکیشن")]
        [Display(Name = "تصویر کوچک محصول در اپلیکیشن")]
        public string AppSmallImage { get; set; }

        [DisplayName("تصویر بزرگ محصول در اپلیکیشن")]
        [Display(Name = "تصویر بزرگ محصول در اپلیکیشن")]
        public string AppLargeImage { get; set; }

        [DisplayName("تصویر اول محصول در سایت")]
        [Display(Name = "تصویر اول محصول در سایت")]
        public string SiteFirstImage { get; set; }

        [DisplayName("تصویر دوم محصول در سایت")]
        [Display(Name = "تصویر دوم محصول در سایت")]
        public string SiteSecondImage { get; set; }

        [DisplayName("تصویر سوم محصول در سایت")]
        [Display(Name = "تصویر سوم محصول در سایت")]
        public string SiteThirdImage { get; set; }

        [DisplayName("تاریخ ایجاد محصول")]
        [Display(Name = "تاریخ ایجاد محصول")]
        public DateTime CreateDate { get; set; }

        [DisplayName("محصول ویژه است ؟")]
        [Display(Name = "محصول ویژه است ؟")]
        public bool IsSpecial { get; set; }

        [DisplayName("قیمت با تخفیف")]
        [Display(Name = "قیمت با تخفیف")]
        public int _Discount { get; set; }

        [NotMapped]
        public int FinalPrice
        {
            get
            {
                _Discount += UnitPrice - (UnitPrice) * (DiscountPercent) / 100;
                return _Discount;
            }
        }
        public virtual ICollection<ProductInOrder> Orders { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<ProductInBasket> ProductInBaskets { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Product> RelatedProducts { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return PersianName + " ، کد : " + Id;
            }
        }

    }
}
