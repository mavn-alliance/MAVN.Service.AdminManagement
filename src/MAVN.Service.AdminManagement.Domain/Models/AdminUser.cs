using System;
using System.Collections.Generic;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    /// <summary>
    /// Represents an admin user profile.
    /// </summary>
    public class AdminUser
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public string AdminUserId { get; set; }

        /// <summary>
        /// The email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Whether or not admin is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The registration date and time.
        /// </summary>
        public DateTime RegisteredAt { get; set; }
        
        /// <summary>
        /// Phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Company.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Department.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Job Title.
        /// </summary>
        public string JobTitle { get; set; }
        
        /// <summary>
        /// Back Office permissions.
        /// </summary>
        public List<Permission> Permissions { set; get; }
        
        /// <summary>
        /// Whether or not default permissions list should be used. Needed for backwards compatibility.
        /// </summary>
        public bool UseDefaultPermissions { set; get; }
    }
}
