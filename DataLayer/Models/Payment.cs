using GladcherryShopping.Models;
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
    public class Payment
    {
        public Payment()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("مبلغ به ریال")]
        [Display(Name = "مبلغ به ریال")]
        [Required(ErrorMessage = " لطفا مبلغ را وارد نمایید . ")]
        public long Amount { get; set; }

        [DisplayName("واریز کننده")]
        [Display(Name = "واریز کننده")]
        public byte Status { get; set; }

        [NotMapped]
        public string getStatus
        {
            get
            {
                switch (Status)
                {
                    case 1:
                        return "مدیریت";
                    case 2:
                        return "تامین کننده";
                    case 3:
                        return "کاربر";
                    default:
                        return "Error";
                }
            }
            set { }
        }

        [DisplayName("نوع عملیات")]
        [Display(Name = "نوع عملیات")]
        public byte ActionType { get; set; }

        [NotMapped]
        public string getActionType
        {
            get
            {
                switch (ActionType)
                {
                    case 1:
                        return "بدهکار";
                    case 2:
                        return "افزایش اعتبار";
                    case 3:
                        return "بستانکار";
                    default:
                        return "Error";
                }
            }
            set { }
        }

        [DisplayName("تاریخ پرداختی")]
        [Display(Name = "تاریخ پرداختی")]
        public DateTime? MainDate { get; set; }

        [DisplayName("تاریخ پرداختی")]
        [Display(Name = "تاریخ پرداختی")]
        public string Mdate { get; set; }

        [DisplayName("موضوع پرداختی")]
        [Display(Name = "موضوع پرداختی")]
        public string Subject { get; set; }

        [DisplayName("توضیحات")]
        [Display(Name = "توضیحات")]
        [DataType(dataType: DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("زمان پرداختی")]
        [Display(Name = "زمان پرداختی")]
        public DateTime CreateDate { get; set; }

        [DisplayName("کاربر")]
        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [DisplayName("سفارش مربوطه")]
        [Display(Name = "سفارش مربوطه")]
        public int? OrderId { get; set; }
        public virtual Order Order { get; set; }

        [DisplayName("تراکنش مربوطه")]
        [Display(Name = "تراکنش مربوطه")]
        public int? TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }

        [DisplayName("کد پیگیری واریز")]
        [Display(Name = "کد پیگیری واریز")]
        public string TrackingCode { get; set; }

        [DisplayName("آیا مبلغ بلوکه شده ؟")]
        [Display(Name = "آیا مبلغ بلوکه شده ؟")]
        public bool IsBlocked { get; set; }
    }
}
