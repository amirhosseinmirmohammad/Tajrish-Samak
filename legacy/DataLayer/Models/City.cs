using GladcherryShopping.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class City
    {
        public City()
        {
            Users = new List<ApplicationUser>();
        }

        [ScaffoldColumn(false)]
        [Bindable(false)]
        [Key]
        [Required]
        [DatabaseGenerated
        (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("نام شهر")]
        [Required(ErrorMessage = "لطفا نام شهر را وارد کنید")]
        [MaxLength(100, ErrorMessage = "نام شهر حدااکثر می تواند 100 کاراکتر داشته باشد")]
        public string Name { get; set; }

        [DisplayName("نام استان")]
        [Required(ErrorMessage = "لطفا استان مربوطه را انتخاب کنید")]
        public int StateId { get; set; }
        public virtual State State { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}