using JetBrains.Annotations;

namespace MAVN.Service.AdminManagement.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AdminManagementSettings
    {
        public DbSettings Db { get; set; }
        public string BackOfficeLink { set; get; }
        public AdminCreatedEmail AdminCreatedEmail { set; get; }
        public PartnerAdminWelcomeEmail PartnerAdminWelcomeEmail { set; get; }
        public PasswordResetEmail PasswordResetEmail { set; get; }
        public RabbitMqSettings RabbitMq { get; set; }
        public RedisSettings Redis { set; get; }
        public LimitationSettings LimitationSettings { get; set; }
    }
}
