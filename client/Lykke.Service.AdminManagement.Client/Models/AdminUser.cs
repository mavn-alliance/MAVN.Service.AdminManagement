using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Represents the Admin User profile
    /// </summary>
    [PublicAPI]
    public class AdminUser
    {
        /// <summary>
        /// Internal Admin Id
        /// </summary>
        public string AdminUserId { get; set; }
        /// <summary>
        /// Is Admin active or not
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Email address of the Admin
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Registration Date
        /// </summary>
        public DateTime RegisteredAt { get; set; }
        /// <summary>
        /// Phone number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Company
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// Department
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// Job Title
        /// </summary>
        public string JobTitle { get; set; }
        /// <summary>
        /// Back Office Permissions
        /// </summary>
        public IReadOnlyList<AdminPermission> Permissions { get; set; }
    }
}
