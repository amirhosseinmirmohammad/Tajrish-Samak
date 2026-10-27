namespace DataLayer.Models.Mapping
{
    public class BlogMap : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Blog>
    {
        public BlogMap()
        {

            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.UserId).HasColumnName("UserId");

            this.HasRequired(current => current.User)
                .WithMany(ApplicationUser => ApplicationUser.Blogs)
                .HasForeignKey(current => current.UserId)
                .WillCascadeOnDelete(false);

            this.HasOptional(current => current.Category)
                .WithMany(Category => Category.Blogs)
                .HasForeignKey(current => current.CategoryId)
                .WillCascadeOnDelete(false);

        }
    }
}