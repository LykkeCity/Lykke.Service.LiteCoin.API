using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Job.LiteCoin.Functions;
using Lykke.Job.LiteCoin.PeriodicalHandlers;
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
            
            builder.RegisterType<UpdateBalancesPeriodicalHandler>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(GetPeriod(_settings.CurrentValue.UpdateBalancesPeriod, nameof(LiteCoinApiSettings.UpdateBalancesPeriod))));

            builder.RegisterType<UpdateFeeRatePeriodicalHandler>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(GetPeriod(_settings.CurrentValue.UpdateFeeRatePeriod, nameof(LiteCoinApiSettings.UpdateFeeRatePeriod))));

            builder.RegisterType<UpdateObersvableOperationsPeriodicalHandler>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(GetPeriod(_settings.CurrentValue.UpdateObservableOperationsPeriod, nameof(LiteCoinApiSettings.UpdateObservableOperationsPeriod))));
        }

        private TimeSpan GetPeriod(string value, string settingName)
        {
            try
            {
                var result = TimeSpan.Parse(value);

                return result;
            }
            catch (FormatException e)
            {
                throw new FormatException($"Setting {settingName} parsing failed. Use hh:mm:ss format for timespan", innerException: e);
            }
        }

    }
}
