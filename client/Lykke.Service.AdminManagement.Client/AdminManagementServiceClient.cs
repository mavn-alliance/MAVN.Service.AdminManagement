using Lykke.HttpClientGenerator;

namespace Lykke.Service.AdminManagement.Client
{
    /// <summary>
    /// AdminManagement API aggregating interface.
    /// </summary>
    public class AdminManagementServiceClient : IAdminManagementServiceClient
    {
        /// <inheritdoc cref="IAdminManagementServiceClient"/>
        public IAuthClient AuthApi { get; set; }

        /// <inheritdoc cref="IAdminManagementServiceClient"/>
        public IAdminsClient AdminsApi { get; set; }

        /// <summary>C-tor</summary>
        public AdminManagementServiceClient(IHttpClientGenerator httpClientGenerator)
        {
            AuthApi = httpClientGenerator.Generate<IAuthClient>();
            AdminsApi = httpClientGenerator.Generate<IAdminsClient>();
        }
    }
}
