using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Mapping
{
    class ProductInBasketMap
         : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductInBasket>
    {
        public ProductInBasketMap()
        {
            this.ToTable("ProductInBasket");
            this.Property(current => current.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            this.HasKey(current => current.Id);
            this.Property(current => current.Count)
                .IsRequired();

            // Table Relations
            this.HasRequired(current => current.Basket)
                .WithMany(Basket => Basket.ProductInBaskets)
                .HasForeignKey(current => current.BasketId)
                .WillCascadeOnDelete(true);

            this.HasRequired(current => current.Product)
                .WithMany(Product => Product.ProductInBaskets)
                .HasForeignKey(current => current.ProductId)
                .WillCascadeOnDelete(false);

        }
    }
}
