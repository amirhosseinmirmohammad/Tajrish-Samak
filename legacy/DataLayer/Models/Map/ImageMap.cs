namespace DataLayer.Models.Mapping
{
    class ImageMap
       : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Image>
    {
        public ImageMap()
        {
            this.Property(current => current.Id)
                .IsRequired();

            this.Property(current => current.Alt)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode()
                .IsVariableLength();

                this.Property(current => current.Link)
                .IsRequired()
                .IsVariableLength();

            this.Property(current => current.Source)
                .IsRequired()
                .IsVariableLength();

            this.Property(current => current.Title)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode()
                .IsVariableLength();

        }
    }
}
