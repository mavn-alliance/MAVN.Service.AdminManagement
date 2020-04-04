namespace MAVN.Service.AdminManagement.Client.Models.Enums
{
    /// <summary>
    /// Represents status result of password reset attempt
    /// </summary>
    public enum ResetPasswordErrorCodes
    {
        /// <summary>
        /// No error, password reset
        /// </summary>
        None,
        /// <summary>
        /// Admin not found
        /// </summary>
        AdminUserNotFound,
        /// <summary>
        /// Invalid password provided
        /// </summary>
        InvalidPassword
    }
}