namespace MAVN.Service.AdminManagement.Client.Models.Enums
{
    /// <summary>
    /// Error codes after updating password.
    /// </summary>
    public enum ChangePasswordErrorCodes
    {
        /// <summary>
        /// No error.
        /// </summary>
        None,

        /// <summary>
        /// Login not found.
        /// </summary>
        LoginNotFound,

        /// <summary>
        /// Password doesn't match the correct value.
        /// </summary>
        PasswordMismatch,

        /// <summary>
        /// Email or password doesn't match.
        /// </summary>
        InvalidEmailOrPasswordFormat,
        
        /// <summary>
        /// Provided new password is invalid.
        /// </summary>
        NewPasswordInvalid,
        
        /// <summary>
        /// Admin is not active.
        /// </summary>
        AdminNotActive
    }
}