namespace MAVN.Service.AdminManagement.Domain.Enums
{
    public enum ServicesError
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
