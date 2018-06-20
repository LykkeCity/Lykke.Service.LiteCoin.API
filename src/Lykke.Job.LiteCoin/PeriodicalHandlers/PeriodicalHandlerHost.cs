using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Common.Log;
using Lykke.Job.LiteCoin.Functions;
using Lykke.Job.LiteCoin.Settings;

namespace Lykke.Job.LiteCoin.PeriodicalHandlers
{
    public class PeriodicalHandlerHost:IDisposable
    {
        private readonly IEnumerable<TimerTrigger> _timerTriggers;

        public PeriodicalHandlerHost(UpdateFeeRateFunctions feeRateFunctions,
            UpdateBalanceFunctions balanceFunctions,
            UpdateObservableOperationsFunctions observableOperationsFunctions, 
            ILog log,
            PerodicalHandlerSettings perodicalHandlerSettings)
        {
            var feeRateTrigger = new TimerTrigger(nameof(UpdateFeeRateFunctions.UpdateDynamicFee), 
                perodicalHandlerSettings.UpdateFeeRatePeriod,
                log);
            feeRateTrigger.Triggered += (trigger, args, token) => feeRateFunctions.UpdateDynamicFee();

            var balanceTrigger = new TimerTrigger(nameof(UpdateBalanceFunctions.UpdateBalances), 
                perodicalHandlerSettings.UpdateBalancesPeriod,
                log);
            balanceTrigger.Triggered += (trigger, args, token) => balanceFunctions.UpdateBalances();

            var observableOperationsTrigger = new TimerTrigger(nameof(UpdateObservableOperationsFunctions.DetectUnconfirmedTransactions), 
                perodicalHandlerSettings.UpdateObservableOperationsPeriod,
                log);
            observableOperationsTrigger.Triggered += (trigger, args, token) => observableOperationsFunctions.DetectUnconfirmedTransactions();

            _timerTriggers = new[]
            {
                feeRateTrigger,
                balanceTrigger,
                observableOperationsTrigger
            };
        }

        public void Start()
        {
            foreach (var timerTrigger in _timerTriggers)
            {
                timerTrigger.Start();
            }
        }

        public void Stop()
        {
            foreach (var timerTrigger in _timerTriggers)
            {
                timerTrigger.Stop();
            }
        }

        public void Dispose()
        {
            foreach (var timerTrigger in _timerTriggers)
            {
                timerTrigger.Dispose();
            }
        }
    }
}
