using System;
using Lykke.Service.AdminManagement.Client.Models.Enums;

namespace Lykke.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Admin users response model used in the getting of the admin users call
    /// </summary>
    public class AdminUserResponseModel
    {
        /// <summary>
        /// The Admin User Profile
        /// </summary>
        public AdminUser Profile { get; set; }
        /// <summary>
        /// Holds any Error that might have happened during the execution
        /// </summary>
        public AdminUserResponseErrorCodes Error { get; set; }
    }
}
