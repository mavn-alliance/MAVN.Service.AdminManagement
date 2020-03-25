using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.AdminManagement.Client;
using Lykke.Service.AdminManagement.Client.Models;
using Lykke.Service.AdminManagement.Client.Models.Enums;
using Lykke.Service.AdminManagement.Client.Models.Requests;
using Lykke.Service.AdminManagement.Domain.Enums;
using Lykke.Service.AdminManagement.Domain.Exceptions;
using Lykke.Service.AdminManagement.Domain.Models;
using Lykke.Service.AdminManagement.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Refit;
using AdminPermissionCheckErrorCodes = Lykke.Service.AdminManagement.Client.Models.Enums.AdminPermissionCheckErrorCodes;
using AdminUser = Lykke.Service.AdminManagement.Client.Models.AdminUser;
using SuggestedValueType = Lykke.Service.AdminManagement.Client.Models.Enums.SuggestedValueType;

namespace Lykke.Service.AdminManagement.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class AdminsController : Controller, IAdminsClient
    {
        private readonly IAdminUserService _adminUserService;
        private readonly IMapper _mapper;
        private readonly IAutofillValuesService _autofillValuesService;

        public AdminsController(
            IAdminUserService adminUserService,
            IMapper mapper,
            IAutofillValuesService autofillValuesService)
        {
            _adminUserService = adminUserService;
            _mapper = mapper;
            _autofillValuesService = autofillValuesService;
        }

        /// <summary>
        /// Returns a list of autofill values.
        /// </summary>
        /// <returns>A list of autofill values.</returns>
        [HttpGet("autofillValues")]
        [ProducesResponseType(typeof(AutofillValuesResponseModel), (int) HttpStatusCode.OK)]
        public async Task<AutofillValuesResponseModel> GetAutofillValuesAsync()
        {
            var values = await _autofillValuesService.GetAllAsync();

            return new AutofillValuesResponseModel
            {
                Values = values.Select(x => 
                    new SuggestedValueMapping
                    {
                        Type = _mapper.Map<SuggestedValueType>(x.Key),
                        Values = x.Value
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Registers new admin in the system.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><see cref="RegistrationResponseModel"/></returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegistrationResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<RegistrationResponseModel> RegisterAsync([FromBody] RegistrationRequestModel request)
        {
            var result = await _adminUserService.RegisterAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Company,
                request.Department,
                request.JobTitle,
                _mapper.Map<IReadOnlyList<Permission>>(request.Permissions));

            return _mapper.Map<RegistrationResponseModel>(result);
        }

        /// <summary>
        /// Registers new admin in the system.
        /// </summary>
        /// <param name="adminRequest">Request</param>
        /// <returns><see cref="RegistrationResponseModel"/></returns>
        [HttpPost("update")]
        [ProducesResponseType(typeof(AdminUserResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<AdminUserResponseModel> UpdateAsync([FromBody] UpdateAdminRequestModel adminRequest)
        {
            var result = await _adminUserService.UpdateDataAsync(
                adminRequest.AdminUserId,
                adminRequest.Company,
                adminRequest.Department,
                adminRequest.FirstName,
                adminRequest.LastName,
                adminRequest.JobTitle,
                adminRequest.PhoneNumber,
                adminRequest.IsActive);

            return _mapper.Map<AdminUserResponseModel>(result);
        }

        /// <summary>
        /// Updates permissions for admin.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><see cref="AdminUserResponseModel"/></returns>
        [HttpPost("updatePermissions")]
        [ProducesResponseType(typeof(AdminUserResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<AdminUserResponseModel> UpdatePermissionsAsync([FromBody] UpdatePermissionsRequestModel request)
        {
            var result = await _adminUserService.UpdatePermissionsAsync(
                request.AdminUserId,
                _mapper.Map<List<Permission>>(request.Permissions));

            return _mapper.Map<AdminUserResponseModel>(result);
        }

        /// <summary>
        /// Get admin users
        /// </summary>
        [HttpGet("getadminusers")]
        [ProducesResponseType(typeof(bool), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IReadOnlyList<AdminUser>> GetAdminUsersAsync()
        {
            var adminUsers = await _adminUserService.GetAllAsync();
            return _mapper.Map<List<AdminUser>>(adminUsers.ToList());
        }

        /// <summary>
        /// Gets paginated list of AdminUsers
        /// </summary>
        /// <returns><see cref="PaginatedAdminUserResponseModel"/></returns>
        [HttpPost("paginated")]
        [ProducesResponseType(typeof(PaginatedAdminUserResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<PaginatedAdminUserResponseModel> GetPaginatedAsync(
            [FromBody] PaginationRequestModel pagingInfo)
        {
            var result = await _adminUserService.GetPaginatedAsync(pagingInfo.CurrentPage, pagingInfo.PageSize, pagingInfo.Active);
            return _mapper.Map<PaginatedAdminUserResponseModel>(result);
        }

        /// <summary>
        /// Searches for the customer profile with a given email
        /// </summary>
        /// <returns><see cref="AdminUserResponseModel"/></returns>
        [HttpPost("getbyemail")]
        [ProducesResponseType(typeof(AdminUserResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<AdminUserResponseModel> GetByEmailAsync([FromBody] GetByEmailRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request?.Email))
                throw new BadRequestException($"{nameof(request.Email)} can't be empty");

            var admin = await _adminUserService.GetByEmailAsync(request.Email, request.Active);
            
            return _mapper.Map<AdminUserResponseModel>(admin);
        }

        /// <summary>
        /// Gets admin by Id
        /// </summary>
        /// <returns><see cref="AdminUserResponseModel"/></returns>
        [HttpPost("getById")]
        [ProducesResponseType(typeof(AdminUserResponseModel), (int)HttpStatusCode.OK)]
        public async Task<AdminUserResponseModel> GetByIdAsync([FromBody] GetAdminByIdRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request?.AdminUserId))
                throw new BadRequestException($"{nameof(request.AdminUserId)} can't be empty");

            var admin = await _adminUserService.GetByIdAsync(request.AdminUserId);
            
            return _mapper.Map<AdminUserResponseModel>(admin);
        }

        /// <summary>
        /// Gets a list of admin permissions
        /// </summary>
        [HttpPost("getPermissions")]
        [ProducesResponseType(typeof(List<AdminPermission>), (int)HttpStatusCode.OK)]
        public async Task<List<AdminPermission>> GetPermissionsAsync([FromBody] GetAdminByIdRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request?.AdminUserId))
                throw new BadRequestException($"{nameof(request.AdminUserId)} can't be empty");

            var permissions = await _adminUserService.GetPermissionsAsync(request.AdminUserId);

            return _mapper.Map<List<AdminPermission>>(permissions);
        }

        /// <summary>
        /// Resets admin password
        /// </summary>
        [HttpPost("resetPassword")]
        [ProducesResponseType(typeof(ResetPasswordResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ResetPasswordResponseModel> ResetPasswordAsync([FromBody] ResetPasswordRequestModel request)
        {
            var result = await _adminUserService.ResetPasswordAsync(request.AdminUserId, request.Password);

            return _mapper.Map<ResetPasswordResponseModel>(result);
        }
    }
}
