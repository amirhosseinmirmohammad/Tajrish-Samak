using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Image
    {
        public Image() { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public long Id { get; set; }

        [DisplayName("آدرس عکس")]
        [Display(Name = "آدرس عکس")]
        [Required(ErrorMessage = "لطفاآدرس عکس را تعیین نمایید .")]
        public string Source { get; set; }

        [DisplayName("عنوان عکس")]
        [Display(Name = "عنوان عکس")]
        [Required(ErrorMessage = "عنوان عکس را تعیین نمایید .")]
        public string Title { get; set; }

        [DisplayName("تگ عکس")]
        [Display(Name = "تگ عکس")]
        [Required(ErrorMessage = "تگ عکس را تعیین نمایید .")]
        public string Alt { get; set; }

        [DisplayName("لینک عکس")]
        [Display(Name = "لینک عکس")]
        [Required(ErrorMessage = "لطفا لینک عکس را تعیین نمایید .")]
        public string Link { get; set; }
        public int? ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int? GalleryId { get; set; }
        public virtual Gallery Gallery { get; set; }
    }
}
