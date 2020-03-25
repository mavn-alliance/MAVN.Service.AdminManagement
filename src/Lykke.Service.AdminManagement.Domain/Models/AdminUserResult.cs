using Lykke.Service.AdminManagement.Domain.Enums;

namespace Lykke.Service.AdminManagement.Domain.Models
{
    public class AdminUserResult
    {
        public AdminUser Profile { get; set; }

        public AdminUserErrorCodes Error { get; set; }
    }
}
