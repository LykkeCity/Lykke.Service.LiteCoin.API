using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.LiteCoin.API.Core.Fee;
using Lykke.Service.LiteCoin.API.Services.Fee.Contracts;

namespace Lykke.Service.LiteCoin.API.Services.Fee
{
    public class DynamicFeeProvider: IDynamicFeeProvider
    {
        private readonly FeeSettings _feeSettings;

        public DynamicFeeProvider(FeeSettings feeSettings)
        {
            _feeSettings = feeSettings;
        }


        public async Task<int> GetFeePerKylobyte()
        {
            return (await _feeSettings.DynamicFeeProviderUrl.AppendPathSegment("v1/ltc/main")
                .GetJsonAsync<BlockCypherBlockchainStatsResponce>()).MediumFeePerKb;
        }
    }
}
