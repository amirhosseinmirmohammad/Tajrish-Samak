namespace DataLayer.Models.Mapping
{
    class ServiceCategoryMap
        : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ServiceCategory>
    {
        public ServiceCategoryMap()
        {
            this.HasKey(current => current.Id);

            this.HasOptional(current => current.Parent)
             .WithMany(ServiceCategory => ServiceCategory.SubCategories)
             .HasForeignKey(current => current.ParentId)
             .WillCascadeOnDelete(false);
        }
    }
}
