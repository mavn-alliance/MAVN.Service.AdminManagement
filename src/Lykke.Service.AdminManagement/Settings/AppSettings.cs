using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.Credentials.Client;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.Sessions.Client;

namespace Lykke.Service.AdminManagement.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public AdminManagementSettings AdminManagementService { get; set; }

        public SessionsServiceClientSettings SessionsService { get; set; }

        public CredentialsServiceClientSettings CredentialsService { get; set; }

        public CustomerProfileServiceClientSettings CustomerProfileServiceClient { get; set; }
    }
}
