using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Mapping
{
    public class BlogCommentMap : EntityTypeConfiguration<BlogComment>
    {
        public BlogCommentMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Text)
                .IsRequired()
                .HasMaxLength(500);

            // Relationships

            this.HasRequired(t => t.Blog)
                .WithMany(Blog => Blog.BlogComments)
                .HasForeignKey(d => d.BlogId)
                .WillCascadeOnDelete(false);
        }
    }
}
