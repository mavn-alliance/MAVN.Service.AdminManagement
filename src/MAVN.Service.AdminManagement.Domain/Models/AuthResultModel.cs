using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    public class AuthResultModel
    {
        public string CustomerId { get; set; }

        public string Token { get; set; }

        public ServicesError Error { get; set; }
    }
}
