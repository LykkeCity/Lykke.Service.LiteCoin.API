﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.SettingsReader;
using Lykke.JobTriggers.Extenstions;
using Lykke.Service.LiteCoin.API.Core.Services;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Services;
using Lykke.Service.LiteCoin.API.Services.LifeiteManagers;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.LiteCoin.OperationsNotifications.Modules
{
    public class JobModule : Module
    {
        private readonly LiteCoinAPISettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettingsManager;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(LiteCoinAPISettings settings, IReloadingManager<DbSettings> dbSettingsManager, ILog log)
        {
            _settings = settings;
            _log = log;
            _dbSettingsManager = dbSettingsManager;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterAzureQueueHandlers(builder);

            // TODO: Add your dependencies here

            builder.Populate(_services);
        }

        private void RegisterAzureQueueHandlers(ContainerBuilder builder)
        {
            builder.AddTriggers(
                pool =>
                {
                    pool.AddDefaultConnection(_settings.Db.DataConnString);
                });
        }

    }
}
