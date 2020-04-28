namespace MAVN.Service.AdminManagement.Domain.Enums
{
    public enum ServicesError
    {
        None = 0,

        LoginNotFound,

        AdminEmailIsNotVerified,

        PasswordMismatch,

        RegisteredWithAnotherPassword,

        AlreadyRegistered,

        InvalidEmailOrPasswordFormat,
        
        AdminNotActive
    }
}
