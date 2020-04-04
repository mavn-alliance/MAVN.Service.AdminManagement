using MAVN.Service.AdminManagement.Client.Models.Enums;

namespace MAVN.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Represents a resopnse to check whether or not an admin has a permission.
    /// </summary>
    public class AdminUserPermissionCheckResponseModel
    {
        /// <summary>
        /// Holds any Error that might have happened during the execution
        /// </summary>
        public AdminPermissionCheckErrorCodes Error { get; set; }
        /// <summary>
        /// Whether or not admin has the permission.
        /// </summary>
        public bool? HasPermission { set; get; }
    }
}