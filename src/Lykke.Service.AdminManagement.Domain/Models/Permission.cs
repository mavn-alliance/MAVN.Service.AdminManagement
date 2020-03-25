using Lykke.Service.AdminManagement.Domain.Enums;

namespace Lykke.Service.AdminManagement.Domain.Models
{
    public class Permission
    {
        public string Type { set; get; }
        public PermissionLevel Level { set; get; }
    }
}