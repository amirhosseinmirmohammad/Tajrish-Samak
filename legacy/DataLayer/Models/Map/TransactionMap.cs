using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Mapping
{
    class TransactionMap
      : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Transaction>
    {
        public TransactionMap()
        {
            this.Property(current => current.Acceptor)
                //.HasMaxLength(50)
                .IsOptional()
                .IsUnicode()
                .IsVariableLength();

            this.Property(current => current.AcceptorPhoneNumber)
                .IsOptional();

            this.Property(current => current.AcceptorPostalCode)
                .IsOptional();

            this.Property(current => current.BankName)
                //.HasMaxLength(50)
                .IsOptional()
                .IsUnicode()
                .IsVariableLength();

            this.Property(current => current.CardNumber)
                .IsOptional();

            this.Property(current => current.InvoiceNumber)
                .IsOptional();

            this.Property(current => current.Number)
                .IsOptional();

            //this.Property(current => current.OrderId)
            //    .IsRequired();

            this.HasOptional(current => current.Order)
                .WithMany(Order => Order.Transactions)
                .HasForeignKey(current => current.OrderId)
                .WillCascadeOnDelete(false);

            this.Property(current => current.RecievedDocumentNumber)
                .IsOptional();

            this.Property(current => current.RecievedDocumentDate)
                .IsOptional();

            this.Property(current => current.TerminalNumber)
                .IsOptional();
        }
    }
}
