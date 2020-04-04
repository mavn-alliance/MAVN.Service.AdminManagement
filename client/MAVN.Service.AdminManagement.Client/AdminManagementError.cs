using JetBrains.Annotations;

namespace MAVN.Service.AdminManagement.Client
{
    /// <summary>
    /// Enum for admin management errors.
    /// </summary>
    [PublicAPI]
    public enum AdminManagementError
    {
        None = 0,
        LoginNotFound,
        PasswordMismatch,
        RegisteredWithAnotherPassword,
        AlreadyRegistered,
        InvalidEmailOrPasswordFormat,
        AdminNotActive
    }
}
