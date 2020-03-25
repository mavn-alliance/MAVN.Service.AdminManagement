using System;
using System.Text.RegularExpressions;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Logs.Loggers.LykkeSanitizing;
using Lykke.Sdk;
using Lykke.Service.AdminManagement.Middleware;
using Lykke.Service.AdminManagement.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.AdminManagement
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "AdminManagement API", ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "AdminManagementLog";
                    logs.AzureTableConnectionStringResolver =
                        settings => settings.AdminManagementService.Db.LogsConnString;
                    logs.Extended = x => x.AddSanitizingFilter(new Regex(@"(\\?""?[Pp]assword\\?""?:\s*\\?"")(.*?)(\\?"")"), "$1*$3")
                        .AddSanitizingFilter(new Regex(@"(\\?""?[Ll]ogin\\?""?:\s*\\?"")(.*?)(\\?"")"), "$1*$3")
                        .AddSanitizingFilter(new Regex(@"(\\?""?[Ee]mail\\?""?:\s*\\?"")(.*?)(\\?"")"), "$1*$3");
                };

                options.Extend = (serviceCollection, settings) =>
                {
                    serviceCollection.AddAutoMapper(
                        typeof(AutoMapperProfile),
                        typeof(MsSqlRepositories.AutoMapperProfile));
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.WithMiddleware = applicationBuilder =>
                {
                    applicationBuilder.UseMiddleware<BadRequestExceptionMiddleware>();
                };
            });
        }
    }
}
