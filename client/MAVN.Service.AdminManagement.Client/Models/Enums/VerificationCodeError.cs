using JetBrains.Annotations;

namespace MAVN.Service.AdminManagement.Client.Models.Enums
{
    /// <summary>
    /// Enum for verification code errors.
    /// </summary>
    [PublicAPI]
    public enum VerificationCodeError
    {
        /// <summary>ErrorCode: None</summary>
        None,
        /// <summary>ErrorCode: AlreadyVerified</summary>
        AlreadyVerified,
        /// <summary>ErrorCode: Verification code does not exist</summary>
        VerificationCodeDoesNotExist,
        /// <summary>ErrorCode: VerificationCodeMismatch</summary>
        VerificationCodeMismatch,
        /// <summary>ErrorCode: VerificationCodeExpired</summary>
        VerificationCodeExpired,
        /// <summary>Admin does not exist</summary>
        AdminDoesNotExist,
    }
}
