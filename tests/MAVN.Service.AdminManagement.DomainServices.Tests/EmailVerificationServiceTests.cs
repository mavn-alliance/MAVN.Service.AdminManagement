using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using MAVN.Service.AdminManagement.Contract.Events;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Models.Verification;
using MAVN.Service.AdminManagement.Domain.Repositories;
using MAVN.Service.AdminManagement.Domain.Services;
using Moq;
using Xunit;

namespace MAVN.Service.AdminManagement.DomainServices.Tests
{
    public class EmailVerificationServiceTests
    {
        private readonly Mock<IAdminUserService> _adminUserServiceMock = new Mock<IAdminUserService>();
        private readonly Mock<INotificationsService> _notificationsServiceMock = new Mock<INotificationsService>();

        [Fact]
        public async Task UserTriesToConfirmEmail_WithNewAdmin_Successfully()
        {
            var verificationEmailRepository = new Mock<IEmailVerificationCodeRepository>();

            var verificationEmailGetResponse = GetMockedVerificationCode();

            var confirmEmailResponse = new ConfirmVerificationCodeResultModel
            {
                Error = VerificationCodeError.None,
                IsVerified = true
            };

            verificationEmailRepository
                .Setup(x => x.GetByValueAsync(It.IsAny<string>()))
                .ReturnsAsync(verificationEmailGetResponse.Object);

            verificationEmailRepository
                .Setup(x => x.CreateOrUpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((IVerificationCode)null);

            _adminUserServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new AdminUserResult {Profile = new AdminUser {Permissions = new List<Permission>()}});
            
            var publisherCodeVerified = new Mock<IRabbitPublisher<AdminEmailVerifiedEvent>>();

            EmailVerificationService emailVerificationService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                emailVerificationService = new EmailVerificationService(
                    verificationEmailRepository.Object,
                    publisherCodeVerified.Object,
                    _notificationsServiceMock.Object,
                    _adminUserServiceMock.Object,
                    logFactory
                );
            }

            var result = await emailVerificationService.ConfirmCodeAsync("DDD666".ToBase64());

            Assert.Equal(confirmEmailResponse.Error.ToString(), result.Error.ToString());
            Assert.True(result.IsVerified);
        }

        [Fact]
        public async Task UserTriesToConfirmEmail_WithAdminThatDoesNotExists_AdminNotExistingReturned()
        {
            var verificationEmailRepository = new Mock<IEmailVerificationCodeRepository>();

            var confirmEmailResponse = new ConfirmVerificationCodeResultModel
            {
                Error = VerificationCodeError.VerificationCodeDoesNotExist,
                IsVerified = true
            };

            verificationEmailRepository
                .Setup(x => x.GetByValueAsync(It.IsAny<string>()))
                .ReturnsAsync(null as IVerificationCode);

            verificationEmailRepository
                .Setup(x => x.CreateOrUpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((IVerificationCode)null);
            
            var publisherCodeVerified = new Mock<IRabbitPublisher<AdminEmailVerifiedEvent>>();

            EmailVerificationService emailVerificationService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                emailVerificationService = new EmailVerificationService(
                    verificationEmailRepository.Object,
                    publisherCodeVerified.Object,
                    _notificationsServiceMock.Object,
                    _adminUserServiceMock.Object,
                    logFactory
                );
            }

            var result = await emailVerificationService.ConfirmCodeAsync("DDD666".ToBase64());

            Assert.Equal(confirmEmailResponse.Error.ToString(), result.Error.ToString());
            Assert.False(result.IsVerified);
        }

        [Fact]
        public async Task UserTriesToConfirmEmail_WithVerificationCodeThatNotExistsInTheStorage_VerificationCodeMismatchReturned()
        {
            var verificationEmailRepository = new Mock<IEmailVerificationCodeRepository>();

            var verificationEmailGetResponse = GetMockedVerificationCode();

            verificationEmailRepository
                .Setup(x => x.GetByValueAsync(It.IsAny<string>()))
                .ReturnsAsync(verificationEmailGetResponse.Object);

            verificationEmailRepository
                .Setup(x => x.CreateOrUpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((IVerificationCode)null);
            
            var publisherCodeVerified = new Mock<IRabbitPublisher<AdminEmailVerifiedEvent>>();

            EmailVerificationService emailVerificationService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                emailVerificationService = new EmailVerificationService(
                    verificationEmailRepository.Object,
                    publisherCodeVerified.Object,
                    _notificationsServiceMock.Object,
                    _adminUserServiceMock.Object,
                    logFactory
                );
            }

            var result = await emailVerificationService.ConfirmCodeAsync("123456".ToBase64());

            Assert.Equal(VerificationCodeError.VerificationCodeMismatch.ToString(), result.Error.ToString());
            Assert.False(result.IsVerified);
        }

