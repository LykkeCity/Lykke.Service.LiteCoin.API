using System;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Fee;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Fee
{
    internal class FeeRateFacade:IFeeRateFacade
    {
        private readonly FeeSettings _feeSettings;
        private readonly IDynamicFeeRateRepository _dynamicFeeRateRepository;
        private readonly IDynamicFeeProvider _dynamicFeeProvider;

        public FeeRateFacade(FeeSettings feeSettings, IDynamicFeeRateRepository dynamicFeeRateRepository, IDynamicFeeProvider dynamicFeeProvider)
        {
            _feeSettings = feeSettings;
            _dynamicFeeRateRepository = dynamicFeeRateRepository;
            _dynamicFeeProvider = dynamicFeeProvider;
        }

        public async Task<int> GetFeePerKiloByte()
        {
            var dynamicFee = await _dynamicFeeRateRepository.Get();


            if (dynamicFee == null)
            {
                return _feeSettings.DefaultFeePerKyloByte;
            }


            return Math.Max(Math.Min(dynamicFee.FeePerKb, _feeSettings.MaxFeePerKyloByte), _feeSettings.MinFeePerKyloByte);
        }

        public async Task UpdateFeeRate()
        {
            var feeRateFromProvider = await _dynamicFeeProvider.GetFeePerKylobyte();

            await _dynamicFeeRateRepository.InsertOrReplace(DynamicFeeRate.Create(feeRateFromProvider));
        }
    }
}
