using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Mapping
{
    public class PaymentMap : EntityTypeConfiguration<Payment>
    {
        public PaymentMap()
        {
            this.Property(current => current.Id)
                .IsRequired();
            this.Property(current => current.Description)
                .IsOptional();

            // Table Relations
            this.HasOptional(current => current.User)
                .WithMany(User => User.Payments)
                .HasForeignKey(current => current.UserId)
                .WillCascadeOnDelete(false);
            this.HasOptional(current => current.Order)
                .WithMany(Order => Order.Payments)
                .HasForeignKey(current => current.OrderId)
                .WillCascadeOnDelete(false);
            this.HasOptional(current => current.Transaction)
                .WithMany(Transaction => Transaction.Payments)
                .HasForeignKey(current => current.TransactionId)
                .WillCascadeOnDelete(false);
        }
    }
}
