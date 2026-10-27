namespace DataLayer.Models.Mapping
{
    class ProductInOrderMap 
        :System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductInOrder>
    {
        public ProductInOrderMap()
        {
            this.ToTable("ProductsInOrders");
            this.Property(current => current.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            this.HasKey(current => current.Id);
            this.Property(current => current.Count)
                .IsRequired();

            // Table Relations
            this.HasRequired(current => current.Order)
                .WithMany(Order => Order.Products)
                .HasForeignKey(current => current.OrderId)
                .WillCascadeOnDelete(true);

            this.HasRequired(current => current.Product)
                .WithMany(Product => Product.Orders)
                .HasForeignKey(current => current.ProductId)
                .WillCascadeOnDelete(false);

        }
    }
}
