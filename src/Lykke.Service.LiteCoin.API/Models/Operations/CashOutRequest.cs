﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Models.Operations
{
    public class CashOutRequest
    {
        public string SourceWalletId { get; set; } 

        public string DestAddress { get; set; }

        public string AssetId { get; set; }

        public decimal Amount { get; set; }
    }
}
