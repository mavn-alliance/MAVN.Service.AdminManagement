using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using MAVN.Service.Credentials.Client;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.Sessions.Client;

namespace MAVN.Service.AdminManagement.Settings
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
