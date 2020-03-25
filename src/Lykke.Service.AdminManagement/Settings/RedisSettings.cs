using System;
using JetBrains.Annotations;

namespace Lykke.Service.AdminManagement.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RedisSettings
    {
        public string ConnectionString { set; get; }
        public string InstanceName { set; get; }
        public TimeSpan Ttl { set; get; }
    }
}