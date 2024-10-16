namespace DataLayer.Models.Mapping
{
    class BrandMap
        : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Brand>
    {
        public BrandMap()
        {
            this.HasKey(current => current.Id);

            this.HasOptional(current => current.Parent)
             .WithMany(Brand => Brand.SubCategories)
             .HasForeignKey(current => current.ParentId)
             .WillCascadeOnDelete(false);

        }
    }
}
