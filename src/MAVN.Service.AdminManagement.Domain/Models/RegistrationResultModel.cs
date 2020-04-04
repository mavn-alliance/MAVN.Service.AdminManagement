using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    public class RegistrationResultModel
    {
        public AdminUser Admin { get; set; }

        public ServicesError Error { get; set; }
    }
}