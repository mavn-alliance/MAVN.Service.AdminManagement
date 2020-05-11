using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common;
using MAVN.Service.AdminManagement.Domain.Services;
using MAVN.Service.NotificationSystem.SubscriberContract;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models.Emails;

namespace MAVN.Service.AdminManagement.DomainServices
{
    public class NotificationsService : INotificationsService
    {
        private readonly IRabbitPublisher<EmailMessageEvent> _emailsPublisher;

        private readonly string _backOfficeUrl;
        private readonly string _adminCreatedEmailTemplateId;
        private readonly string _adminCreatedEmailSubjectTemplateId;
        private readonly string _adminCreatedVerificationLinkPath;
        private readonly string _adminPasswordResetEmailTemplateId;
        private readonly string _adminPasswordResetEmailSubjectTemplateId;

        public NotificationsService(
            IRabbitPublisher<EmailMessageEvent> emailsPublisher,
            string backOfficeUrl,
            string adminCreatedEmailTemplateId,
            string adminCreatedEmailSubjectTemplateId,
            string adminCreatedVerificationLinkPath,
            string adminPasswordResetEmailTemplateId,
            string adminPasswordResetEmailSubjectTemplateId)
        {
            _emailsPublisher = emailsPublisher;
            _backOfficeUrl = backOfficeUrl;
            _adminCreatedEmailTemplateId = adminCreatedEmailTemplateId;
            _adminCreatedEmailSubjectTemplateId = adminCreatedEmailSubjectTemplateId;
            _adminCreatedVerificationLinkPath = adminCreatedVerificationLinkPath;
            _adminPasswordResetEmailTemplateId = adminPasswordResetEmailTemplateId;
            _adminPasswordResetEmailSubjectTemplateId = adminPasswordResetEmailSubjectTemplateId;
        }

        public async Task NotifyAdminCreatedAsync(AdminCreatedEmailDto model)
        {
            var url = GetLocalizedPath(_backOfficeUrl, model.Localization);

            var values = new Dictionary<string, string>
            {
                {nameof(model.Name), model.Name},
                {"BackOfficeUrl", url},
                {"EmailVerificationLink", url + _adminCreatedVerificationLinkPath.TrimStart('/').Replace("{0}", model.EmailVerificationCode)},
                {"Login", model.Email},
                {nameof(model.Password), model.Password},
                {nameof(model.Localization), model.Localization.ToString()}
            };

            await SendEmailAsync(model.AdminUserId, model.Email, values, _adminCreatedEmailTemplateId,
                _adminCreatedEmailSubjectTemplateId);
        }

        public async Task NotifyAdminPasswordResetAsync(string adminUserId, string email, string login, string password, string name)
        {
            var values = new Dictionary<string, string>
            {
                {"Name", name},
                {"BackOfficeUrl", _backOfficeUrl},
                {"Login", login},
                {"Password", password}
            };

            await SendEmailAsync(adminUserId, email, values, _adminPasswordResetEmailTemplateId,
                _adminPasswordResetEmailSubjectTemplateId);
        }

        private async Task SendEmailAsync(string customerId, string destination, Dictionary<string, string> values, string emailTemplateId, string subjectTemplateId)
        {
            if(!string.IsNullOrWhiteSpace(destination))
                values["target_email"] = destination;
            
            await _emailsPublisher.PublishAsync(new EmailMessageEvent
            {
                CustomerId = customerId,
                MessageTemplateId = emailTemplateId,
                SubjectTemplateId = subjectTemplateId,
                TemplateParameters = values,
                Source = $"{AppEnvironment.Name} - {AppEnvironment.Version}"
            });
        }

        private string GetLocalizedPath(string url, Localization localization)
        {
            return url.TrimEnd('/') + $"/{localization.ToString().ToLower()}/";
        }
    }
}
