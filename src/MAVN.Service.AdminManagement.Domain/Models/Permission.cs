using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    public class Permission
    {
        public string Type { set; get; }
        public PermissionLevel Level { set; get; }
    }
}