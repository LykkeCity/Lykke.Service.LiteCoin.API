using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.LiteCoin.Settings
{
    public class PerodicalHandlerSettings
    {
        public TimeSpan UpdateBalancesPeriod { get; set; }

        public TimeSpan UpdateFeeRatePeriod { get; set; }

        public TimeSpan UpdateObservableOperationsPeriod { get; set; }
    }
}
