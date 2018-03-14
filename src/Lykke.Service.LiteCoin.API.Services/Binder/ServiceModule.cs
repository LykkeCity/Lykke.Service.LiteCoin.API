using Autofac;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Broadcast;
using Lykke.Service.LiteCoin.API.Core.Fee;
using Lykke.Service.LiteCoin.API.Core.ObservableOperation;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Services.Address;
using Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi;
using Lykke.Service.LiteCoin.API.Services.Broadcast;
using Lykke.Service.LiteCoin.API.Services.Fee;
using Lykke.Service.LiteCoin.API.Services.ObservableOperation;
using Lykke.Service.LiteCoin.API.Services.Operations;
using Lykke.Service.LiteCoin.API.Services.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Services.Transactions;
using Lykke.Service.LiteCoin.API.Services.Wallet;
using Lykke.SettingsReader;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Binder
{
    public  class ServiceModule:Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<LiteCoinApiSettings> _settings;
        public ServiceModule(IReloadingManager<LiteCoinApiSettings> settings, ILog log)
        {
            _log = log;
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterNetwork(builder);
            RegisterFeeServices(builder);
            RegisterAddressValidatorServices(builder);
            RegisterInsightApiBlockChainReaders(builder);
            RegisterDetectorServices(builder);
            RegisterTransactionOutputsServices(builder);
            RegisterTransactionBuilderServices(builder);
            RegisterBroadcastServices(builder);
            RegisterObservableServices(builder);
        }

        private void RegisterNetwork(ContainerBuilder builder)
        {
            NBitcoin.Litecoin.Networks.EnsureRegistered();

            builder.RegisterInstance(Network.GetNetwork(_settings.CurrentValue.Network)).As<Network>();
        }

        private void RegisterFeeServices(ContainerBuilder builder)
        {
            builder.RegisterInstance(new FeeSettings
            {
                DefaultFeePerKyloByte = _settings.CurrentValue.DefaultFeePerKyloByte,
                DynamicFeeProviderUrl = _settings.CurrentValue.DynamicFeeProviderUrl,
                MaxFeePerKyloByte = _settings.CurrentValue.MaxFeePerKyloByte,
                MinFeePerKyloByte = _settings.CurrentValue.MinFeePerKyloByte
            }).AsSelf();

            builder.RegisterType<DynamicFeeProvider>()
                .As<IDynamicFeeProvider>();

            builder.RegisterType<FeeRateFacade>()
                .As<IFeeRateFacade>();

            builder.RegisterType<FeeService>().As<IFeeService>();
        }

        private void RegisterAddressValidatorServices(ContainerBuilder builder)
        {
            builder.RegisterType<AddressValidator>().As<IAddressValidator>();
        }

        private void RegisterInsightApiBlockChainReaders(ContainerBuilder builder)
        {
            builder.RegisterInstance(new InsightApiSettings
            {
                Url = _settings.CurrentValue.InsightAPIUrl
            }).SingleInstance();

            builder.RegisterType<InsightApiBlockChainProvider>().As<IBlockChainProvider>();
        }
        

        private void RegisterDetectorServices(ContainerBuilder builder)
        {
            builder.RegisterInstance(new OperationsConfirmationsSettings
            {
                MinConfirmationsToDetectOperation = _settings.CurrentValue.MinConfirmationsToDetectOperation
            });
            
        }
        
        private void RegisterTransactionOutputsServices(ContainerBuilder builder)
        {
            builder.RegisterType<TransactionOutputsService>().As<ITransactionOutputsService>();
        }

        private void RegisterTransactionBuilderServices(ContainerBuilder builder)
        {
            builder.RegisterType<TransactionBuilderService>().As<ITransactionBuilderService>();
            builder.RegisterType<OperationService>().As<IOperationService>();
        }

        private void RegisterBroadcastServices(ContainerBuilder builder)
        {
            builder.RegisterType<BroadcastService>().As<IBroadcastService>();
        }

        private void RegisterObservableServices(ContainerBuilder builder)
        {
            builder.RegisterType<ObservableOperationService>().As<IObservableOperationService>();
            builder.RegisterType<WalletBalanceService>().As<IWalletBalanceService>();
        }
    }
}
