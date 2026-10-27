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
    public class Transaction
    {
        public Transaction() { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("شماره تراکنش")]
        [Display(Name = "شماره تراکنش")]
        //[Required(ErrorMessage = "لطفا شماره تراکنش را تعیین نمایید .")]
        public string Number { get; set; }

        [DisplayName("تاریخ تراکنش")]
        [Display(Name = "تاریخ تراکنش")]
        public DateTime Date { get; set; }

        [DisplayName("شماره پیگیری")]
        [Display(Name = "شماره پیگیری")]
        //[Required(ErrorMessage = "لطفا شماره پیگیری را تعیین نمایید .")]
        public string InvoiceNumber { get; set; }

        [DisplayName("نام بانک")]
        [Display(Name = "نام بانکی")]
        //[Required(ErrorMessage = "لطفا نام بانک را تعیین نمایید .")]
        public string BankName { get; set; }

        [DisplayName("شماره سند دریافتی")]
        [Display(Name = "شماره سند دریافتی")]
        //[Required(ErrorMessage = "لطفا شماره سند دریافتی را تعیین نمایید .")]
        public string RecievedDocumentNumber { get; set; }

        [DisplayName("تاریخ سند دریافتی")]
        [Display(Name = "تاریخ سند دریافتی")]
        public DateTime RecievedDocumentDate { get; set; }

        [DisplayName("4 رقم آخر کارت بانکی")]
        [Display(Name = "4 رقم آخر کارت بانکی")]
        //[Required(ErrorMessage = "لطفا 4 رقم آخر کارت بانکی را تعیین نمایید .")]
        public string CardNumber { get; set; }

        [DisplayName("شماره پایانه")]
        [Display(Name = "شماره پایانه")]
        //[Required(ErrorMessage = "لطفا شماره پایانه را تعیین نمایید .")]
        public string TerminalNumber { get; set; }

        [DisplayName("پذیرنده")]
        [Display(Name = "پذیرنده")]
        //[Required(ErrorMessage = "لطفا پذیرنده را تعیین نمایید .")]
        public string Acceptor { get; set; }

        [DisplayName("نتیجه عملیات")]
        [Display(Name = "نتیجه عملیات")]
        //[Required(ErrorMessage = "لطفا نتیجه عملیات را تعیین نمایید .")]
        public byte OperationResult { get; set; }

        [DisplayName("کد پستی پذیرنده")]
        [Display(Name = "کد پستی پذیرنده")]
        //[Required(ErrorMessage = "لطفا کد پستی پذیرنده را تعیین نمایید .")]
        public string AcceptorPostalCode { get; set; }

        [DisplayName("شماره تلفن پذیرنده")]
        [Display(Name = "شماره تلفن پذیرنده")]
        //[Required(ErrorMessage = "لطفا شماره تلفن پذیرنده را تعیین نمایید .")]
        public string AcceptorPhoneNumber { get; set; }

        [NotMapped]
        public string GetOperationResult
        {
            get
            {
                switch (OperationResult)
                {
                    case 1:
                        return "موفق";
                    case 2:
                        return "ناموفق";
                    default:
                        return "Error";
                }
            }
            set { }
        }

        //[DisplayName("سفارش مربوطه")]
        //[Display(Name = "سفارش مربوطه")]
        //[Key, ForeignKey("Order")]
        //public int OrderId { get; set; }
        //public virtual Order Order { get; set; }

        //یک به یک
        [DisplayName("سفارش مربوطه")]
        [Display(Name = "سفارش مربوطه")]
        //[Key, ForeignKey("Order")]
        public int? OrderId { get; set; }
        public virtual Order Order { get; set; }

        [DisplayName("لیست گردش مالی")]
        [Display(Name = "لیست گردش مالی")]
        public virtual ICollection<Payment> Payments { get; set; }

    }
}
