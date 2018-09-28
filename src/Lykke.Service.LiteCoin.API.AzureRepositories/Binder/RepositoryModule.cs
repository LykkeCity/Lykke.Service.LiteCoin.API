using Autofac;
using AzureStorage.Blob;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.LiteCoin.API.AzureRepositories.Asset;
using Lykke.Service.LiteCoin.API.AzureRepositories.Fee;
using Lykke.Service.LiteCoin.API.AzureRepositories.Operations;
using Lykke.Service.LiteCoin.API.AzureRepositories.Transactions;
using Lykke.Service.LiteCoin.API.AzureRepositories.Wallet;
using Lykke.Service.LiteCoin.API.Core.Asset;
using Lykke.Service.LiteCoin.API.Core.Fee;
using Lykke.Service.LiteCoin.API.Core.ObservableOperation;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Core.Transactions;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.SettingsReader;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Binder
{
    public class RepositoryModule : Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<LiteCoinApiSettings> _settings;
        public RepositoryModule(IReloadingManager<LiteCoinApiSettings> settings, ILog log)
        {
            _log = log;
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepo(builder);
            RegisterBlob(builder);
        }

        private void RegisterRepo(ContainerBuilder builder)
        {
            builder.RegisterInstance(new AssetRepository())
                .As<IAssetRepository>()
                .SingleInstance();

            builder.RegisterInstance(new OperationMetaRepository(
                AzureTableStorage<OperationMetaEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "OperationMetaRef", _log)))
                .As<IOperationMetaRepository>()
                .SingleInstance();

            builder.RegisterInstance(new OperationEventRepository(
                    AzureTableStorage<OperationEventTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "OperationEventsRef", _log)))
                .As<IOperationEventRepository>()
                .SingleInstance();


            builder.RegisterInstance(new UnconfirmedTransactionRepository(
                AzureTableStorage<UnconfirmedTransactionEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "UnconfirmedTransactionsRef", _log)))
                .As<IUnconfirmedTransactionRepository>()
                .SingleInstance();

            builder.RegisterInstance(new ObservableOperationRepository(
                AzureTableStorage<ObservableOperationEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "ObservableOperationsRef", _log)))
                .As<IObservableOperationRepository>()
                .SingleInstance();

            builder.RegisterInstance(new ObservableWalletRepository(
                AzureTableStorage<ObservableWalletEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "ObservableWalletsRef", _log)))
                .As<IObservableWalletRepository>()
                .SingleInstance();

            builder.RegisterInstance(new WalletBalanceRepository(
                    AzureTableStorage<WalletBalanceEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "WalletBalancesRef", _log)))
                .As<IWalletBalanceRepository>()
                .SingleInstance();


            builder.RegisterInstance(new DynamicFeeRateRepository(
                    AzureTableStorage<DynamicFeeRateEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "DynamicFeeRate", _log)))
                .As<IDynamicFeeRateRepository>()
                .SingleInstance();
        }

        private void RegisterBlob(ContainerBuilder builder)
        {
            builder.RegisterInstance(
                new TransactionBlobStorage(AzureBlobStorage.Create(_settings.Nested(p => p.Db.DataConnString))))
                .As<ITransactionBlobStorage>();
        }
    }
}
