using Autofac;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Lykke.Service.AdminManagement.Domain.Repositories;
using Lykke.Service.AdminManagement.MsSqlRepositories;
using Lykke.Service.AdminManagement.MsSqlRepositories.Repositories;
using Lykke.Service.AdminManagement.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.AdminManagement.Modules
{
    [UsedImplicitly]
    public class DataLayerModule : Module
    {
        private readonly DbSettings _settings;

        public DataLayerModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.CurrentValue.AdminManagementService.Db;
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
        }
    }
}
