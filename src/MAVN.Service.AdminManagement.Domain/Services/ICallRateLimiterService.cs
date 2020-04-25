using System.Threading.Tasks;

namespace MAVN.Service.AdminManagement.Domain.Services
{
    public interface ICallRateLimiterService
    {
        Task ClearAllCallRecordsForEmailVerificationAsync(string adminId);

        Task RecordEmailVerificationCallAsync(string adminId);

        Task<bool> IsAllowedToCallEmailVerificationAsync(string adminId);
    }
}
