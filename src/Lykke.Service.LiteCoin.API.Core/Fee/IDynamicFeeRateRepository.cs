using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Fee
{
    public interface IDynamicFeeRate
    {
        int FeePerKb { get; }
    }

    public class DynamicFeeRate:IDynamicFeeRate
    {
        public int FeePerKb { get; set; }

        public static DynamicFeeRate Create(int feePerKb)
        {
            return new DynamicFeeRate
            {
                FeePerKb = feePerKb
            };
        }
    }

    public interface IDynamicFeeRateRepository
    {   
        Task InsertOrReplace(IDynamicFeeRate dynamicFeeRate);
        Task<IDynamicFeeRate> Get();
    }
}
