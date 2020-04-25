using System;

namespace MAVN.Service.AdminManagement.Domain.Models.Verification
{
    public interface IVerificationCode
    {
        string AdminId { get; set; }

        string VerificationCode { get; set; }

        bool IsVerified { get; set; }

        DateTime ExpireDate { get; set; }
    }
}
