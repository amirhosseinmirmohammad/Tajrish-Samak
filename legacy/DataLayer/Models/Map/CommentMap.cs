using System.Data.Entity.ModelConfiguration;

namespace DataLayer.Models.Mapping
{
    public class CommentMap : EntityTypeConfiguration<Comment>
    {
        public CommentMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Text)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("Comment");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Text).HasColumnName("Text");
            this.Property(t => t.DateTime).HasColumnName("DateTime");
            this.Property(t => t.IsApprove).HasColumnName("IsApprove");

            // Relationships

            this.HasOptional(t => t.User)
                .WithMany(t => t.Comments)
                .HasForeignKey(d => d.UserId)
                .WillCascadeOnDelete(false);

            this.HasRequired(t => t.Product)
                .WithMany(Product => Product.Comments)
                .HasForeignKey(d => d.ProductId)
                .WillCascadeOnDelete(false);
        }
    }
}
