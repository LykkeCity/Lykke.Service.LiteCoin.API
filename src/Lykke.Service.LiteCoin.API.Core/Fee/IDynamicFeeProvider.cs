using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Fee
{
    public interface IDynamicFeeProvider
    {
        Task<int> GetFeePerKylobyte();
    }
}
