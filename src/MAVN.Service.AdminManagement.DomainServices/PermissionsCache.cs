using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Services;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MAVN.Service.AdminManagement.DomainServices
{
    public class PermissionsCache : IPermissionsCache
    {
        private const string PermissionsKeyPattern = "{0}:adminId:{1}";

        private const string NoValue = "NO_VALUE";

        private readonly IDatabase _db;
        private readonly string _redisInstanceName;
        private readonly TimeSpan _ttl;

        public PermissionsCache(IConnectionMultiplexer connectionMultiplexer, string redisInstanceName, TimeSpan ttl)
        {
            _db = connectionMultiplexer.GetDatabase();
            _redisInstanceName = redisInstanceName;
            _ttl = ttl;
        }

        public async Task<(bool, List<Permission>)> TryGetAsync(string adminId)
        {
            var keyValue = await _db.StringGetAsync(GetKey(adminId));

            if (string.IsNullOrWhiteSpace(keyValue))
            {
                return (false, null);
            }

            if (keyValue == NoValue)
            {
                return (true, null);
            }

            var permissions = JsonConvert.DeserializeObject<List<Permission>>(keyValue);

            return (true, permissions);
        }

        public async Task SetAsync(string adminId, List<Permission> permissions)
        {
            await _db.StringSetAsync(GetKey(adminId), permissions?.ToJson() ?? NoValue, _ttl);
        }

        private string GetKey(string adminId)
        {
            return string.Format(PermissionsKeyPattern, _redisInstanceName, adminId);
        }
    }
}