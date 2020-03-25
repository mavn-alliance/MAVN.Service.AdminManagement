using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.AdminManagement.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }
    }
}