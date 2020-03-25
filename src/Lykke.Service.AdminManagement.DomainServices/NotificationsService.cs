using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common;
using Lykke.Service.AdminManagement.Domain.Services;
using Lykke.Service.NotificationSystem.SubscriberContract;

namespace Lykke.Service.AdminManagement.DomainServices
{
    public class NotificationsService : INotificationsService
    {
        private readonly IRabbitPublisher<EmailMessageEvent> _emailsPublisher;

        private readonly string _backOfficeUrl;
        private readonly string _adminCreatedEmailTemplateId;
        private readonly string _adminCreatedEmailSubjectTemplateId;
        private readonly string _adminPasswordResetEmailTemplateId;
        private readonly string _adminPasswordResetEmailSubjectTemplateId;

        public NotificationsService(
            IRabbitPublisher<EmailMessageEvent> emailsPublisher,
            string backOfficeUrl,
            string adminCreatedEmailTemplateId,
            string adminCreatedEmailSubjectTemplateId,
            string adminPasswordResetEmailTemplateId,
            string adminPasswordResetEmailSubjectTemplateId)
        {
            _emailsPublisher = emailsPublisher;
            _backOfficeUrl = backOfficeUrl;
            _adminCreatedEmailTemplateId = adminCreatedEmailTemplateId;
            _adminCreatedEmailSubjectTemplateId = adminCreatedEmailSubjectTemplateId;
            _adminPasswordResetEmailTemplateId = adminPasswordResetEmailTemplateId;
            _adminPasswordResetEmailSubjectTemplateId = adminPasswordResetEmailSubjectTemplateId;
        }

        public async Task NotifyAdminCreatedAsync(string adminUserId, string email, string login, string password, string name)
        {
            var values = new Dictionary<string, string>
            {
                {"Name", name},
                {"BackOfficeUrl", _backOfficeUrl},
                {"Login", login},
                {"Password", password}
            };

            await SendEmailAsync(adminUserId, email, values, _adminCreatedEmailTemplateId,
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
    }
}