using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TajrishSamak.Web.Data.Entities
{
    /// <summary>Maps to the legacy AspNetRoles table (Identity 2).</summary>
    [Table("AspNetRoles")]
    public class IdentityRoleEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>Maps to the legacy AspNetUserRoles join table (Identity 2).</summary>
    [Table("AspNetUserRoles")]
    public class IdentityUserRoleEntity
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
