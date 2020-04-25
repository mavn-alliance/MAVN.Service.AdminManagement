using System.Collections.Generic;
using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public IReadOnlyList<Permission> Permissions { get; set; }
        public Localization Localization { get; set; }
    }
}
