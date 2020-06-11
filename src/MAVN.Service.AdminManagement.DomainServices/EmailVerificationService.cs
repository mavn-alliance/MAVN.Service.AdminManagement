using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.AdminManagement.Contract.Events;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models.Verification;
using MAVN.Service.AdminManagement.Domain.Repositories;
using MAVN.Service.AdminManagement.Domain.Services;

namespace MAVN.Service.AdminManagement.DomainServices
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private const string PartnerPermissionsName = "ProgramPartners";

        private readonly IEmailVerificationCodeRepository _emailVerificationCodeRepository;
        private readonly IRabbitPublisher<AdminEmailVerifiedEvent> _codeVerifiedEventPublisher;
        private readonly INotificationsService _notificationsService;
        private readonly IAdminUserService _adminUserService;
        private readonly ILog _log;

        public EmailVerificationService(
            IEmailVerificationCodeRepository emailVerificationCodeRepository,
            IRabbitPublisher<AdminEmailVerifiedEvent> codeVerifiedEventPublisher,
            INotificationsService notificationsService,
            IAdminUserService adminUserService,
            ILogFactory logFactory)
        {
            _emailVerificationCodeRepository = emailVerificationCodeRepository;
            _codeVerifiedEventPublisher = codeVerifiedEventPublisher;
            _notificationsService = notificationsService;
            _adminUserService = adminUserService;
            _log = logFactory.CreateLog(this);
        }

        /// <inheritdoc />
        public async Task<ConfirmVerificationCodeResultModel> ConfirmCodeAsync(string verificationCode)
        {
            if (string.IsNullOrEmpty(verificationCode))
                throw new ArgumentNullException(nameof(verificationCode));

            string verificationCodeValue;
            try
            {
                verificationCodeValue = verificationCode.Base64ToString();
            }
            catch (FormatException)
            {
                _log.Warning($"Verification code {verificationCode} format error (must be base64 string)");
                return new ConfirmVerificationCodeResultModel
                {
                    IsVerified = false,
                    Error = VerificationCodeError.VerificationCodeDoesNotExist
                };
            }

            var existingEntity = await _emailVerificationCodeRepository.GetByValueAsync(verificationCodeValue);
            if (existingEntity == null)
            {
                _log.Warning($"Verification code {verificationCodeValue} not found in the system");
                return new ConfirmVerificationCodeResultModel
                {
                    Error = VerificationCodeError.VerificationCodeDoesNotExist
                };
            }

            if (existingEntity.IsVerified)
            {
                _log.Info($"Verification code {verificationCodeValue} already verified when trying to confirm");
                return new ConfirmVerificationCodeResultModel
                {
                    Error = VerificationCodeError.AlreadyVerified,
                    IsVerified = true
                };
            }

            if (verificationCodeValue != existingEntity.VerificationCode)
            {
                _log.Warning($"VerificationCode {verificationCodeValue} does not match the stored verification code");
                return new ConfirmVerificationCodeResultModel { Error = VerificationCodeError.VerificationCodeMismatch };
            }

            if (existingEntity.ExpireDate < DateTime.UtcNow)
            {
                _log.Warning($"VerificationCode {verificationCodeValue} has expired");
                return new ConfirmVerificationCodeResultModel { Error = VerificationCodeError.VerificationCodeExpired };
            }

            await Task.WhenAll(_emailVerificationCodeRepository.SetAsVerifiedAsync(verificationCodeValue))
                .ContinueWith(_ =>
                    _codeVerifiedEventPublisher.PublishAsync(new AdminEmailVerifiedEvent
                    {
                        AdminId = existingEntity.AdminId,
                        TimeStamp = DateTime.UtcNow
                    }));

            var admin = await _adminUserService.GetByIdAsync(existingEntity.AdminId);

            var partnersPermissions = admin.Profile.Permissions.FirstOrDefault(x => x.Type == PartnerPermissionsName);

            //check if it is program partner
            if (partnersPermissions != null && partnersPermissions.Level == PermissionLevel.PartnerEdit)
                await _notificationsService.NotifyPartnerAdminWelcomeAsync(admin.Profile.AdminUserId, admin.Profile.Email);

            return new ConfirmVerificationCodeResultModel { IsVerified = true };
        }
    }
}
