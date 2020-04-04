using JetBrains.Annotations;

namespace MAVN.Service.AdminManagement.Client
{
    /// <summary>
    /// AdminManagement client interface.
    /// </summary>
    [PublicAPI]
    public interface IAdminManagementServiceClient
    {
        /// <summary><see cref="IAuthClient"/> inerface property.</summary>
        IAuthClient AuthApi { get; set; }

        /// <summary><see cref="IAdminsClient"/> inerface property.</summary>
        IAdminsClient AdminsApi { get; set; }
    }
}
