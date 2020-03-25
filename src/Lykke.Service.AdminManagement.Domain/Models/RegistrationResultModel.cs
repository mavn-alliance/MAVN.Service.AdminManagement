using Lykke.Service.AdminManagement.Domain.Enums;

namespace Lykke.Service.AdminManagement.Domain.Models
{
    public class RegistrationResultModel
    {
        public AdminUser Admin { get; set; }

        public ServicesError Error { get; set; }
    }
}