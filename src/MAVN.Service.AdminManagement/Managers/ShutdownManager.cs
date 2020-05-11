using System.Threading.Tasks;
using Lykke.Sdk;
using MAVN.Service.AdminManagement.Domain.Services;
using MAVN.Service.NotificationSystem.SubscriberContract;
using MAVN.Service.AdminManagement.Contract.Events;

namespace MAVN.Service.AdminManagement.Managers
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly IRabbitPublisher<EmailMessageEvent> _emailMessageEventPublisher;
        private readonly IRabbitPublisher<AdminEmailVerifiedEvent> _adminEmailVerifiedEventPublisher;

        public ShutdownManager(
            IRabbitPublisher<EmailMessageEvent> emailMessageEventPublisher,
            IRabbitPublisher<AdminEmailVerifiedEvent> adminEmailVerifiedEventPublisher
        )
        {
            _emailMessageEventPublisher = emailMessageEventPublisher;
            _adminEmailVerifiedEventPublisher = adminEmailVerifiedEventPublisher;
        }

        public Task StopAsync()
        {
            _emailMessageEventPublisher.Stop();
            _adminEmailVerifiedEventPublisher.Stop();

            return Task.CompletedTask;
        }
    }
}
