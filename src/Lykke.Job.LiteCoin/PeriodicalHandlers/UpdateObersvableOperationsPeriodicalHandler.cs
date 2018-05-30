﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.LiteCoin.Functions;

namespace Lykke.Job.LiteCoin.PeriodicalHandlers
{
    public class UpdateObersvableOperationsPeriodicalHandler:TimerPeriod
    {
        private readonly UpdateObservableOperationsFunctions _functions;

        public UpdateObersvableOperationsPeriodicalHandler(int periodMs, ILog log, UpdateObservableOperationsFunctions functions) 
            : base(nameof(UpdateObersvableOperationsPeriodicalHandler), 
                periodMs, 
                log.CreateComponentScope(nameof(UpdateObersvableOperationsPeriodicalHandler)))
        {
            _functions = functions;
        }

        public override Task Execute()
        {
            return _functions.DetectUnconfirmedTransactions();
        }
    }
}
