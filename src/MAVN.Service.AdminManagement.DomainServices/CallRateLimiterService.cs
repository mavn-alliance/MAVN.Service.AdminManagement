using System;
using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Services;
using StackExchange.Redis;

namespace MAVN.Service.AdminManagement.DomainServices
{
    public class CallRateLimiterService : ICallRateLimiterService
    {
        private readonly CallRateLimitSettingsDto _settings;
        private readonly string _redisInstanceName;
        private const string EmailVerificationKeyPattern = "{0}::admin_emailverification::{1}";

        private readonly IDatabase _db;

        public CallRateLimiterService(IConnectionMultiplexer connectionMultiplexer, CallRateLimitSettingsDto settings, string redisInstanceName)
        {
            _settings = settings;
            _redisInstanceName = redisInstanceName;
            _db = connectionMultiplexer.GetDatabase();
        }

        public Task ClearAllCallRecordsForEmailVerificationAsync(string adminId)
            => ClearAllCallRecordsAsync(adminId, EmailVerificationKeyPattern);

        public Task RecordEmailVerificationCallAsync(string adminId)
            => RecordCallAsync(adminId, EmailVerificationKeyPattern, _settings.EmailVerificationCallsMonitoredPeriod);

        public Task<bool> IsAllowedToCallEmailVerificationAsync(string adminId)
            => IsAllowedToCallAsync(adminId, EmailVerificationKeyPattern,
                _settings.EmailVerificationCallsMonitoredPeriod, _settings.EmailVerificationMaxAllowedRequestsNumber);

        private async Task ClearAllCallRecordsAsync(string adminId, string pattern)
        {
            var key = GetKeyFromPattern(adminId, pattern);

            await _db.SortedSetRemoveRangeByScoreAsync(key, double.MinValue, double.MaxValue);
        }

        private async Task RecordCallAsync(string adminId, string pattern, TimeSpan monitoredPeriod)
        {
            var key = GetKeyFromPattern(adminId, pattern);

            await _db.SortedSetAddAsync(key, DateTime.UtcNow.ToString(), DateTime.UtcNow.Ticks);
            await _db.KeyExpireAsync(key, monitoredPeriod);
        }

        private async Task<bool> IsAllowedToCallAsync(string adminId, string pattern, TimeSpan monitoredPeriod, int maxNumberOfCalls)
        {
            await ClearOldCallRecordsAsync(adminId, pattern, monitoredPeriod);

            var key = GetKeyFromPattern(adminId, pattern);
            var now = DateTime.UtcNow;
            var activeCallRecords = await _db.SortedSetRangeByScoreAsync(key, (now - monitoredPeriod).Ticks,
                now.Ticks);

            return activeCallRecords.Length < maxNumberOfCalls;
        }

        private async Task ClearOldCallRecordsAsync(string adminId, string pattern, TimeSpan monitoredPeriod)
        {
            var key = GetKeyFromPattern(adminId, pattern);
            await _db.SortedSetRemoveRangeByScoreAsync(key, double.MinValue,
                (DateTime.UtcNow - monitoredPeriod).Ticks);
        }

        private string GetKeyFromPattern(string adminId, string pattern)
        {
            return string.Format(pattern, _redisInstanceName, adminId);
        }
    }
}
