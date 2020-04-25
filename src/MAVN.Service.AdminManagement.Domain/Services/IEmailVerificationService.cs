using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Models.Verification;

namespace MAVN.Service.AdminManagement.Domain.Services
{
    public interface IEmailVerificationService
    {
        /// <summary>
        /// Confirms verification code
        /// </summary>
        /// <param name="verificationCode">Verification code value in base64 format</param>
        /// <returns></returns>
        Task<ConfirmVerificationCodeResultModel> ConfirmCodeAsync(string verificationCode);
    }
}
