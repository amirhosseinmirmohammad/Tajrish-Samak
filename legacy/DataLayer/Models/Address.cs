using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using GladcherryShopping.Models;

namespace DataLayer.Models
{
    public class Address : System.Object
    {
        public Address()
        {
            Orders = new List<Order>();
        }

        internal class configuration : EntityTypeConfiguration<Address>
        {
            public configuration()
            {
                this.HasRequired(ne => ne.Users)
                    .WithMany(applicationUser => applicationUser.Addresses)
                    .HasForeignKey(id => id.UserId)
                    .WillCascadeOnDelete(false);
            }
        }

        public int Id { get; set; }

        [Display(Name = "آدرس")]
        [DisplayName("آدرس")]
        public string Addrress { get; set; }

        [Display(Name = "عنوان ساختمان")]
        [DisplayName("عنوان ساختمان")]
        public string Title { get; set; }

        [Display(Name = "پلاک")]
        [DisplayName("پلاک")]
        public string plaque { get; set; }

        [Display(Name = "شماره واحد")]
        [DisplayName("شماره واحد")]
        public int unit { get; set; }

        [Display(Name = "تلفن ثابت")]
        [DisplayName("تلفن ثابت")]
        public string Tell { get; set; }

        [Display(Name = " شماره موبایل")]
        [DisplayName("شماره موبایل")]
        public string Mobile { get; set; }

        [Display(Name = "وضعیت")]
        [DisplayName("وضعیت")]
        public bool IsConfirm { get; set; }

        [Display(Name = "عرض جغرافیایی")]
        [DisplayName("عرض جغرافیایی")]
        public string latitude { get; set; }

        [Display(Name = "طول جغرافیایی")]
        [DisplayName("طول جغرافیایی")]
        public string longitude { get; set; }

        [Display(Name = "نوع ساختمان")]
        [DisplayName("نوع ساختمان")]
        public string Type { get; set; }

        [Display(Name = "نام کاربر")]
        [DisplayName("نام کاربر")]
        public string UserId { get; set; }
        public virtual ApplicationUser Users { get; set; }

        [DisplayName("لیست سفارش های کاربر")]
        [Display(Name = "لیست سفارش های کاربر")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
