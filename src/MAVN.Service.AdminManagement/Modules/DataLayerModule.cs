using Autofac;
using JetBrains.Annotations;
using MAVN.Common.MsSql;
using MAVN.Service.AdminManagement.Domain.Repositories;
using MAVN.Service.AdminManagement.MsSqlRepositories;
using MAVN.Service.AdminManagement.MsSqlRepositories.Repositories;
using MAVN.Service.AdminManagement.Settings;
using Lykke.SettingsReader;
using System;

namespace MAVN.Service.AdminManagement.Modules
{
    [UsedImplicitly]
    public class DataLayerModule : Module
    {
        private readonly DbSettings _settings;
        private readonly TimeSpan _verificationLinkExpirePeriod;

        public DataLayerModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.CurrentValue.AdminManagementService.Db;
            _verificationLinkExpirePeriod = settings.CurrentValue.AdminManagementService.AdminCreatedEmail.VerificationLinkExpirePeriod;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(
                _settings.DbConnectionString,
                connString => new AdminManagementContext(connString, false),
                dbConn => new AdminManagementContext(dbConn));

            builder.RegisterType<AdminUsersRepository>()
                .As<IAdminUsersRepository>()
                .SingleInstance();

            builder.RegisterType<PermissionsRepository>()
                .As<IPermissionsRepository>()
                .SingleInstance();

            builder.RegisterType<EmailVerificationCodeRepository>()
                .WithParameter(TypedParameter.From(_verificationLinkExpirePeriod))
                .As<IEmailVerificationCodeRepository>()
                .SingleInstance();
        }
    }
}
