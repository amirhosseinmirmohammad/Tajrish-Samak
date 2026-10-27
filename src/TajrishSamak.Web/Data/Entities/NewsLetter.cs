using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TajrishSamak.Web.Data.Entities
{
    public class NewsLetter
    {
        public NewsLetter()
        {
            Notifications = new List<Notification>();
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
