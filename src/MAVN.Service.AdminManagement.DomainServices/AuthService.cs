using System;
using System.Net;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using Lykke.Service.Credentials.Client;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Services;
using Lykke.Service.Credentials.Client.Models.Requests;
using Lykke.Service.Credentials.Client.Models.Responses;
using Lykke.Service.Sessions.Client;
using Lykke.Service.Sessions.Client.Models;

namespace MAVN.Service.AdminManagement.DomainServices
{
    public class AuthService : IAuthService
    {
        private readonly ISessionsServiceClient _sessionsServiceClient;
        private readonly ICredentialsClient _credentialsClient;
        private readonly IAdminUserService _adminUserService;
        private readonly ILog _log;

        public AuthService(
            ISessionsServiceClient sessionsServiceClient,
            ICredentialsClient credentialsClient,
            IAdminUserService adminUserService,
            ILogFactory logFactory)
        {
            _sessionsServiceClient = sessionsServiceClient;
            _credentialsClient = credentialsClient;
            _adminUserService = adminUserService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<AuthResultModel> AuthAsync(string login, string password)
        {
            var admin = await _adminUserService.GetByEmailAsync(login, null);

            if (admin.Error == AdminUserErrorCodes.None)
            {
                if (!admin.Profile.IsEmailVerified)
                {
                    return new AuthResultModel { Error = ServicesError.AdminEmailIsNotVerified };
                }

                if (admin.Profile.IsActive == false)
                {
                    return new AuthResultModel {Error = ServicesError.AdminNotActive};
                }
            }
            
            AdminCredentialsValidationResponse credentials;

            try
            {
                credentials = await _credentialsClient.Admins
                    .ValidateAsync(new CredentialsValidationRequest {Login = login, Password = password});
            }
            catch (ClientApiException e) when (e.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return new AuthResultModel {Error = ServicesError.InvalidEmailOrPasswordFormat};
            }

            if (credentials.AdminId == null)
            {
                switch (credentials.Error)
                {
                    case CredentialsError.LoginNotFound:
                        return new AuthResultModel {Error = ServicesError.LoginNotFound};

                    case CredentialsError.PasswordMismatch:
                        return new AuthResultModel {Error = ServicesError.PasswordMismatch};
                }

                var exception = new InvalidOperationException(
                    $"Unexpected error during credentials validation for {login.SanitizeEmail()}");

                _log.Error(exception, context: credentials.Error);

                throw exception;
            }

            var session = await _sessionsServiceClient.SessionsApi
                .AuthenticateAsync(credentials.AdminId, new CreateSessionRequest());

            return new AuthResultModel {CustomerId = credentials.AdminId, Token = session.SessionToken};
        }

        public async Task<PasswordUpdateError> UpdatePasswordAsync(string login, string oldPassword, string newPassword)
        {
            var admin = await _adminUserService.GetByEmailAsync(login, null);

            if (admin.Error == AdminUserErrorCodes.None)
            {
                if (admin.Profile.IsActive == false)
                {
                    return PasswordUpdateError.AdminNotActive;
                }
            }
            
            AdminCredentialsValidationResponse credentials;

            try
            {
                credentials = await _credentialsClient.Admins
                    .ValidateAsync(new CredentialsValidationRequest {Login = login, Password = oldPassword});
            }
            catch (ClientApiException e) when (e.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return PasswordUpdateError.InvalidEmailOrPasswordFormat;
            }
            
            if (credentials.AdminId == null)
            {
                switch (credentials.Error)
                {
                    case CredentialsError.LoginNotFound:
                        return PasswordUpdateError.LoginNotFound;

                    case CredentialsError.PasswordMismatch:
                        return PasswordUpdateError.PasswordMismatch;
                }

                var exception = new InvalidOperationException(
                    $"Unexpected error during credentials validation for {login.SanitizeEmail()}");

                _log.Error(exception, context: credentials.Error);

                throw exception;
            }

            try
            {
                await _credentialsClient.Admins.ChangePasswordAsync(new AdminCredentialsUpdateRequest
                {
                    AdminId = admin.Profile.AdminUserId,
                    Login = login,
                    Password = newPassword
                });
            }
            catch (ClientApiException exception) when (exception.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return PasswordUpdateError.NewPasswordInvalid;
            }

            return PasswordUpdateError.None;
        }
    }
}
