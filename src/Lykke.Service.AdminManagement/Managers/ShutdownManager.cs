using System.Threading.Tasks;
using Lykke.Sdk;
using Lykke.Service.AdminManagement.Domain.Services;
using Lykke.Service.NotificationSystem.SubscriberContract;

namespace Lykke.Service.AdminManagement.Managers
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly IRabbitPublisher<EmailMessageEvent> _emailMessageEventPublisher;

        public ShutdownManager(IRabbitPublisher<EmailMessageEvent> emailMessageEventPublisher)
        {
            _emailMessageEventPublisher = emailMessageEventPublisher;
        }

        public Task StopAsync()
        {
            _emailMessageEventPublisher.Stop();

            return Task.CompletedTask;
        }
    }
}