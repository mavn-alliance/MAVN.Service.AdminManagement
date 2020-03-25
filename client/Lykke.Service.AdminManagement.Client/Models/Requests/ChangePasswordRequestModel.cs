using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Lykke.Service.AdminManagement.Client.Models.Requests
{
    /// <summary>
    /// Used to update admins passwords
    /// </summary>
    [PublicAPI]
    public class ChangePasswordRequestModel
    {
        /// <summary>Email.</summary>
        [Required]
        public string Email { get; set; }

        /// <summary>Current password.</summary>
        [Required]
        public string CurrentPassword { get; set; }

        /// <summary>New password.</summary>
        [Required]
        public string NewPassword { get; set; }
    }
}