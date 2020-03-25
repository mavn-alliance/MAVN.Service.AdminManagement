using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lykke.Service.AdminManagement.Domain.Enums;

namespace Lykke.Service.AdminManagement.MsSqlRepositories.Entities
{
    [Table("permissions")]
    public class PermissionEntity
    {
        [Key]
        [Required]
        [Column("id")]
        public string Id { get; set; }
        
        [Required]
        [Column("admin_id")]
        public string AdminUserId { get; set; }
        
        [Column("type")]
        [Required]
        public string Type { set; get; }
        
        [Column("level")]
        [Required]
        public PermissionLevel Level { set; get; }
    }
}