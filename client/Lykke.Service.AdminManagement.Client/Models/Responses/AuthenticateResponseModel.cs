using JetBrains.Annotations;

namespace Lykke.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Authenticate response model.
    /// </summary>
    [PublicAPI]
    public class AuthenticateResponseModel
    {
        /// <summary>CustomerId</summary>
        public string CustomerId { get; set; }

        /// <summary>Token</summary>
        public string Token { get; set; }

        /// <summary>Error</summary>
        public AdminManagementError Error { get; set; }
    }
}
