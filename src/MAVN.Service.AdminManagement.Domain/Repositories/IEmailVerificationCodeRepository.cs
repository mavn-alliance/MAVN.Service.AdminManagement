using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Models.Verification;

namespace MAVN.Service.AdminManagement.Domain.Repositories
{
    public interface IEmailVerificationCodeRepository
    {
        Task<IVerificationCode> CreateOrUpdateAsync(string adminId, string verificationCode);
        Task<IVerificationCode> GetByValueAsync(string value);
        Task SetAsVerifiedAsync(string value);
    }
}
