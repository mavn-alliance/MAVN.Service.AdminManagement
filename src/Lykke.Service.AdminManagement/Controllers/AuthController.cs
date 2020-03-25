using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.AdminManagement.Client;
using Lykke.Service.AdminManagement.Client.Models;
using Lykke.Service.AdminManagement.Client.Models.Enums;
using Lykke.Service.AdminManagement.Client.Models.Requests;
using Lykke.Service.AdminManagement.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.AdminManagement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller, IAuthClient
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        /// <summary>
        /// Authenticates customer in the system.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><see cref="AuthenticateResponseModel"/></returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthenticateResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<AuthenticateResponseModel> AuthenticateAsync([FromBody] AuthenticateRequestModel request)
        {
            var authModel = await _authService.AuthAsync(request.Email, request.Password);

            return _mapper.Map<AuthenticateResponseModel>(authModel);
        }

        /// <summary>
        /// Changes admin's password.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns><see cref="ChangePasswordResponseModel"/></returns>
        [HttpPost("changePassword")]
        [ProducesResponseType(typeof(ChangePasswordResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<ChangePasswordResponseModel> ChangePasswordAsync([FromBody] ChangePasswordRequestModel request)
        {
            var authModel = await _authService.UpdatePasswordAsync(request.Email, request.CurrentPassword, request.NewPassword);

            return new ChangePasswordResponseModel
            {
                Error = _mapper.Map<ChangePasswordErrorCodes>(authModel)
            };
        }
    }
}
