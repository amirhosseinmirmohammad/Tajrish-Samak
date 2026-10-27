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
    public class File : System.Object
    {
        public File () { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        [Bindable(false)]
        public int Id { get; set; }

        [DisplayName("سایز فایل")]
        [Display(Name = "سایز فایل")]
        public int FileSize { get; set; }

        [DisplayName("زمان آپلود فایل")]
        [Display(Name = "زمان آپلود فایل")]
        public DateTime UploadDate { get; set; }

        [DisplayName("پسوند فایل")]
        [Display(Name = "پسوند فایل")]
        public string Extension { get; set; }

        [DisplayName("آدرس فایل")]
        [Display(Name = "آدرس فایل")]
        public string FullPath { get; set; }

        [DisplayName("نام فایل همراه پسوند")]
        [Display(Name = "نام فایل همراه پسوند")]
        public string FileName { get; set; }

        [DisplayName("نام فایل بدون پسوند")]
        [Display(Name = "نام فایل بدون پسوند")]
        public string FileNameWithoutExtension { get; set; }

        [DisplayName("محل ذخیره فایل")]
        [Display(Name = "محل ذخیره فایل")]
        public string Directory { get; set; }

        [DisplayName("ارسال ایمیل مربوطه")]
        [Display(Name = "ارسال ایمیل مربوطه")]
        public int? SentEmailId { get; set; }
        public virtual SentEmail SentEmail { get; set; }
        public int? orderId { get; set; }
        public virtual Order order { get; set; }
        public int? BlogId { get; set; }
        public virtual Blog blog { get; set; }
        public int? ServiceId { get; set; }
        public virtual Service service { get; set; }
    }
}
