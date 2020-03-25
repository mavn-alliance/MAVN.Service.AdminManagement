using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.Service.AdminManagement.Domain.Services;
using Lykke.Service.AdminManagement.DomainServices;
using Lykke.Service.AdminManagement.Settings;
using Lykke.Service.NotificationSystem.SubscriberContract;
using Lykke.SettingsReader;

namespace Lykke.Service.AdminManagement.Modules
{
    [UsedImplicitly]
    public class RabbitMqModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        
        private const string NotificationSystemEmailExchangeName = "lykke.notificationsystem.command.emailmessage";
        
        public RabbitMqModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        protected override void Load(ContainerBuilder builder)
        {
            var rabbitMqSettings = _appSettings.CurrentValue.AdminManagementService.RabbitMq;

            builder.RegisterGeneric(typeof(RabbitPublisher<>));
            
            builder.RegisterType<RabbitPublisher<EmailMessageEvent>>()
                .As<IRabbitPublisher<EmailMessageEvent>>()
                .As<IStartable>()
                .SingleInstance()
                .WithParameter("connectionString", rabbitMqSettings.RabbitMqConnectionString)
                .WithParameter("exchangeName", NotificationSystemEmailExchangeName);
        }
    }
}