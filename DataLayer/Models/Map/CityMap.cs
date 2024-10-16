using System.Data.Entity.ModelConfiguration;

namespace DataLayer.Models.Mapping
{
    public class CityMap : EntityTypeConfiguration<City>
    {
        public CityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("City");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.StateId).HasColumnName("StateId");

            // Relationships
            this.HasRequired(t => t.State)
                .WithMany(t => t.Cities)
                .HasForeignKey(d => d.StateId)
                .WillCascadeOnDelete(false);           
        }
    }
}