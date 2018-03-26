using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Wallet;

namespace Lykke.Job.LiteCoin.Functions
{
    public class UpdateBalanceFunctions
    {
        private readonly IObservableWalletRepository _observableWalletRepository;
        private readonly IWalletBalanceService _walletBalanceService;
        private readonly ILog _log;

        public UpdateBalanceFunctions(IObservableWalletRepository observableWalletRepository, IWalletBalanceService walletBalanceService, ILog log)
        {
            _observableWalletRepository = observableWalletRepository;
            _walletBalanceService = walletBalanceService;
            _log = log;
        }

        [TimerTrigger("00:10:00")]
        public async Task UpdateBalances()
        {
            var wallets = (await _observableWalletRepository.GetAll()).ToList();

            foreach (var observableWallet in wallets)
            {
                try
                {
                    await _walletBalanceService.UpdateBalance(observableWallet);
                }
                catch (Exception e)
                {
                    await _log.WriteErrorAsync(nameof(UpdateBalanceFunctions), nameof(UpdateBalances), observableWallet.ToJson(), e);
                }
            }
        }
    }
}
