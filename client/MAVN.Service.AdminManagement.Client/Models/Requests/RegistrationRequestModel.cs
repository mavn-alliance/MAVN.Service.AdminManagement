using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using MAVN.Service.AdminManagement.Client.Models.Enums;

namespace MAVN.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Registration request model.
    /// </summary>
    [PublicAPI]
    public class RegistrationRequestModel
    {
        /// <summary>Email</summary>
        [Required]
        public string Email { get; set; }

        /// <summary>Password</summary>
        [Required]
        public string Password { get; set; }

        /// <summary>First Name</summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>Last Name</summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>Phone number</summary>
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>Company</summary>
        [Required]
        public string Company { get; set; }

        /// <summary>Department</summary>
        [Required]
        public string Department { get; set; }

        /// <summary>Job Title</summary>
        [Required]
        public string JobTitle { get; set; }

        /// <summary>Back Office Permissions</summary>
        [Required]
        public IReadOnlyList<AdminPermission> Permissions { get; set; }

        /// <summary>Localization</summary>
        public Localization Localization { get; set; }
    }
}
