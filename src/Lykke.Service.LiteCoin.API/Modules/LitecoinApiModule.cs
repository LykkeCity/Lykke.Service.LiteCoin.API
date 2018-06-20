using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.Services;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.LifetimeManagers;
using Lykke.Service.LiteCoin.API.Services;
using Lykke.Service.LiteCoin.API.Services.Health;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.LiteCoin.API.Modules
{
    public class LitecoinApiModule : Module
    {
        private readonly IReloadingManager<LiteCoinApiSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public LitecoinApiModule(IReloadingManager<LiteCoinApiSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<ApiStartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ApiShutdownManager>()
                .As<IShutdownManager>();

            builder.Populate(_services);
        }
    }
}
