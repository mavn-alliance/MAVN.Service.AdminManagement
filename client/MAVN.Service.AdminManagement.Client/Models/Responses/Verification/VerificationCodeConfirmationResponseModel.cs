using JetBrains.Annotations;
using MAVN.Service.AdminManagement.Client.Models.Enums;

namespace MAVN.Service.AdminManagement.Client.Models.Responses.Verification
{
    /// <summary>
    /// ConfirmEmail response model.
    /// </summary>
    [PublicAPI]
    public class VerificationCodeConfirmationResponseModel
    {
        /// <summary>Is verified</summary>
        public bool IsVerified { get; set; }

        /// <summary>Error</summary>
        public VerificationCodeError Error { get; set; }
    }
}
