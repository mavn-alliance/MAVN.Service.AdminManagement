using System;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    /// <summary>
    /// Represents an encrypted admin user profile.
    /// </summary>
    public class AdminUserEncrypted
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public string AdminUserId { get; set; }

        /// <summary>
        /// The SHA256 hash of email address.
        /// </summary>
        public string EmailHash { get; set; }

        /// <summary>
        /// The registration date and time.
        /// </summary>
        public DateTime RegisteredAt { get; set; }

        /// <summary>
        /// Is admin active or not.
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Whether or not default permissions list should be used. Needed for backwards compatibility.
        /// </summary>
        public bool UseDefaultPermissions { set; get; }
    }
}
