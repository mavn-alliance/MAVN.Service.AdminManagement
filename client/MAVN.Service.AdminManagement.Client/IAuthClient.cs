using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.AdminManagement.Client.Models;
using MAVN.Service.AdminManagement.Client.Models.Requests;
using Refit;

namespace MAVN.Service.AdminManagement.Client
{
    /// <summary>
    /// Auth interface for AdminManagement client.
    /// </summary>
    [PublicAPI]
    public interface IAuthClient
    {
        /// <summary>
        /// Authenticates customer in the system.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><see cref="AuthenticateResponseModel"/></returns>
        [Post("/api/auth/login")]
        Task<AuthenticateResponseModel> AuthenticateAsync([Body] AuthenticateRequestModel request);

        /// <summary>
        /// Changes admin's password.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><see cref="ChangePasswordResponseModel"/></returns>
        [Post("/api/auth/changePassword")]
        Task<ChangePasswordResponseModel> ChangePasswordAsync([Body] ChangePasswordRequestModel request);
    }
}
