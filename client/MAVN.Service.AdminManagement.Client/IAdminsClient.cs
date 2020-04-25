using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.AdminManagement.Client.Models;
using MAVN.Service.AdminManagement.Client.Models.Requests;
using MAVN.Service.AdminManagement.Client.Models.Requests.Verification;
using MAVN.Service.AdminManagement.Client.Models.Responses.Verification;
using Refit;

namespace MAVN.Service.AdminManagement.Client
{
    /// <summary>
    /// Admins interface for AdminManagement client.
    /// </summary>
    [PublicAPI]
    public interface IAdminsClient
    {
        /// <summary>
        /// Returns a list of autofill values.
        /// </summary>
        /// <returns>A list of autofill values.</returns>
        [Get("/api/admins/autofillValues")]
        Task<AutofillValuesResponseModel> GetAutofillValuesAsync();
        
        /// <summary>
        /// Registers new admin in the system.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><see cref="RegistrationResponseModel"/></returns>
        [Post("/api/admins/register")]
        Task<RegistrationResponseModel> RegisterAsync([Body] RegistrationRequestModel request);
        
        /// <summary>
        /// Confirm Email in the system.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns><see cref="VerificationCodeConfirmationResponseModel"/></returns>
        [Post("/api/admins/confirmemail")]
        Task<VerificationCodeConfirmationResponseModel> ConfirmEmailAsync([Body] VerificationCodeConfirmationRequestModel request);

        /// <summary>
        /// Update admin data.
        /// </summary>
        /// <param name="adminRequest">Request</param>
        /// <returns><see cref="RegistrationResponseModel"/></returns>
        [Post("/api/admins/update")]
        Task<AdminUserResponseModel> UpdateAsync([Body] UpdateAdminRequestModel adminRequest);

        /// <summary>
        /// Get Admin users
        /// </summary>
        /// <returns><see cref="AdminUser"/></returns>
        [Get("/api/admins/getadminusers")]
        Task<IReadOnlyList<AdminUser>> GetAdminUsersAsync();

        /// <summary>
        /// Gets paginated list of AdminUsers
        /// </summary>
        /// <returns><see cref="PaginatedAdminUserResponseModel"/></returns>
        [Post("/api/admins/paginated")]
        Task<PaginatedAdminUserResponseModel> GetPaginatedAsync([Body] PaginationRequestModel pagingInfo);

        /// <summary>
        /// Searches for the customer profile with a given email
        /// </summary>
        /// <returns><see cref="AdminUserResponseModel"/></returns>
        [Post("/api/admins/getbyemail")]
        Task<AdminUserResponseModel> GetByEmailAsync([Body] GetByEmailRequestModel request);

        /// <summary>
        /// Gets admin by Id
        /// </summary>
        /// <returns><see cref="AdminUserResponseModel"/></returns>
        [Post("/api/admins/getById")]
        Task<AdminUserResponseModel> GetByIdAsync(
            [Body] GetAdminByIdRequestModel request);

        /// <summary>
        /// Updates admin permissions list
        /// </summary>
        /// <returns><see cref="AdminUserResponseModel"/></returns>
        [Post("/api/admins/updatePermissions")]
        Task<AdminUserResponseModel> UpdatePermissionsAsync(
            [Body] UpdatePermissionsRequestModel request);

        /// <summary>
        /// Updates admin permissions list
        /// </summary>
        [Post("/api/admins/getPermissions")]
        Task<List<AdminPermission>> GetPermissionsAsync([Body] GetAdminByIdRequestModel request);

        /// <summary>
        /// Resets admin password
        /// </summary>
        [Post("/api/admins/resetPassword")]
        Task<ResetPasswordResponseModel> ResetPasswordAsync([Body] ResetPasswordRequestModel request);
    }
}
