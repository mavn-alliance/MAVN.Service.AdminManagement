using Lykke.Service.AdminManagement.Domain.Enums;

namespace Lykke.Service.AdminManagement.Domain.Models
{
    public class AdminPasswordResetResult
    {
        public AdminUser Profile { get; set; }

        public AdminPasswordResetErrorCode Error { get; set; }
    }
}