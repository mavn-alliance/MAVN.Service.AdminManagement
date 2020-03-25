using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lykke.Service.AdminManagement.MsSqlRepositories.Entities
{
    public class AdminUserEntity
    {
        [Key]
        [Required]
        [Column("admin_id")]
        public string AdminUserId { get; set; }

        [Required]
        [Column("email_hash", TypeName = "char(64)")]
        public string EmailHash { get; set; }

        [Column("registered_at")]
        public DateTime RegisteredAt { get; set; }

        [Column("is_disabled")]
        public bool IsDisabled { get; set; }

        [Column("use_custom_permissions")]
        public bool UseCustomPermissions { get; set; }
    }
}
