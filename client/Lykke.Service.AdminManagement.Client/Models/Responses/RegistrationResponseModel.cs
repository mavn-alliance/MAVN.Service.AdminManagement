using JetBrains.Annotations;

namespace Lykke.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Registration response model.
    /// </summary>
    [PublicAPI]
    public class RegistrationResponseModel
    {
        /// <summary>Admin user.</summary>
        public AdminUser Admin { get; set; }

        /// <summary>Error</summary>
        public AdminManagementError Error { get; set; }
    }
}
