namespace DataLayer.Models.Mapping
{
    class FileMap
        : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<File>
    {
        public FileMap()
        {
            this.ToTable("Files");
            this.Property(current => current.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(databaseGeneratedOption: System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            this.HasKey(current => current.Id);
            this.Property(current => current.SentEmailId)
                .IsOptional();
            this.Property(current => current.Extension)
                .IsRequired()
                .IsUnicode(true)
                .IsVariableLength()
                .HasMaxLength(30);
            this.Property(current => current.FullPath)
                .IsRequired()
                .IsUnicode(true)
                .IsVariableLength()
                .IsMaxLength();
            this.Property(current => current.FileName)
                .IsRequired()
                .IsUnicode(true)
                .IsVariableLength()
                .HasMaxLength(250);
            this.Property(current => current.FileNameWithoutExtension)
                .IsRequired()
                .IsUnicode(true)
                .IsVariableLength()
                .HasMaxLength(200);
            this.Property(current => current.Directory)
                .IsRequired()
                .IsUnicode(true)
                .IsVariableLength()
                .IsMaxLength();
            this.Property(current => current.FileSize)
                .IsRequired();
            this.Property(current => current.UploadDate)
                .IsRequired();

            // Table Relations

            this.HasOptional(current => current.blog)
                .WithMany(Blog => Blog.Images)
                .HasForeignKey(current => current.BlogId)
                .WillCascadeOnDelete(true);

            this.HasOptional(current => current.service)
    .WithMany(Service => Service.Images)
    .HasForeignKey(current => current.ServiceId)
    .WillCascadeOnDelete(true);

        }
    }
}
