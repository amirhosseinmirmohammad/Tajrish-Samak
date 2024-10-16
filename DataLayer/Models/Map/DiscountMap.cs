namespace DataLayer.Models.Mapping
{
    public class DiscountMap
    : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Discount>
    {
        public DiscountMap()
        {
            this.HasKey(current => current.Id);

            this.Property(current => current.Percent)
                .IsOptional();

            this.Property(current => current.Title)
                .IsRequired();

            this.Property(current => current.Code)
                .IsRequired();

            this.HasMany(current => current.Users)
                .WithMany(User => User.Discounts)
                .Map(current =>
             {
                current.ToTable("UserDiscounts");
                current.MapLeftKey("DiscountId");
                current.MapRightKey("UserId");
             });
        }
    }
}
