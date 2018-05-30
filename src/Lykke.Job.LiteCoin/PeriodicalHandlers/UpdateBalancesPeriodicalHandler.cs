using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.LiteCoin.Functions;

namespace Lykke.Job.LiteCoin.PeriodicalHandlers
{
    public class UpdateBalancesPeriodicalHandler:TimerPeriod
    {
        private readonly UpdateBalanceFunctions _functions;

        public UpdateBalancesPeriodicalHandler(int periodMs, ILog log, UpdateBalanceFunctions functions) 
            : base(nameof(UpdateBalancesPeriodicalHandler), 
                periodMs,
                log.CreateComponentScope(nameof(UpdateBalancesPeriodicalHandler)))
        {
            _functions = functions;
        }

        public override Task Execute()
        {
            return _functions.UpdateBalances();
        }
    }
}
