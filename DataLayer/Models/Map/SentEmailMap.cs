namespace DataLayer.Models.Mapping
{
    class SentEmailMap
        : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SentEmail>
    {
        public SentEmailMap()
        {
            this.ToTable("SentEmails");
            this.Property(current => current.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            this.HasKey(current => current.Id);
            this.Property(current => current.Text)
                .IsRequired()
                .IsUnicode(true)
                .IsVariableLength()
                .IsMaxLength();
            this.Property(current => current.SendDate)
                .IsRequired();
            this.Property(current => current.FromEmail)
                .IsRequired()
                .IsUnicode(true)
                .IsVariableLength()
                .HasMaxLength(350);

            // Table Relations
            this.HasMany(current => current.Recipients)
                .WithMany(ApplicationUser => ApplicationUser.RecievedEmails)
                .Map(current =>
                {
                    current.ToTable("UserEmails");

                    current.MapLeftKey("UserId");
                    current.MapRightKey("SentEmailId");
                });
        }
    }
}
