using Lykke.Service.AdminManagement.Client.Models.Enums;

namespace Lykke.Service.AdminManagement.Client.Models.Requests
{
    /// <summary>
    /// Represents a request to check whether or not an admin has a permission.
    /// </summary>
    public class GetAdminByIdRequestModel
    {
        /// <summary>
        /// Id of the admin.
        /// </summary>
        public string AdminUserId { set; get; }
    }
}