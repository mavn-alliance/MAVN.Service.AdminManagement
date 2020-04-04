using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    public class AdminUserHasPermissionResult
    {
        public bool? HasPermission { set; get; }
        
        public AdminPermissionCheckErrorCodes Error { set; get; }
    }
}