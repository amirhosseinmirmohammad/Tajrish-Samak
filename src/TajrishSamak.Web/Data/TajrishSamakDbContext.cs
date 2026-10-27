using TajrishSamak.Web.Data.Entities;
using FileEntity = TajrishSamak.Web.Data.Entities.File;
using Microsoft.EntityFrameworkCore;

namespace TajrishSamak.Web.Data
{
    /// <summary>
    /// Maps to the EXISTING TajrishSamak production database (schema created by the
    /// legacy EF6 model). No migrations - this context is read/write compatible with
    /// the current schema so the legacy site and this app can run side by side.
    /// </summary>
    public class TajrishSamakDbContext : DbContext
    {
        public TajrishSamakDbContext(DbContextOptions<TajrishSamakDbContext> options)
            : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInBasket> ProductInBaskets { get; set; }
        public DbSet<ProductInOrder> ProductInOrders { get; set; }
        public DbSet<SentEmail> SentEmails { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<SiteMessage> SiteMessages { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<UserTransactions> UserTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // LEGACY COMPATIBILITY: the production database has NULLs in many string
            // columns (images, optional texts) while the ported model properties are
            // plain non-annotated strings (pre-NRT style). EF Core 10 reads string
            // columns with a strict reader for required properties, so every string
            // property is explicitly marked nullable here - matching the legacy EF6
            // behaviour where any string column could hold NULL.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && !property.IsKey() && !property.IsForeignKey())
                    {
                        property.IsNullable = true;
                    }
                }
            }

            // ---------- table names that differ from the DbSet names ----------
            modelBuilder.Entity<City>().ToTable("City");
            modelBuilder.Entity<State>().ToTable("State");
            modelBuilder.Entity<Comment>().ToTable("Comment");
            modelBuilder.Entity<FileEntity>().ToTable("Files");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<ProductInBasket>().ToTable("ProductInBasket");
            modelBuilder.Entity<ProductInOrder>().ToTable("ProductsInOrders");
            modelBuilder.Entity<SentEmail>().ToTable("SentEmails");
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");

            // ---------- file (attachment) relationships ----------
            modelBuilder.Entity<FileEntity>().HasOne(f => f.blog).WithMany(b => b.Images).HasForeignKey(f => f.BlogId);
            modelBuilder.Entity<FileEntity>().HasOne(f => f.service).WithMany(s => s.Images).HasForeignKey(f => f.ServiceId);
            modelBuilder.Entity<FileEntity>().HasOne(f => f.SentEmail).WithMany().HasForeignKey(f => f.SentEmailId);
            modelBuilder.Entity<FileEntity>().HasOne(f => f.order).WithMany().HasForeignKey(f => f.orderId);

            // ---------- content ----------
            modelBuilder.Entity<Blog>().HasOne(b => b.User).WithMany(u => u.Blogs).HasForeignKey(b => b.UserId);
            modelBuilder.Entity<Blog>().HasOne(b => b.Category).WithMany(c => c.Blogs).HasForeignKey(b => b.CategoryId);
            modelBuilder.Entity<BlogComment>().HasOne(c => c.Blog).WithMany(b => b.BlogComments).HasForeignKey(c => c.BlogId);
            modelBuilder.Entity<Service>().HasOne(s => s.User).WithMany(u => u.Services).HasForeignKey(s => s.UserId);
            modelBuilder.Entity<Service>().HasOne(s => s.Category).WithMany(c => c.Services).HasForeignKey(s => s.CategoryId);

