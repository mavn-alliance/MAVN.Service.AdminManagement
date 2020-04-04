using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    public class AdminPasswordResetResult
    {
        public AdminUser Profile { get; set; }

        public AdminPasswordResetErrorCode Error { get; set; }
    }
}