using System.Data.Entity.ModelConfiguration;

namespace DataLayer.Models.Mapping
{
    public class SiteMessageMap : EntityTypeConfiguration<SiteMessage>
    {
        public SiteMessageMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.FullName)
                .IsRequired();

            this.Property(t => t.Body)
            .IsRequired();
        }
    }
}
