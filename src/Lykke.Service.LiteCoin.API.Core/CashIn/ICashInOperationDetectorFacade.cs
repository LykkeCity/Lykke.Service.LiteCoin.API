﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashIn
{
    public interface ICashInOperationDetectorFacade
    {
        Task DetectCashInOps();
    }
}
