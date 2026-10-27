namespace DataLayer.Models.Mapping
{
    class OrderMap
        : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            this.ToTable("Orders");
            this.Property(current => current.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            this.HasKey(current => current.Id);
            this.Property(current => current.Receiver)
                .IsRequired();

            this.Property(current => current.OrderDate)
                .IsRequired();

            this.Property(current => current.TotalPrice)
                .IsOptional();

            this.Property(current => current.UserId)
                .IsOptional();
            this.Property(current => current.FactorNumber)
                .IsOptional();
            this.Property(current => current.IsCanceled)
                .IsOptional();
            this.Property(current => current.CancelDescription)
                .IsOptional();

            // Table Relations
            this.HasOptional(current => current.User)
                .WithMany(User => User.Orders)
                .HasForeignKey(current => current.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
