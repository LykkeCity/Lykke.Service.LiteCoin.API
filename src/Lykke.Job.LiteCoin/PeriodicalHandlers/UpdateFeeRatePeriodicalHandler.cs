﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.LiteCoin.Functions;

namespace Lykke.Job.LiteCoin.PeriodicalHandlers
{
    public class UpdateFeeRatePeriodicalHandler:TimerPeriod
    {
        private readonly UpdateFeeRateFunctions _functions;

        public UpdateFeeRatePeriodicalHandler(int periodMs, ILog log, UpdateFeeRateFunctions functions) 
            : base(nameof(UpdateFeeRatePeriodicalHandler), 
                periodMs, 
                log.CreateComponentScope(nameof(UpdateObservableOperationsFunctions)))
        {
            _functions = functions;
        }

        public override Task Execute()
        {
            return _functions.UpdateDynamicFee();
        }
    }
}