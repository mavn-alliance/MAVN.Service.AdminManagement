using MAVN.Service.AdminManagement.Client.Models.Enums;

namespace MAVN.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Represents result of change password attempt
    /// </summary>
    public class ResetPasswordResponseModel
    {
        /// <summary>
        /// The Admin User Profile
        /// </summary>
        public AdminUser Profile { get; set; }
        /// <summary>
        /// Holds any Error that might have happened during the execution
        /// </summary>
        public ResetPasswordErrorCodes Error { get; set; }
    }
}