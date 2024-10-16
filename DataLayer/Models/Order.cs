using GladcherryShopping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class Order : object
    {
        public Order()
        {
            Transactions = new List<Transaction>();
            Products = new List<ProductInOrder>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("دریافت کننده سفارش")]
        [Display(Name = "دریافت کننده سفارش")]
        [Required(ErrorMessage = "لطفا دریافت کننده خدمات را تعیین نمایید .")]
        public int Receiver { get; set; }

        [DisplayName("نوع سفارش")]
        [Display(Name = "نوع سفارش")]
        [Required(ErrorMessage = "لطفا نوع سفارش را تعیین نمایید .")]
        public byte Type { get; set; }

        [DisplayName("نحوه پرداخت")]
        [Display(Name = "نحوه پرداخت")]
        [Required(ErrorMessage = "لطفا نحوه پرداخت را تعیین نمایید .")]
        public byte PaymentType { get; set; }

        [DisplayName("تاریخ سفارش")]
        [Display(Name = "تاریخ سفارش")]
        [Required(ErrorMessage = "لطفا تاریخ سفارش محصول را تعیین نمایید .")]
        public DateTime OrderDate { get; set; }
        public bool Done { get; set; }

        [DisplayName("کد پیگیری")]
        [Display(Name = "کد پیگیری")]
        public string InvoiceNumber { get; set; }

        [DisplayName("قیمت نهایی")]
        [Display(Name = "قیمت نهایی")]
        public string TotalPrice { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        [DisplayName("کاربر مربوطه")]
        [Display(Name = "کاربر مربوطه")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [DisplayName("لغو شده ؟")]
        [Display(Name = "لغو شده ؟")]
        public bool IsCanceled { get; set; }

        [DisplayName("علت لغو سفارش")]
        [Display(Name = "علت لغو سفارش")]
        public string CancelDescription { get; set; }

        [DisplayName("توضیحات کاربر هنگام ثبت سفارش")]
        [Display(Name = "توضیحات کاربر هنگام ثبت سفارش")]
        public string UserOrderDescription { get; set; }

        [DisplayName("نام و نام خانوادگی کاربر")]
        [Display(Name = "نام و نام خانوادگی کاربر")]
        public string FullName { get; set; }

        [DisplayName("شماره موبایل فعال")]
        [Display(Name = "شماره موبایل فعال")]
        //[RegularExpression(@"(\+98|0)?9\d{9}", ErrorMessage = "لطفا شماره موبایل معتبری را وارد وارد کنید")]
        public string Phone { get; set; }

        [DisplayName("آدرس کاربر")]
        [Display(Name = "آدرس کاربر")]
        public string UserAddress { get; set; }

        [DisplayName("شماره فاکتور")]
        [Display(Name = "شماره فاکتور")]
        public string FactorNumber { get; set; }

        [NotMapped]
        public string getReceiver
        {
            get
            {
                switch (Receiver)
                {
                    case 1:
                        return "کاربر";
                    case 2:
                        return "شخص دیگر";
                    default:
                        return "Error";
                }
            }
            set { }
        }

        [NotMapped]
        public string getPaymentType
        {
            get
            {
                switch (PaymentType)
                {
                    case 1:
                        return "نقدی";
                    case 2:
                        return "آنلاین";
                    default:
                        return "Error";
                }
            }
            set { }
        }

        [NotMapped]
        public string getType
        {
            get
            {
                switch (Type)
                {
                    case 1:
                        return "معمولی";
                    case 2:
                        return "اکسپرس";
                    default:
                        return "Error";
                }
            }
            set { }
        }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<ProductInOrder> Products { get; set; }

        [DisplayName("آدرس انتخابی کاربر")]
        [Display(Name = "آدرس انتخابی کاربر")]
        public int? AddressId { get; set; }
        public virtual Address Address { get; set; }

        [DisplayName("لیست گردش مالی")]
        [Display(Name = "لیست گردش مالی")]
        public virtual ICollection<Payment> Payments { get; set; }

    }
}
