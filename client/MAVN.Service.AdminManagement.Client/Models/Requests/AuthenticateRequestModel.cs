using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Authenticate request model.
    /// </summary>
    [PublicAPI]
    public class AuthenticateRequestModel
    {
        /// <summary>Email.</summary>
        [Required]
        public string Email { get; set; }

        /// <summary>Password.</summary>
        [Required]
        public string Password { get; set; }
    }
}
