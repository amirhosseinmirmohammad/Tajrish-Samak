using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DataLayer.Models;
using DataLayer.Models.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace GladcherryShopping.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        [DisplayName("نام")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [DisplayName("نام خانوادگی")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [DisplayName("جنسیت")]
        [Display(Name = "جنسیت")]
        public byte? Gender { get; set; }

        [DisplayName("تاریخ تولد")]
        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }

        [DisplayName("تاریخ ثبت نام")]
        [Display(Name = "تاریخ  ثبت نام")]
        public DateTime RegistrationDate { get; set; }

        [DisplayName("امتیاز کاربر")]
        [Display(Name = "امتیاز کاربر")]
        public long UserScore { get; set; }

        [DisplayName("تصویر پروفایل")]
        [Display(Name = "تصویر پروفایل")]
        public string ProfileImage { get; set; }

        [DisplayName("آدرس کاربر")]
        [Display(Name = "آدرس کاربر")]
        public string AddressLine { get; set; }

        [DisplayName("کد دسترسی برای دوستان")]
        public string AccessCode { get; set; }

        [DisplayName("کد دعوت دوست")]
        public string IntroCode { get; set; }

        [DisplayName("اعتبار")]
        [Display(Name = "اعتبار")]
        public long Credit { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public string getGender
        {
            get
            {
                switch (Gender)
                {
                    case 1:
                        return "مرد";
                    case 2:
                        return "زن";
                    default:
                        return "سایر";
                }
            }
        }

        [DisplayName("موبایل")]
        [Display(Name = "موبایل")]
        [RegularExpression(@"\b\d{4}[\s-.]?\d{3}[\s-.]?\d{4}\b", ErrorMessage = "فرمت تلفن همراه وارد شده صحیح نیست.")]
        public string Mobile { get; set; }
        public int? StateId { get; set; }
        public virtual State State { get; set; }
        public int? CityId { get; set; }
        public virtual City City { get; set; }

        [DisplayName("کد اعتبارسنجی")]
        [Display(Name = "کد اعتبارسنجی")]
        public string VerificationCode { get; set; }
        public virtual ICollection<SentEmail> RecievedEmails { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Discount> Discounts { get; set; }

        [DisplayName("لیست گردش مالی")]
        [Display(Name = "لیست گردش مالی")]
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }

        [DisplayName("شماره کاربری در وان سیگنال")]
        [Display(Name = "شماره کاربری در وان سیگنال")]
        public string PlayerId { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<ServiceCategory> ServiceCategories { get; set; }
        public virtual ICollection<Service> Services { get; set; }


    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("TajrishSamakStore", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SentEmail> SentEmails { get; set; }
        public DbSet<SiteMessage> SiteMessages { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInOrder> ProductInOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UserTransactions> UserTransactions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<ProductInBasket> ProductInBaskets { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Service> Services { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ApplicationMap());
            modelBuilder.Configurations.Add(new CityMap());
            modelBuilder.Configurations.Add(new StateMap());
            modelBuilder.Configurations.Add(new NotificationMap());
            modelBuilder.Configurations.Add(new ImageMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new CommentMap());
            modelBuilder.Configurations.Add(new TransactionMap());
            modelBuilder.Configurations.Add(new SiteMessageMap());
            modelBuilder.Configurations.Add(new SentEmailMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProductInOrderMap());
            modelBuilder.Configurations.Add(new DiscountMap());
            modelBuilder.Configurations.Add(new Address.configuration());
            modelBuilder.Configurations.Add(new NewsLetter.configuration());
            modelBuilder.Configurations.Add(new SliderMap());
            modelBuilder.Configurations.Add(new GalleryMap());
            modelBuilder.Configurations.Add(new ProductInBasketMap());
            modelBuilder.Configurations.Add(new PaymentMap());
            modelBuilder.Configurations.Add(new Favorite.configuration());
            modelBuilder.Configurations.Add(new BlogMap());
            modelBuilder.Configurations.Add(new BlogCommentMap());
            modelBuilder.Configurations.Add(new FileMap());
            modelBuilder.Configurations.Add(new BrandMap());
            modelBuilder.Configurations.Add(new ServiceCategoryMap());
            modelBuilder.Configurations.Add(new ServiceMap());

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}