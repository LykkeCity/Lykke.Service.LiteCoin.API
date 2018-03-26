using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.Service.LiteCoin.API.Services.Fee.Contracts
{
    public class BlockCypherBlockchainStatsResponce
    {
        [JsonProperty("medium_fee_per_kb")]
        public int MediumFeePerKb { get; set; }
    }
}