        [Fact]
        public async Task UserTriesToConfirmEmail_WithVerificationCodeThatHasAlreadyExpired_VerificationCodeExpiredReturned()
        {
            var verificationEmailRepository = new Mock<IEmailVerificationCodeRepository>();

            var verificationEmailGetResponse = GetMockedVerificationCode();

            verificationEmailGetResponse.SetupProperty(_ => _.ExpireDate, DateTime.UtcNow.AddYears(-1000));

            var confirmEmailResponse = new ConfirmVerificationCodeResultModel
            {
                Error = VerificationCodeError.VerificationCodeExpired,
                IsVerified = true
            };

            verificationEmailRepository
                .Setup(x => x.GetByValueAsync(It.IsAny<string>()))
                .ReturnsAsync(verificationEmailGetResponse.Object);

            verificationEmailRepository
                .Setup(x => x.CreateOrUpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((IVerificationCode)null);
            
            var publisherCodeVerified = new Mock<IRabbitPublisher<AdminEmailVerifiedEvent>>();

            EmailVerificationService emailVerificationService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                emailVerificationService = new EmailVerificationService(
                    verificationEmailRepository.Object,
                    publisherCodeVerified.Object,
                    _notificationsServiceMock.Object,
                    _adminUserServiceMock.Object,
                    logFactory
                );
            }

            var result = await emailVerificationService.ConfirmCodeAsync("DDD666".ToBase64());

            Assert.Equal(confirmEmailResponse.Error.ToString(), result.Error.ToString());
            Assert.False(result.IsVerified);
        }

        [Fact]
        public async Task UserTriesToConfirmEmail_WithAdminThatIsAlreadyVerified_AlreadyVerifiedReturned()
        {
            var verificationEmailRepository = new Mock<IEmailVerificationCodeRepository>();

            var verificationEmailGetResponse = GetMockedVerificationCode();

            verificationEmailGetResponse.SetupProperty(_ => _.IsVerified, true);

            verificationEmailRepository
                .Setup(x => x.GetByValueAsync(It.IsAny<string>()))
                .ReturnsAsync(verificationEmailGetResponse.Object);

            verificationEmailRepository
                .Setup(x => x.CreateOrUpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((IVerificationCode)null);
            
            var publisherCodeVerified = new Mock<IRabbitPublisher<AdminEmailVerifiedEvent>>();

            EmailVerificationService emailVerificationService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                emailVerificationService = new EmailVerificationService(
                    verificationEmailRepository.Object,
                    publisherCodeVerified.Object,
                    _notificationsServiceMock.Object,
                    _adminUserServiceMock.Object,
                    logFactory
                );
            }

            var result = await emailVerificationService.ConfirmCodeAsync("DDD666".ToBase64());

            Assert.Equal(VerificationCodeError.AlreadyVerified.ToString(), result.Error.ToString());
            Assert.True(result.IsVerified);
        }

        [Fact]
        public async Task UserTriesToConfirmEmail_WithVerificationCodeThatNotExistsInTheSrorage_VerificationCodeMismatchReturned()
        {
            var verificationEmailRepository = new Mock<IEmailVerificationCodeRepository>();

            var verificationEmailGetResponse = GetMockedVerificationCode();

            var confirmEmailResponse = new ConfirmVerificationCodeResultModel
            {
                Error = VerificationCodeError.VerificationCodeMismatch,
                IsVerified = true
            };

            verificationEmailRepository
                .Setup(x => x.GetByValueAsync(It.IsAny<string>()))
                .ReturnsAsync(verificationEmailGetResponse.Object);

            verificationEmailRepository
                .Setup(x => x.CreateOrUpdateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(verificationEmailGetResponse.Object);

            var publisherCodeVerified = new Mock<IRabbitPublisher<AdminEmailVerifiedEvent>>();

            EmailVerificationService emailVerificationService;
            using (var logFactory = LogFactory.Create().AddUnbufferedConsole())
            {
                emailVerificationService = new EmailVerificationService(
                    verificationEmailRepository.Object,
                    publisherCodeVerified.Object,
                    _notificationsServiceMock.Object,
                    _adminUserServiceMock.Object,
                    logFactory
                );
            }

            var result =
                await emailVerificationService.ConfirmCodeAsync("123456".ToBase64());

            Assert.Equal(confirmEmailResponse.Error.ToString(), result.Error.ToString());
            Assert.False(result.IsVerified);
        }

        private Mock<IVerificationCode> GetMockedVerificationCode()
        {
            var verificationEmailGetResponse = new Mock<IVerificationCode>();
            verificationEmailGetResponse.SetupProperty(_ => _.AdminId, "70fb9648-f482-4c29-901b-25fe6febd8af");
            verificationEmailGetResponse.SetupProperty(_ => _.ExpireDate, DateTime.UtcNow.AddYears(1000));
            verificationEmailGetResponse.SetupProperty(_ => _.VerificationCode, "DDD666");
            verificationEmailGetResponse.SetupProperty(_ => _.IsVerified, false);

            return verificationEmailGetResponse;
        }
    }
}
