using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Wallet;

namespace Lykke.Job.LiteCoin.Functions
{
    public class UpdateBalanceFunctions
    {
        private readonly IObservableWalletRepository _observableWalletRepository;
        private readonly IWalletBalanceService _walletBalanceService;
        private readonly ILog _log;
        private readonly WalletBalanceSettings _walletBalanceSettings;

        public UpdateBalanceFunctions(IObservableWalletRepository observableWalletRepository,
            IWalletBalanceService walletBalanceService,
            ILog log, 
            WalletBalanceSettings walletBalanceSettings)
        {
            _observableWalletRepository = observableWalletRepository;
            _walletBalanceService = walletBalanceService;
            _log = log;
            _walletBalanceSettings = walletBalanceSettings;
        }

        public async Task UpdateBalances()
        {
            string continuation = null;

            do
            {
                var pagedResult = await _observableWalletRepository.GetPaged(_walletBalanceSettings.BatchSizeOnUpdate,
                    continuation);

                continuation = pagedResult.Continuation;

                try
                {
                    await _walletBalanceService.UpdateBalanceBatched(pagedResult.Items);
                }
                catch (Exception e)
                {
                    throw new BusinessException($"Failed to update balance on addreses: {string.Join(", ", pagedResult.Items.Select(p => p.Address))}", ErrorCode.FailedToUpdateBalance, e);
                }

            } while (continuation!=null);
        }
    }
}
