using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Pagination;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Services.Operations;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Wallet
{
    public class WalletBalanceService:IWalletBalanceService
    {
        private readonly IWalletBalanceRepository _balanceRepository;
        private readonly IObservableWalletRepository _observableWalletRepository;
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly OperationsConfirmationsSettings _confirmationsSettings;

        public WalletBalanceService(IWalletBalanceRepository balanceRepository, 
            IObservableWalletRepository observableWalletRepository,
            IBlockChainProvider blockChainProvider, 
            OperationsConfirmationsSettings confirmationsSettings)
        {
            _balanceRepository = balanceRepository;
            _observableWalletRepository = observableWalletRepository;
            _blockChainProvider = blockChainProvider;
            _confirmationsSettings = confirmationsSettings;
        }

        public async Task Subscribe(string address)
        {
            await _observableWalletRepository.Insert(ObservableWallet.Create(address));
        }

        public async Task Unsubscribe(string address)
        {
            await _observableWalletRepository.Delete(address);
            await _balanceRepository.DeleteIfExist(address);
        }

        public async Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation)
        {
            return await _balanceRepository.GetBalances(take, continuation);
        }

        public async Task<IWalletBalance> UpdateBalance(string address)
        {
            var wallet = await _observableWalletRepository.Get(address);
            if (wallet != null)
            {
                return await UpdateBalance(wallet);
            }

            return null;
        }

        public async Task<IWalletBalance> UpdateBalance(IObservableWallet wallet)
        {
            if (wallet != null)
            {
                var balance = await _blockChainProvider.GetBalanceSatoshiFromUnspentOutputs(wallet.Address, _confirmationsSettings.MinConfirmationsToDetectOperation);
                var lastBlock = await _blockChainProvider.GetLastBlockHeight();

                return await UpdateBalance(wallet.Address, balance, lastBlock);
            }

            return null;
        }

        public async Task<IEnumerable<IWalletBalance>> UpdateBalanceBatched(IEnumerable<IObservableWallet> wallets)
        {
            var addressBalances = await _blockChainProvider.GetBalancesSatoshiFromUnspentOutputsBatched(
                wallets.Select(p => p.Address),
                _confirmationsSettings.MinConfirmationsToDetectOperation);

            var lastBlock = await _blockChainProvider.GetLastBlockHeight();

            var result = new List<IWalletBalance>();

            foreach (var addressBalanceResult in addressBalances)
            {
                result.Add(await UpdateBalance(addressBalanceResult.address, addressBalanceResult.balance, lastBlock));
            }

            return result;
        }

        private async Task<IWalletBalance> UpdateBalance(string address, long balance, int lastBlock)
        {
            if (balance != 0)
            {
                var walletBalanceEntity = WalletBalance.Create(address, balance, lastBlock);
                await _balanceRepository.InsertOrReplace(walletBalanceEntity);

                return walletBalanceEntity;
            }

            await _balanceRepository.DeleteIfExist(address);
            return null;
        }
    }
}
