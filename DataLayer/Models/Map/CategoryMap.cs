namespace DataLayer.Models.Mapping
{
    class CategoryMap
        : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            this.HasKey(current => current.Id);

            this.HasOptional(current => current.Parent)
             .WithMany(Category => Category.SubCategories)
             .HasForeignKey(current => current.ParentId)
             .WillCascadeOnDelete(false);

        }
    }
}
