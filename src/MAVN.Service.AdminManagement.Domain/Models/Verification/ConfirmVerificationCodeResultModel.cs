using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Models.Verification
{
    public class ConfirmVerificationCodeResultModel
    {
        public bool IsVerified { get; set; }

        public VerificationCodeError Error { get; set; }
    }
}
