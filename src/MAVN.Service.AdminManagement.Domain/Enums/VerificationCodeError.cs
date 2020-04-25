namespace MAVN.Service.AdminManagement.Domain.Enums
{
    public enum VerificationCodeError
    {
        None,
        AlreadyVerified,
        VerificationCodeDoesNotExist,
        VerificationCodeMismatch,
        VerificationCodeExpired,
        AdminDoesNotExist,
    }
}
