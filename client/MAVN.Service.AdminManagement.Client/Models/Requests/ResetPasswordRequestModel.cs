namespace MAVN.Service.AdminManagement.Client.Models.Requests
{
    /// <summary>
    /// Represents request to change password
    /// </summary>
    public class ResetPasswordRequestModel
    {
        /// <summary>
        /// Admin Id
        /// </summary>
        public string AdminUserId { set; get; }
        /// <summary>
        /// New password
        /// </summary>
        public string Password { set; get; }
    }
}