            // ---------- catalog ----------
            modelBuilder.Entity<Product>().HasOne(p => p.category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
            modelBuilder.Entity<Product>().HasOne(p => p.Brand).WithMany(b => b.Products).HasForeignKey(p => p.BrandId);
            // self-referencing "related products" kept in the legacy Product_Id column
            modelBuilder.Entity<Product>().HasMany(p => p.RelatedProducts).WithOne().HasForeignKey("Product_Id");

            // legacy quirk: Image.Product points at the bigint Product_Id column,
            // while the int ProductId column is an unrelated legacy scalar
            modelBuilder.Entity<Image>().HasOne(i => i.Product).WithMany().HasForeignKey("Product_Id");

            modelBuilder.Entity<Image>().HasOne(i => i.Gallery).WithMany(g => g.Images).HasForeignKey(i => i.GalleryId);

            // ---------- hierarchies ----------
            modelBuilder.Entity<Brand>().HasOne(b => b.Parent).WithMany(b => b.SubCategories).HasForeignKey(b => b.ParentId);
            modelBuilder.Entity<Category>().HasOne(c => c.Parent).WithMany(c => c.SubCategories).HasForeignKey(c => c.ParentId);
            modelBuilder.Entity<ServiceCategory>().HasOne(c => c.Parent).WithMany(c => c.SubCategories).HasForeignKey(c => c.ParentId);

            // ---------- commerce ----------
            modelBuilder.Entity<Basket>().HasMany(b => b.ProductInBaskets).WithOne(p => p.Basket).HasForeignKey(p => p.BasketId);
            modelBuilder.Entity<Product>().HasMany(p => p.ProductInBaskets).WithOne(p => p.Product).HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<Order>().HasMany(o => o.Products).WithOne(p => p.Order).HasForeignKey(p => p.OrderId);
            modelBuilder.Entity<Product>().HasMany(p => p.Orders).WithOne(p => p.Product).HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<Order>().HasMany(o => o.Transactions).WithOne(t => t.Order).HasForeignKey(t => t.OrderId);
            modelBuilder.Entity<Transaction>().HasMany(t => t.Payments).WithOne(p => p.Transaction).HasForeignKey(p => p.TransactionId);
            modelBuilder.Entity<Payment>().HasOne(p => p.User).WithMany(u => u.Payments).HasForeignKey(p => p.UserId);
            modelBuilder.Entity<Payment>().HasOne(p => p.Order).WithMany(o => o.Payments).HasForeignKey(p => p.OrderId);
            modelBuilder.Entity<Favorite>().HasOne(f => f.Users).WithMany(u => u.Favorites).HasForeignKey(f => f.UserId);
            modelBuilder.Entity<Favorite>().HasOne(f => f.Product).WithMany(p => p.Favorites).HasForeignKey(f => f.ProductId);
            modelBuilder.Entity<Comment>().HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey(c => c.UserId);
            modelBuilder.Entity<Comment>().HasOne(c => c.Product).WithMany(p => p.Comments).HasForeignKey(c => c.ProductId);
            modelBuilder.Entity<Order>().HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId);
            modelBuilder.Entity<Order>().HasOne(o => o.Address).WithMany(a => a.Orders).HasForeignKey(o => o.AddressId);

            // ---------- user / location ----------
            modelBuilder.Entity<Address>().HasOne(a => a.Users).WithMany().HasForeignKey(a => a.UserId);
            modelBuilder.Entity<ApplicationUser>().HasOne(u => u.State).WithMany(s => s.Users).HasForeignKey(u => u.StateId);
            modelBuilder.Entity<ApplicationUser>().HasOne(u => u.City).WithMany(c => c.Users).HasForeignKey(u => u.CityId);

            // ---------- legacy many-to-many join tables ----------
            modelBuilder.Entity<Discount>()
                .HasMany(d => d.Users)
                .WithMany(u => u.Discounts)
                .UsingEntity<Dictionary<string, object>>(
        "UserDiscounts",
        j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
        j => j.HasOne<Discount>().WithMany().HasForeignKey("DiscountId"),
        j => j.HasKey("UserId", "DiscountId"));

            modelBuilder.Entity<SentEmail>()
                .HasMany(s => s.Recipients)
                .WithMany(u => u.RecievedEmails)
                .UsingEntity<Dictionary<string, object>>(
        "UserEmails",
        j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
        j => j.HasOne<SentEmail>().WithMany().HasForeignKey("SentEmailId"),
        j => j.HasKey("UserId", "SentEmailId"));

            modelBuilder.Entity<Notification>()
                .HasMany(n => n.Users)
                .WithMany(u => u.Notifications)
                .UsingEntity<Dictionary<string, object>>(
        "UserInNotifications",
        j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
        j => j.HasOne<Notification>().WithMany().HasForeignKey("NotificationId"),
        j => j.HasKey("UserId", "NotificationId"));

            modelBuilder.Entity<NewsLetter>()
                .HasMany(n => n.Notifications)
                .WithMany(n => n.NewsLetters)
                .UsingEntity<Dictionary<string, object>>(
        "NewsLetterNotifications",
        j => j.HasOne<Notification>().WithMany().HasForeignKey("Notification_Id"),
        j => j.HasOne<NewsLetter>().WithMany().HasForeignKey("NewsLetter_Id"),
        j => j.HasKey("Notification_Id", "NewsLetter_Id"));

            // Notification.Images was never exposed on the Image entity in the legacy
            // model; the legacy column stays untouched and unmapped.
            modelBuilder.Entity<Notification>().Ignore(n => n.Images);
        }
    }
}
