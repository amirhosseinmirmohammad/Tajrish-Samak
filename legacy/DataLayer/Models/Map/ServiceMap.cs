namespace DataLayer.Models.Mapping
{
    public class ServiceMap : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Service>
    {
        public ServiceMap()
        {

            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.UserId).HasColumnName("UserId");

            this.HasRequired(current => current.User)
                .WithMany(ApplicationUser => ApplicationUser.Services)
                .HasForeignKey(current => current.UserId)
                .WillCascadeOnDelete(false);

            this.HasOptional(current => current.Category)
                .WithMany(ServiceCategory => ServiceCategory.Services)
                .HasForeignKey(current => current.CategoryId)
                .WillCascadeOnDelete(false);

        }
    }
}