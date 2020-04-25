using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Models.Emails
{
    public class AdminCreatedEmailDto
    {
        public string AdminUserId { get; set; }
        public string Email { get; set; }
        public string EmailVerificationCode { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public Localization Localization { get; set; }
    }
}
