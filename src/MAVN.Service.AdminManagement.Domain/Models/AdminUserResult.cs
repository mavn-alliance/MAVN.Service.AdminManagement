using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    public class AdminUserResult
    {
        public AdminUser Profile { get; set; }

        public AdminUserErrorCodes Error { get; set; }
    }
}
