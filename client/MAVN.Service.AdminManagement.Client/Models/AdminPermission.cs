using MAVN.Service.AdminManagement.Client.Models.Enums;

namespace MAVN.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Represents Admin Permission.
    /// </summary>
    public class AdminPermission
    {
        /// <summary>
        /// Type of admin permission.
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// Level of admin permission.
        /// </summary>
        public AdminPermissionLevel Level { set; get; }
    }
}