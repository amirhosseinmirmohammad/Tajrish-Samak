using System;
using System.Collections.Generic;

namespace TajrishSamak.Web.Data.Entities
{
    /// <summary>
    /// Maps to the legacy "AspNetUsers" table created by Microsoft.AspNet.Identity.EntityFramework
    /// (MVC5 era). Custom profile columns (RegistrationDate, UserScore, Credit, IsActive, ...)
    /// live next to the standard Identity columns. ASP.NET Core Identity wiring (login, password
    /// hash upgrade) is handled in the Identity stage - for now this is a plain read entity.
    /// </summary>
    public class ApplicationUser
    {
        public ApplicationUser()
        {
            RecievedEmails = new List<SentEmail>();
            Comments = new List<Comment>();
            Orders = new List<Order>();
            Addresses = new List<Address>();
            Notifications = new List<Notification>();
            Discounts = new List<Discount>();
            Payments = new List<Payment>();
            Favorites = new List<Favorite>();
            Blogs = new List<Blog>();
            ServiceCategories = new List<ServiceCategory>();
            Services = new List<Service>();
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public long UserScore { get; set; }
        public string ProfileImage { get; set; }
        public string AddressLine { get; set; }
        public string AccessCode { get; set; }
        public string IntroCode { get; set; }
        public long Credit { get; set; }
        public bool IsActive { get; set; }
        public string Mobile { get; set; }
        public int? StateId { get; set; }
        public State State { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
        public string VerificationCode { get; set; }
        public string PlayerId { get; set; }

        public ICollection<SentEmail> RecievedEmails { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Discount> Discounts { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<Blog> Blogs { get; set; }
        public ICollection<ServiceCategory> ServiceCategories { get; set; }
        public ICollection<Service> Services { get; set; }
    }
}
