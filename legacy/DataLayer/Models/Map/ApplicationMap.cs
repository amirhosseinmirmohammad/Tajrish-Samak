namespace DataLayer.Models.Mapping
{
    class ApplicationMap
      : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Application>
    {
        public ApplicationMap()
        {
            this.Property(current => current.Id)
                .IsRequired();

            this.Property(current => current.UserName)
                .IsRequired()
                .IsVariableLength();

            this.Property(current => current.Title)
                .IsRequired()
                .IsUnicode()
                .IsVariableLength();

            this.Property(current => current.EmailAddress)
                .IsRequired()
                .IsVariableLength();

            this.Property(current => current.Password)
                .IsRequired()
                .IsVariableLength();

            this.Property(current => current.PortNumber)
                .IsRequired();


            this.Property(current => current.FromNumber)
                .IsRequired();

            this.Property(current => current.SmtpServer)
                .IsRequired();
        }
    }
}
