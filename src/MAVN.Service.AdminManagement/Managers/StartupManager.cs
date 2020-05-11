using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Services;
using Lykke.Sdk;
using MAVN.Service.NotificationSystem.SubscriberContract;
using MAVN.Service.AdminManagement.Contract.Events;

namespace MAVN.Service.AdminManagement.Managers
{
    public class StartupManager : IStartupManager
    {
        private readonly IRabbitPublisher<EmailMessageEvent> _emailMessageEventPublisher;
        private readonly IRabbitPublisher<AdminEmailVerifiedEvent> _adminEmailVerifiedEventPublisher;

        public StartupManager(
            IRabbitPublisher<EmailMessageEvent> emailMessageEventPublisher,
            IRabbitPublisher<AdminEmailVerifiedEvent> adminEmailVerifiedEventPublisher
        )
        {
            _emailMessageEventPublisher = emailMessageEventPublisher;
            _adminEmailVerifiedEventPublisher = adminEmailVerifiedEventPublisher;
        }

        public Task StartAsync()
        {
            _emailMessageEventPublisher.Start();
            _adminEmailVerifiedEventPublisher.Start();

            return Task.CompletedTask;
        }
    }
}
