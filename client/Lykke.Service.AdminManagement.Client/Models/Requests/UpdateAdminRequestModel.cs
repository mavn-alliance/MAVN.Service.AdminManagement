using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.AdminManagement.Client.Models.Requests
{
    /// <summary>
    /// Represents a request to update an admin profile.
    /// </summary>
    public class UpdateAdminRequestModel
    {
        /// <summary>Agent Id</summary>
        [Required]
        public string AdminUserId { get; set; }
        
        /// <summary>Is agent active or not</summary>
        [Required]
        public bool IsActive { get; set; }
        
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
    }
}