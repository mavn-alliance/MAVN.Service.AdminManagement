using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Services;
using MAVN.Service.Credentials.Client;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Client.Models.Responses;
using MAVN.Service.Sessions.Client;
using MAVN.Service.Sessions.Client.Models;
using Moq;
using Xunit;

namespace MAVN.Service.AdminManagement.DomainServices.Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task UserTriesToLogIn_WithMissingLogin_LoginNotFoundErrorReturned()
        {
            var sessionsServiceClient = new Mock<ISessionsServiceClient>();
            var credentialsClient = new Mock<ICredentialsClient>();
            var adminUsersService = new Mock<IAdminUserService>();

            var response = new AdminCredentialsValidationResponse { Error = CredentialsError.LoginNotFound };
            var adminUsersServiceResponse = GetAdminUserResult();

            credentialsClient
                .Setup(x => x.Admins.ValidateAsync(It.IsAny<CredentialsValidationRequest>()))
                .ReturnsAsync(response);

            adminUsersService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), null))
                .ReturnsAsync(adminUsersServiceResponse);

            AuthService authService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                authService = new AuthService(
                    sessionsServiceClient.Object,
                    credentialsClient.Object,
                    adminUsersService.Object,
                    logFactory);
            }

            var result = await authService.AuthAsync("email", "password");

            Assert.Equal(response.Error.ToString(), result.Error.ToString());
            Assert.Null(result.CustomerId);
            Assert.Null(result.Token);
        }

        [Fact]
        public async Task UserTriesToLogIn_WithWrongPassword_PasswordMismatchErrorReturned()
        {
            var sessionsServiceClient = new Mock<ISessionsServiceClient>();
            var credentialsClient = new Mock<ICredentialsClient>();
            var adminUsersService = new Mock<IAdminUserService>();

            var response = new AdminCredentialsValidationResponse { Error = CredentialsError.PasswordMismatch };
            var adminUsersServiceResponse = GetAdminUserResult();

            credentialsClient
                .Setup(x => x.Admins.ValidateAsync(It.IsAny<CredentialsValidationRequest>()))
                .ReturnsAsync(response);
            
            adminUsersService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), null))
                .ReturnsAsync(adminUsersServiceResponse);

            AuthService authService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                authService = new AuthService(
                    sessionsServiceClient.Object,
                    credentialsClient.Object,
                    adminUsersService.Object,
                    logFactory);
            }

            var result = await authService.AuthAsync("email", "password");

            Assert.Equal(response.Error.ToString(), result.Error.ToString());
            Assert.Null(result.CustomerId);
            Assert.Null(result.Token);
        }

        [Fact]
        public async Task UserTriesToLogIn_WithInvalidFormatEmail_InvalidEmailOrPasswordFormatErrorReturned()
        {
            var sessionsServiceClient = new Mock<ISessionsServiceClient>();
            var credentialsClient = new Mock<ICredentialsClient>();
            var adminUsersService = new Mock<IAdminUserService>();

            var exception = new ClientApiException(HttpStatusCode.BadRequest, new ErrorResponse());
            var adminUsersServiceResponse = GetAdminUserResult();

            credentialsClient
                .Setup(x => x.Admins.ValidateAsync(It.IsAny<CredentialsValidationRequest>()))
                .Throws(exception);
            
            adminUsersService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), null))
                .ReturnsAsync(adminUsersServiceResponse);

            AuthService authService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                authService = new AuthService(
                    sessionsServiceClient.Object,
                    credentialsClient.Object,
                    adminUsersService.Object,
                    logFactory);
            }

            var result = await authService.AuthAsync("email", "password");

            Assert.Equal(ServicesError.InvalidEmailOrPasswordFormat.ToString(), result.Error.ToString());
            Assert.Null(result.CustomerId);
            Assert.Null(result.Token);
        }

        [Fact]
        public async Task UserTriesToLogIn_UnexpectedErrorInValidation_InvalidOperationIsThrown()
        {
            var sessionsServiceClient = new Mock<ISessionsServiceClient>();
            var credentialsClient = new Mock<ICredentialsClient>();
            var adminUsersService = new Mock<IAdminUserService>();

            var response = new AdminCredentialsValidationResponse { Error = CredentialsError.LoginAlreadyExists };
            var adminUsersServiceResponse = GetAdminUserResult();

            credentialsClient
                .Setup(x => x.Admins.ValidateAsync(It.IsAny<CredentialsValidationRequest>()))
                .ReturnsAsync(response);
            
            adminUsersService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), null))
                .ReturnsAsync(adminUsersServiceResponse);

            AuthService authService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                authService = new AuthService(
                    sessionsServiceClient.Object,
                    credentialsClient.Object,
                    adminUsersService.Object,
                    logFactory);
            }

            await Assert.ThrowsAsync<InvalidOperationException>(() => authService.AuthAsync("email", "password"));
        }

        [Fact]
        public async Task UserTriesToLogIn_WithValidCredentials_SuccessfullyAuthenticated()
        {
            var sessionsServiceClient = new Mock<ISessionsServiceClient>();
            var credentialsClient = new Mock<ICredentialsClient>();
            var adminUsersService = new Mock<IAdminUserService>();

            var credentialsResponse = new AdminCredentialsValidationResponse { AdminId = "1" };
            var adminUsersServiceResponse = GetAdminUserResult();

            credentialsClient
                .Setup(x => x.Admins.ValidateAsync(It.IsAny<CredentialsValidationRequest>()))
                .ReturnsAsync(credentialsResponse);

            var sessionResponse = new ClientSession { SessionToken = "token" };

            sessionsServiceClient
                .Setup(x => x.SessionsApi.AuthenticateAsync(credentialsResponse.AdminId, It.IsNotNull<CreateSessionRequest>()))
                .ReturnsAsync(sessionResponse);
            
            adminUsersService
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), null))
                .ReturnsAsync(adminUsersServiceResponse);

            AuthService authService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                authService = new AuthService(
                    sessionsServiceClient.Object,
                    credentialsClient.Object,
                    adminUsersService.Object,
                    logFactory);
            }

            var result = await authService.AuthAsync("email", "password");

            Assert.Equal(credentialsResponse.AdminId, result.CustomerId);
            Assert.Equal(sessionResponse.SessionToken, result.Token);
            Assert.Equal(ServicesError.None, result.Error);
        }

        private AdminUserResult GetAdminUserResult()
        {
            return new AdminUserResult
            {
                Error = AdminUserErrorCodes.None,
                Profile = new AdminUser
                {
                    IsEmailVerified = true,
                    IsActive = true
                }
            };
        }
    }
}
