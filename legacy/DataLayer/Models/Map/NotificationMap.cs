namespace DataLayer.Models.Mapping
{
    class NotificationMap
        :System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Notification>
    {
        public NotificationMap()
        {
            this.Property(current => current.Id)
                .IsRequired();

            this.Property(current => current.Text)
                .IsRequired()
                .IsUnicode(true)
                .IsVariableLength();

            this.HasMany(current => current.Users)
                .WithMany(User => User.Notifications)
                .Map(c =>
                {
                    c.MapLeftKey("NotificationId");
                    c.MapRightKey("UserId");
                    c.ToTable("UserInNotifications");
                });
        }
    }
}
