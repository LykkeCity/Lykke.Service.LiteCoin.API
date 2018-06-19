using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Job.LiteCoin.Functions;
using Lykke.Job.LiteCoin.PeriodicalHandlers;
using Lykke.Job.LiteCoin.Settings;
using Lykke.JobTriggers.Extenstions;
using Lykke.Service.LiteCoin.API.Core.Services;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Services;
using Lykke.Service.LiteCoin.API.Services.Health;
using Lykke.Service.LiteCoin.API.Services.LifeiteManagers;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.LiteCoin.Modules
{
    public class LiteCoinJobModule : Module
    {
        private readonly IReloadingManager<LiteCoinApiSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public LiteCoinJobModule(IReloadingManager<LiteCoinApiSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

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

            RegisterPeriodicalHandlers(builder);

            builder.Populate(_services);
        }

        private void RegisterAzureQueueHandlers(ContainerBuilder builder)
        {
            builder.AddTriggers(
                pool =>
                {
                    pool.AddDefaultConnection(_settings.Nested(x=>x.Db.DataConnString));
                });
        }

        private void RegisterPeriodicalHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<UpdateBalanceFunctions>()
                .AsSelf();

            builder.RegisterType<UpdateFeeRateFunctions>()
                .AsSelf();

            builder.RegisterType<UpdateObservableOperationsFunctions>()
                .AsSelf();

            builder.RegisterInstance(new PerodicalHandlerSettings
            {
                UpdateObservableOperationsPeriod = _settings.CurrentValue.UpdateObservableOperationsPeriod,
                UpdateBalancesPeriod = _settings.CurrentValue.UpdateBalancesPeriod,
                UpdateFeeRatePeriod = _settings.CurrentValue.UpdateFeeRatePeriod
            }).AsSelf();

            builder.RegisterType<PeriodicalHandlerHost>().AsSelf().SingleInstance();
        }
    }
}
