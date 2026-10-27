using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajrishSamak.Web.Data.Entities
{
    public class Favorite
    {
        public Favorite()
        {
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [Display(Name = "نام کاربر")]
        [DisplayName("نام کاربر")]
        public string UserId { get; set; }
        public virtual ApplicationUser Users { get; set; }

        [Display(Name = "نام کاربر")]
        [DisplayName("نام کاربر")]
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public DateTime? Date { get; set; }

    }
}
