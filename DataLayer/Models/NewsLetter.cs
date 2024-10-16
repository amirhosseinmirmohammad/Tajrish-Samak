using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DataLayer.Models
{
    public class NewsLetter
    {
        public NewsLetter()
        {
            Notifications = new List<Notification>();
        }

        internal class configuration : EntityTypeConfiguration<NewsLetter>
        {
            public configuration()
            {
                this.Property(current => current.Email)
                    .IsRequired();
            }
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("ایمیل")]
        [Required(ErrorMessage = "لطفا ایمیل را وارد نمایید .")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "لطفا ایمیل معتبری را وارد نمایید")]
        public string Email { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}