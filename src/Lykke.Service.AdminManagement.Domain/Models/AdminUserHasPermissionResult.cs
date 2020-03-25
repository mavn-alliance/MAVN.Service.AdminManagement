using Lykke.Service.AdminManagement.Domain.Enums;

namespace Lykke.Service.AdminManagement.Domain.Models
{
    public class AdminUserHasPermissionResult
    {
        public bool? HasPermission { set; get; }
        
        public AdminPermissionCheckErrorCodes Error { set; get; }
    }
}